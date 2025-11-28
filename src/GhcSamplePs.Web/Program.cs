using GhcSamplePs.Web.Components;
using GhcSamplePs.Web.Services;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Extensions;
using MudBlazor.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.Net.Http.Headers;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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
builder.Services.AddSingleton<IPlayerRepository, MockPlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Register Entity Framework Core DbContext with SQL Server
// Note: Currently using MockPlayerRepository for player data.
// When ready to switch to database persistence, uncomment below and update IPlayerRepository implementation.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddApplicationDbContext(
        connectionString: connectionString,
        enableSensitiveDataLogging: builder.Environment.IsDevelopment(),
        maxRetryCount: 5,
        maxRetryDelaySeconds: 30);
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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
