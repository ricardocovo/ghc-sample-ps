# User Story: Remove Entra ID Secrets from Config

## Summary
As a security-conscious developer, I want to ensure that no real Entra ID (Azure AD) secrets or client IDs are present in appsettings.json or source control, so that secrets are not leaked.

## Acceptance Criteria
- All Entra ID secrets and client IDs are removed from appsettings.json.
- appsettings.json contains only placeholders or references to environment variables.
- .gitignore is updated to exclude local secrets files.
- README documents how to configure secrets for local development.
