# User Story: Key Vault Default Deny

**As** a security engineer
**I want** the Key Vault to deny all access by default
**So that** only explicitly allowed identities can access secrets.

## Acceptance Criteria
- Key Vault Bicep sets defaultAction: 'Deny'
- No secrets are accessible unless permitted
