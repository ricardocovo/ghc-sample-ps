# Harden Container App transport & environment defaults

## Story
As a platform engineer, I want to enforce secure transport (HTTPS) and set secure environment defaults for the Container App, so that all traffic is encrypted and misconfiguration is minimized.

## Acceptance Criteria
- Container App only accepts HTTPS traffic.
- Default environment variables are secure and do not leak secrets.
- Bicep files (`containerapp.bicep`, `main.bicep`) enforce these settings.
