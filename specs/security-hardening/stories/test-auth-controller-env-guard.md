# User Story: Test Auth Controller Environment Guard

## Summary
As a developer, I want the TestAuthController to be enabled only in non-production environments, so that test endpoints are never exposed in production.

## Acceptance Criteria
- TestAuthController is only registered when ASPNETCORE_ENVIRONMENT is Development or Test.
- No #if !RELEASE preprocessor directives are used.
- Behavior is verified with integration tests.
