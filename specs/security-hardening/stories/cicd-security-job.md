# User Story: CI/CD Security Job

## Summary
As a DevOps engineer, I want the CI/CD pipeline to include a security job that scans for vulnerable dependencies, so that new vulnerabilities are caught before deployment.

## Acceptance Criteria
- Pipeline runs `dotnet list package --vulnerable` as a separate security job.
- Build fails if critical vulnerabilities are found.
- .gitignore excludes security scan artifacts if any.
- Security job is documented in pipeline YAML.
