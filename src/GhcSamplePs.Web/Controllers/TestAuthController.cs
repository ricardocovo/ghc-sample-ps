using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhcSamplePs.Core.Services.Interfaces;
using System.Security.Claims;

namespace GhcSamplePs.Web.Controllers;

/// <summary>
/// Test controller for verifying authentication state in E2E tests.
/// Only available in Testing environment for security.
/// </summary>
[ApiController]
[Route("api/[controller]")]
#if !RELEASE
[ApiExplorerSettings(IgnoreApi = true)]  // Hide from Swagger in non-testing environments
#endif
public class TestAuthController : ControllerBase
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<TestAuthController> _logger;

    public TestAuthController(ICurrentUserProvider currentUserProvider, ILogger<TestAuthController> logger)
    {
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the current authentication state for testing purposes.
    /// </summary>
    /// <returns>The current user information.</returns>
    [HttpGet("current-user")]
    public IActionResult GetCurrentUser()
    {
        var user = _currentUserProvider.GetCurrentUser();
        var isAuthenticated = _currentUserProvider.IsAuthenticated;

        if (!isAuthenticated || user == null)
        {
            return Ok(new
            {
                IsAuthenticated = false,
                User = (object?)null,
                Claims = Array.Empty<object>()
            });
        }

        var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            IsAuthenticated = true,
            User = new
            {
                Identity = user.Identity?.Name,
                AuthenticationType = user.Identity?.AuthenticationType,
                Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            },
            Claims = claims
        });
    }

    /// <summary>
    /// Tests if the current user has admin role.
    /// </summary>
    /// <returns>True if user has admin role.</returns>
    [HttpGet("is-admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public IActionResult IsAdmin()
    {
        return Ok(new { IsAdmin = true });
    }

    /// <summary>
    /// Tests if the current user has user role.
    /// </summary>
    /// <returns>True if user has user role.</returns>
    [HttpGet("is-user")]
    [Authorize(Policy = "RequireUserRole")]
    public IActionResult IsUser()
    {
        return Ok(new { IsUser = true });
    }

    /// <summary>
    /// Tests authenticated access.
    /// </summary>
    /// <returns>True if user is authenticated.</returns>
    [HttpGet("is-authenticated")]
    [Authorize(Policy = "RequireAuthenticatedUser")]
    public IActionResult IsAuthenticated()
    {
        return Ok(new { IsAuthenticated = true });
    }
}
