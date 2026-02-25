# User Story: Pipeline Security Scan

## Summary
As a maintainer, I want the pipeline to fail if vulnerable packages are detected.

## Acceptance Criteria
- CI/CD pipeline includes a security job running `dotnet list package --vulnerable`.
- Build fails if vulnerabilities are found.
- .gitignore prevents accidental commit of security scan outputs.
