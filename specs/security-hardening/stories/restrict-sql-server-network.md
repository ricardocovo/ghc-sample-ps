# User Story: Restrict SQL Server Network Access

## As a
Security engineer

## I want
SQL Server to only allow access from approved networks and services

## So that
database is not exposed to the public internet

### Acceptance Criteria
- [ ] `sql.bicep` restricts network access to required subnets/services
- [ ] Public network access is disabled
- [ ] Automated test verifies SQL Server is not publicly accessible
