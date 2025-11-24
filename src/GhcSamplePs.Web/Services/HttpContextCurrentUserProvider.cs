using System.Security.Claims;
using GhcSamplePs.Core.Services.Interfaces;

namespace GhcSamplePs.Web.Services;

/// <summary>
/// Provides access to the current user's claims principal via HttpContext.
/// This implementation bridges the Core project's ICurrentUserProvider interface
/// with ASP.NET Core's HttpContextAccessor.
/// </summary>
public sealed class HttpContextCurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpContextCurrentUserProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public HttpContextCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc/>
    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
