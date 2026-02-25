# Restrict Key Vault network access

## Story
As a security engineer, I want to restrict Azure Key Vault network access to only trusted networks and services, so that secrets are not exposed to the public internet.

## Acceptance Criteria
- Key Vault allows access only from required subnets/services.
- Public network access is disabled in `keyvault.bicep`.
- Access policies are reviewed and least-privilege.
