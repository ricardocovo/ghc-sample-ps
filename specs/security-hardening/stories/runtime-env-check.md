# User Story: Runtime Environment Check for TestAuthController

**As** a developer
**I want** the TestAuthController to be enabled only in non-production environments
**So that** test endpoints are never exposed in production, regardless of build configuration.

## Acceptance Criteria
- TestAuthController is only enabled if ASPNETCORE_ENVIRONMENT is not Production
- No use of #if !RELEASE preprocessor directives
- Behavior is controlled at runtime
