# Security and Compliance Enhancements

## Overview
This specification covers a set of security, compliance, and operational improvements across the Blazor Clean Architecture solution. The goal is to harden the application, infrastructure, and CI/CD pipeline against common threats, ensure regulatory compliance, and improve operational safety.

## Scope
- HTTP security headers (CSP, X-Frame-Options, etc.)
- API rate limiting
- Environment-based controller restrictions
- Removal of sensitive values from source
- Azure storage and Key Vault hardening
- CI/CD security scanning
- Blob storage retention and content-type allowlist

## Acceptance Criteria
- All tasks below are implemented and verified
- No sensitive values remain in source
- Security headers and rate limiting are enforced
- Storage and Key Vault are hardened per spec
- CI/CD pipeline includes security scanning

## Stories
Each story in `stories/` details a specific task.
