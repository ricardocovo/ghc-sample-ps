# Add security scanning to CI

## Story
As a DevOps engineer, I want to add security scanning to the CI pipeline, so that vulnerabilities are detected early in the development process.

## Acceptance Criteria
- Security scanning step is added to `deploy-application.yml`.
- Scans run on every PR and main branch build.
- Fails the build on critical vulnerabilities.
