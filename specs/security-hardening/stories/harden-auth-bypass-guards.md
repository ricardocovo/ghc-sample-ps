# Harden auth bypass guards (prevent test auth in Prod)

## Story
As a security engineer, I want to ensure that any test authentication or bypass logic cannot be enabled in production, so that only real authentication is possible in live environments.

## Acceptance Criteria
- All test/bypass auth code is disabled or fails safe in production.
- Environment checks are robust and cannot be spoofed by config.
- Add tests to verify bypass cannot be enabled in production.
