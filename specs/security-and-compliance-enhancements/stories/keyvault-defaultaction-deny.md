# Key Vault Default Action Deny

## Description
Update `keyvault.bicep` to set `defaultAction: Deny` for network ACLs, blocking all traffic except explicitly allowed.

## Acceptance Criteria
- Key Vault blocks all by default
- Only allowed networks can access
