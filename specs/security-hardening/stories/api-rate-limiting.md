# User Story: API Rate Limiting

## Summary
As an operator, I want to limit API request rates to prevent abuse and DoS attacks, ensuring fair usage and system stability.

## Acceptance Criteria
- API endpoints are limited to 100 requests per minute per IP.
- Health check endpoints are limited to 10 requests per minute per IP.
- Rate limiting returns HTTP 429 on excess requests.
- Limits are configurable via appsettings.
- Rate limiting is covered by automated tests.
