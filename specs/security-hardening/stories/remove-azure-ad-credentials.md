# Remove Azure AD credentials from source control

## Story
As a developer, I want to ensure that no Azure AD credentials or secrets are present in any source-controlled configuration files, so that the application is not at risk of credential leakage.

## Acceptance Criteria
- All Azure AD credentials/secrets are removed from `appsettings.json` and `appsettings.Development.json`.
- Configuration files reference environment variables or secure vaults for secrets.
- Git history is reviewed for accidental credential commits.
- Add `.gitignore` rules if needed to prevent future leaks.

## Notes
- Coordinate with DevOps to rotate any exposed credentials.
