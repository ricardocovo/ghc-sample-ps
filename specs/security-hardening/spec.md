# Security Hardening

This feature implements a set of security and compliance improvements across the Web, Core, Infrastructure, and CI/CD layers. The goal is to ensure best practices for HTTP security headers, rate limiting, secret management, storage access, and vulnerability scanning are enforced throughout the stack.

## Scope
- Web: Security headers, rate limiting, runtime environment checks, secret removal
- Core: Content-type allowlist for image uploads
- Infrastructure: Storage access, soft delete, Key Vault lockdown, App Insights output
- CI/CD: Automated security job for dependency vulnerability scanning

## Acceptance Criteria
- All HTTP responses include strict security headers (CSP, X-Frame-Options, etc.)
- API and health endpoints are rate limited as specified
- No real secrets or keys are present in source or outputs
- Blob storage is private and has soft delete enabled
- Key Vault denies by default
- Only allowed image types are accepted in uploads
- Security job runs in CI and fails on vulnerabilities

## Out of Scope
- UI changes
- Non-security-related refactoring

## User Stories
- See `stories/` for detailed user stories.
