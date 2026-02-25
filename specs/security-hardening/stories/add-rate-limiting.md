# Add rate limiting to API endpoints

## Story
As a security engineer, I want to add rate limiting to API endpoints, so that abuse and denial-of-service attacks are mitigated.

## Acceptance Criteria
- Rate limiting middleware is added to `Program.cs`.
- Limits are configurable per endpoint.
- Excess requests are rejected with appropriate status code.
