# Restrict SQL Server network access

## Story
As a database administrator, I want to restrict SQL Server network access to only trusted networks and services, so that the database is not exposed to the public internet.

## Acceptance Criteria
- SQL Server allows access only from required subnets/services.
- Public network access is disabled in `sql.bicep`.
- Firewall rules are least-privilege.
