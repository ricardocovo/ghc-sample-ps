# Security Hardening

## Overview
This feature implements a comprehensive set of security hardening measures across the Web, Core, Infrastructure, and CI/CD layers. The goal is to ensure robust protection against common web vulnerabilities, secure sensitive configuration, and enforce best practices for cloud and pipeline security.

## Scope
- Web Layer: Security headers, rate limiting, environment-based test endpoints, and removal of secrets from config.
- Core Layer: Content-type allowlist for image uploads.
- Infrastructure: Hardened Bicep templates for storage, Key Vault, and App Insights.
- CI/CD: Automated security job for dependency vulnerability scanning.

## Acceptance Criteria
- All specified security controls are implemented and tested.
- No sensitive values are present in source-controlled configuration files.
- Infrastructure as code enforces secure defaults.
- CI/CD pipeline fails on critical dependency vulnerabilities.

## User Stories
- [Security Headers Middleware](stories/security-headers-middleware.md)
- [API Rate Limiting](stories/api-rate-limiting.md)
- [Test Auth Controller Environment Guard](stories/test-auth-controller-env-guard.md)
- [Remove Entra ID Secrets from Config](stories/remove-entra-id-secrets.md)
- [Disable Public Blob Access](stories/disable-public-blob-access.md)
- [Remove App Insights Key from Outputs](stories/remove-app-insights-key.md)
- [CI/CD Security Job](stories/cicd-security-job.md)
- [Key Vault Default Deny](stories/keyvault-default-deny.md)
- [Blob Soft Delete Retention](stories/blob-soft-delete-retention.md)
- [Content-Type Allowlist for Uploads](stories/content-type-allowlist.md)
