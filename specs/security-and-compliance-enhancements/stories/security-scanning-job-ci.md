# Security Scanning Job in CI

## Description
Add a security scanning job to the CI pipeline (`deploy-application.yml`). Ensure code and dependencies are scanned for vulnerabilities.

## Acceptance Criteria
- Security scan runs on every PR
- Fails build on critical findings
- Results visible in CI logs
