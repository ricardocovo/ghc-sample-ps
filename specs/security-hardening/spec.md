# Security Hardening

## Overview
This feature implements a comprehensive set of security hardening measures across the Web, Core, Infrastructure, and CI/CD layers. The goal is to ensure robust protection against common web vulnerabilities, secure sensitive configuration, and enforce best practices for cloud and pipeline security.

## Scope
- Web Layer: Security headers, rate limiting, environment-based test endpoints, and removal of secrets from config.
- Core Layer: Strict content-type allowlist for file uploads.
- Infrastructure: Hardened Bicep templates for storage, Key Vault, and App Insights.
- CI/CD: Automated security scanning in the pipeline.

## Acceptance Criteria
- All specified security controls are implemented and tested.
- No sensitive values are present in source-controlled config files.
- Infrastructure templates enforce secure defaults.
- CI/CD pipeline fails on known vulnerabilities.

## User Stories
- [ ] As a developer, I want security headers enforced so that browsers block common attacks.
- [ ] As an operator, I want rate limiting to prevent abuse of public endpoints.
- [ ] As a developer, I want test-only endpoints to be available only in non-production environments.
- [ ] As a maintainer, I want no real secrets or keys in source-controlled config files.
- [ ] As a developer, I want file uploads to accept only safe image types.
- [ ] As an operator, I want storage and Key Vault to be locked down by default.
- [ ] As a maintainer, I want the pipeline to fail if vulnerable packages are detected.
