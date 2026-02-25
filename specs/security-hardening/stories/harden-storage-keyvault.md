# User Story: Harden Storage and Key Vault

## Summary
As an operator, I want storage and Key Vault to be locked down by default.

## Acceptance Criteria
- Public blob access is disabled in storage Bicep.
- Blob soft delete is enabled with 7-day retention.
- Key Vault defaultAction is set to Deny.
- App Insights key is not output from Bicep.
