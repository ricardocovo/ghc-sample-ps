using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GhcSamplePs.Web.Services;

/// <summary>
/// Test authentication handler for Playwright E2E testing.
/// Authenticates users based on the test user provider state.
/// </summary>
public sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestCurrentUserProvider _testUserProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The authentication scheme options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    /// <param name="testUserProvider">The test user provider.</param>
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TestCurrentUserProvider testUserProvider)
        : base(options, logger, encoder)
    {
        _testUserProvider = testUserProvider ?? throw new ArgumentNullException(nameof(testUserProvider));
    }

    /// <inheritdoc/>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = _testUserProvider.GetCurrentUser();

        if (user?.Identity?.IsAuthenticated == true)
        {
            var ticket = new AuthenticationTicket(user, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
