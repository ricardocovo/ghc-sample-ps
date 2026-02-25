# API Rate Limiting

## Description
Implement rate limiting: 100 requests/minute for general API, 10 requests/minute for health endpoints.

## Acceptance Criteria
- Rate limits are enforced per IP
- Health endpoint has stricter limit
- Exceeding limit returns 429 status
