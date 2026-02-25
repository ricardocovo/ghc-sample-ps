# User Story: CI/CD Security Job

**As** a DevOps engineer
**I want** a security job in CI/CD that scans for vulnerable dependencies
**So that** builds fail if known vulnerabilities are present.

## Acceptance Criteria
- Job runs `dotnet list package --vulnerable`
- Fails build on any vulnerabilities
- .gitignore excludes security scan outputs
