# User Story: Key Vault Default Deny

## Summary
As an operator, I want the Key Vault to have a default network action of 'Deny', so that only explicitly allowed networks can access secrets.

## Acceptance Criteria
- keyvault.bicep sets `defaultAction: 'Deny'` for network ACLs.
- Only whitelisted networks can access Key Vault.
- Configuration is validated in Azure portal or CLI.
