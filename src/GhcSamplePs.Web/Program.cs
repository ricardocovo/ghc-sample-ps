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

var builder = WebApplication.CreateBuilder(args);

// Add authentication services with Microsoft Identity Web
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

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
