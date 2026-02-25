# User Story: Security Headers Middleware

## Summary
As a developer, I want the application to set strong HTTP security headers (CSP, X-Frame-Options, X-Content-Type-Options, Referrer-Policy, Permissions-Policy) so that browsers enforce best-practice protections against XSS, clickjacking, and data leaks.

## Acceptance Criteria
- Middleware sets the following headers on all responses:
  - Content-Security-Policy (CSP)
  - X-Frame-Options: DENY
  - X-Content-Type-Options: nosniff
  - Referrer-Policy: no-referrer
  - Permissions-Policy: restricts camera, microphone, geolocation
- Headers are present in all HTTP responses (except static files if not feasible).
- CSP allows only self and trusted sources for scripts/styles/images.
- Middleware is tested with integration tests.
