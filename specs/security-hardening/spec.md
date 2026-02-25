# Security Hardening Plan

## Overview

This document outlines a comprehensive security hardening plan for the application and its Azure infrastructure. The plan addresses critical, high, and medium-priority tasks to remove sensitive data, enforce network and storage security, and improve operational resilience. Each task is tracked as a user story for implementation.

## Tasks

| # | Task | Priority | Files |
|---|------|----------|-------|
| 1 | **Remove Azure AD credentials from source control** | Critical | `appsettings.json`, `appsettings.Development.json` |
| 2 | **Harden auth bypass guards** (prevent test auth in Prod) | Critical | `Program.cs`, `TestAuthController.cs` |
| 3 | **Disable public blob access** for player pictures | Critical | `storage-player-pictures.bicep` |
| 4 | **Harden Container App transport & env default** | High | `containerapp.bicep`, `main.bicep` |
| 5 | **Restrict Key Vault network access** | High | `keyvault.bicep` |
| 6 | **Restrict SQL Server network access** | High | `sql.bicep` |
| 7 | **Add rate limiting** to API endpoints | High | `Program.cs` |
| 8 | **Enable blob delete retention & fix ForwardedHeaders** | Medium | `storage.bicep`, `storage-player-pictures.bicep`, `Program.cs` |
| 9 | **Enable deterministic NuGet restores** | Medium | `.gitignore` |
| 10 | **Add security scanning to CI** | Medium | `deploy-application.yml` |

**Dependencies:** Tasks 1–9 are independent. Task 10 depends on Task 9 (lock files must be committed first).

## Open Questions
- Review open questions at the bottom of `plan.md` before implementation.

