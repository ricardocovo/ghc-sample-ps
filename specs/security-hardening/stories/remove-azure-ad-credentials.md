# User Story: Remove Azure AD Credentials from Source Control

## As a
Security engineer

## I want
all Azure AD credentials and secrets to be removed from source control and configuration files

## So that
no sensitive information is exposed in the repository

### Acceptance Criteria
- [ ] All Azure AD credentials/secrets are removed from `appsettings.json` and `appsettings.Development.json`
- [ ] Configuration files reference environment variables or Key Vault for secrets
- [ ] No secrets are present in git history after removal
