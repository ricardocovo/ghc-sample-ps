# User Story: Environment-Based Test Endpoints

## Summary
As a developer, I want test-only endpoints to be available only in non-production environments.

## Acceptance Criteria
- TestAuthController endpoints are only enabled in Development or Test environments.
- No #if !RELEASE preprocessor directives remain.
- Production deployments never expose test endpoints.
