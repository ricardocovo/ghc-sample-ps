# User Story: API Rate Limiting

**As** an API consumer
**I want** rate limiting enforced on API and health endpoints
**So that** abuse and accidental overload are prevented.

## Acceptance Criteria
- API endpoints limited to 100 requests per minute per IP
- Health endpoints limited to 10 requests per minute per IP
- Limits are enforced via middleware
- Exceeding the limit returns HTTP 429
