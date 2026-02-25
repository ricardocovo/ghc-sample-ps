# User Story: Remove Real Secrets from Source

**As** a security auditor
**I want** all real secrets, keys, and sensitive values removed from source code and outputs
**So that** the repository is safe for open source and audit.

## Acceptance Criteria
- No real Entra ID or App Insights keys in appsettings.json or Bicep outputs
- .gitignore updated to exclude local secrets
- README updated to instruct on secret management
