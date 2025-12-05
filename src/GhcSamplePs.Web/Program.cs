using GhcSamplePs.Web.Components;
using GhcSamplePs.Web.Services;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Extensions;
using MudBlazor.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.Net.Http.Headers;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Create a logger factory for early initialization logging (used in configuration callbacks)
using var earlyLoggerFactory = LoggerFactory.Create(config => 
{
    config.AddConsole();
});
var tokenRefreshLogger = earlyLoggerFactory.CreateLogger("TokenRefresh");

// Configure Azure credential for Managed Identity (used in production)
var azureCredential = new DefaultAzureCredential();

// Configure Data Protection with Azure Blob + Key Vault for production
if (!builder.Environment.IsDevelopment())
{
    var blobEndpoint = builder.Configuration["Storage:BlobEndpoint"];
    var vaultUri = builder.Configuration["KeyVault:VaultUri"];
    
    if (!string.IsNullOrEmpty(blobEndpoint) && !string.IsNullOrEmpty(vaultUri))
    {
        builder.Services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(
                new Uri($"{blobEndpoint}/dataprotection-keys/keys.xml"),
                azureCredential)
            .ProtectKeysWithAzureKeyVault(
                new Uri($"{vaultUri}/keys/dataprotection"),
                azureCredential);
    }
}

// Add authentication services with Microsoft Identity Web
// Configure token refresh handling with automatic refresh on expiration
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.GetSection("AzureAd").Bind(options);
        
        // Enable token persistence for refresh token usage
        options.SaveTokens = true;
        
        // Configure retry policy for token refresh operations (backchannel communication)
        // Uses exponential backoff with jitter for transient failures
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + 
                    TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    // Log retry attempts for token refresh operations
                    if (outcome.Exception is not null)
                    {
                        tokenRefreshLogger.LogWarning(
                            outcome.Exception,
                            "Token refresh retry attempt {RetryAttempt} after {DelaySeconds:F2}s due to {ExceptionType}",
                            retryAttempt,
                            timespan.TotalSeconds,
                            outcome.Exception.GetType().Name);
                    }
                    else
                    {
                        tokenRefreshLogger.LogWarning(
                            "Token refresh retry attempt {RetryAttempt} after {DelaySeconds:F2}s due to HTTP {StatusCode}",
                            retryAttempt,
                            timespan.TotalSeconds,
                            outcome.Result?.StatusCode);
                    }
                });
        
        // Configure backchannel HTTP handler with retry policy for transient failures
        options.BackchannelHttpHandler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = new HttpClientHandler()
        };
        
        // Set reasonable timeout for backchannel operations
        options.BackchannelTimeout = TimeSpan.FromSeconds(30);
        
        // Configure token refresh events for logging and error handling
        options.Events ??= new OpenIdConnectEvents();
        
        var existingOnTokenValidated = options.Events.OnTokenValidated;
        options.Events.OnTokenValidated = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("TokenRefresh");
            
            logger.LogInformation(
                "Token validated successfully for user {UserId}",
                context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");
            
            if (existingOnTokenValidated is not null)
            {
                await existingOnTokenValidated(context);
            }
        };
        
        var existingOnAuthenticationFailed = options.Events.OnAuthenticationFailed;
        options.Events.OnAuthenticationFailed = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("TokenRefresh");
            
            // Log token refresh failures with appropriate level based on exception type
            if (context.Exception is HttpRequestException)
            {
                // Transient network failure - log as warning since retry may succeed
                logger.LogWarning(
                    context.Exception,
                    "Transient authentication failure occurred. The request may be retried automatically.");
            }
            else
            {
                // Non-transient failure - log as error
                logger.LogError(
                    context.Exception,
                    "Authentication failed: {ErrorMessage}",
                    context.Exception.Message);
            }
            
            if (existingOnAuthenticationFailed is not null)
            {
                await existingOnAuthenticationFailed(context);
            }
        };
        
        var existingOnRemoteFailure = options.Events.OnRemoteFailure;
        options.Events.OnRemoteFailure = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("TokenRefresh");
            
            // Handle token refresh failures gracefully
            logger.LogWarning(
                context.Failure,
                "Remote authentication failure: {Error}. User will be redirected to sign-in.",
                context.Failure?.Message ?? "Unknown error");
            
            // Handle the failure gracefully by redirecting to home page
            // This prevents the error from being displayed to the user
            context.Response.Redirect("/");
            context.HandleResponse();
            
            if (existingOnRemoteFailure is not null)
            {
                await existingOnRemoteFailure(context);
            }
        };
    });

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // Require authenticated user by default
    options.FallbackPolicy = options.DefaultPolicy;
    
    // Define custom authorization policies
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
    
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole("User", "Admin"));
});

// Add cascading authentication state for Blazor components
builder.Services.AddCascadingAuthenticationState();

// Register Core services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, HttpContextCurrentUserProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Register Player Management services
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamPlayerRepository, EfTeamPlayerRepository>();
builder.Services.AddScoped<ITeamPlayerService, TeamPlayerService>();

// Register Player Statistic services
builder.Services.AddScoped<IPlayerStatisticRepository, EfPlayerStatisticRepository>();
builder.Services.AddScoped<IPlayerStatisticService, PlayerStatisticService>();

// Register Entity Framework Core DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException("Database connection string 'DefaultConnection' is required in production environments. Configure via environment variables or Azure Key Vault.");
    }
    // In development, DbContext registration is optional if no connection string is provided
}
else
{
    builder.Services.AddApplicationDbContext(
        connectionString: connectionString,
        enableSensitiveDataLogging: builder.Environment.IsDevelopment(),
        enableDetailedErrors: builder.Environment.IsDevelopment(),
        maxRetryCount: 5,
        maxRetryDelaySeconds: 30);

    // Add health checks including database connectivity check
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>("database");
}

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add response compression for better performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/octet-stream",
        "image/svg+xml"
    });
});
    
// Configure compression levels
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

var app = builder.Build();

// Apply database migrations automatically in development environment
// In production, migrations should be applied manually or via deployment pipeline
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
    if (context is not null)
    {
        try
        {
            app.Logger.LogInformation("Applying pending database migrations...");
            await context.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Failed to apply database migrations. This is expected if no database connection is configured.");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable response compression
app.UseResponseCompression();

// Configure static files with caching headers for production
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!app.Environment.IsDevelopment())
        {
            // Cache static assets for 1 year in production
            const int durationInSeconds = 60 * 60 * 24 * 365;
            ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                $"public,max-age={durationInSeconds}";
        }
    }
});

// Authentication and authorization middleware - order matters!
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Map Microsoft Identity UI controllers for sign-in/sign-out
app.MapControllers();

// Map health check endpoint (allows anonymous access for load balancers)
app.MapHealthChecks("/health").AllowAnonymous();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
