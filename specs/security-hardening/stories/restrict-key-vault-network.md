# User Story: Restrict Key Vault Network Access

## As a
Security engineer

## I want
Key Vault to only allow access from approved networks and services

## So that
secrets are not exposed to the public internet

### Acceptance Criteria
- [ ] `keyvault.bicep` restricts network access to required subnets/services
- [ ] Public network access is disabled
- [ ] Automated test verifies Key Vault is not publicly accessible
