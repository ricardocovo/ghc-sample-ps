## Product Requirements Document: Security Hardening for GhcSamplePs

### Overview

A security audit of the GhcSamplePs Blazor application has identified **6 critical**, **5 high**, and **5 medium** security vulnerabilities across the application code (`Program.cs`, `TestAuthController.cs`, `appsettings.json`) and infrastructure-as-code (Bicep modules for storage, Key Vault, SQL, and Container App). This PRD defines the remediation work required to close these gaps, prioritized by risk severity.

---

### Goals & Success Metrics

| Goal | Metric |
|------|--------|
| Eliminate all critical authentication/authorization bypass vectors | 0 code paths that disable auth in Production |
| Remove public blob access from player pictures storage | `allowBlobPublicAccess: false` and `publicAccess: None` in Bicep |
| Remove committed credentials from source control | No Azure AD identifiers in `appsettings.json` |
| Add rate limiting to API endpoints | Rate limiting middleware registered in `Program.cs` |
| Restrict Key Vault and SQL Server network access | `publicNetworkAccess: Disabled` or scoped network rules |
| Add secret scanning and SAST to CI | Workflow steps present in `deploy-application.yml` |

---

### User Stories

#### Story 1: Prevent Authentication Bypass in Production
**As a** security engineer, **I want** the test authentication provider and authorization bypass to be impossible to activate in Production **so that** no misconfiguration can disable auth.

Acceptance Criteria:
- [ ] `TestAuthController` is only registered when `ASPNETCORE_ENVIRONMENT == "Testing"` (not via config flag alone)
- [ ] `TestAuthController.GetCurrentUser` endpoint has `[Authorize]` attribute
- [ ] `Authentication:UseTestProvider` config flag is ignored when environment is `Production` or `Staging`
- [ ] `Authentication:BypassEntraId` config flag is ignored when environment is `Production` or `Staging`
- [ ] `FallbackPolicy` is always set to `DefaultPolicy` in Production regardless of config flags
- [ ] Unit test verifies authorization is enforced in non-Testing environments

#### Story 2: Remove Public Blob Access from Player Pictures Storage
**As a** security engineer, **I want** player picture blobs to be private **so that** only authenticated requests with SAS tokens can access them.

Acceptance Criteria:
- [ ] `storage-player-pictures.bicep`: `allowBlobPublicAccess` set to `false`
- [ ] `storage-player-pictures.bicep`: `publicAccess` set to `None` on the container
- [ ] `BlobStorageService` continues to generate SAS tokens for authorized access (no behavioral change)
- [ ] Verify existing SAS token flow still works after removing public access

#### Story 3: Remove Azure AD Identifiers from Committed Config
**As a** developer, **I want** Azure AD `TenantId`, `ClientId`, and `Domain` to be provided via environment variables or Key Vault **so that** no tenant identity info is in source control.

Acceptance Criteria:
- [ ] `appsettings.json` has empty/placeholder values for `AzureAd:TenantId`, `AzureAd:ClientId`, `AzureAd:Domain`
- [ ] `appsettings.Development.json` documents how to set values via `dotnet user-secrets` or env vars
- [ ] Container App Bicep already passes these as secrets (verified — no change needed there)
- [ ] Application still starts correctly with env var overrides

#### Story 4: Enforce HTTPS/TLS for All Traffic
**As a** security engineer, **I want** all traffic to use TLS **so that** data in transit is protected.

Acceptance Criteria:
- [ ] Container App ingress `transport` changed from `http` to `auto` (or `http2`) in `containerapp.bicep`
- [ ] `ASPNETCORE_ENVIRONMENT` default in `main.bicep` changed from `Development` to `Production`
- [ ] HSTS is applied in all non-Development environments (already correct, but environment default fix ensures it fires)

#### Story 5: Add Rate Limiting
**As a** platform engineer, **I want** rate limiting on API endpoints **so that** the application is protected from abuse.

Acceptance Criteria:
- [ ] `Microsoft.AspNetCore.RateLimiting` middleware added to `Program.cs`
- [ ] Fixed-window or sliding-window rate limiter configured with sensible defaults (e.g., 100 requests/minute per IP)
- [ ] Health check endpoint (`/health`) is excluded from rate limiting
- [ ] Rate limit response returns `429 Too Many Requests`

#### Story 6: Restrict Key Vault Network Access
**As a** security engineer, **I want** Key Vault to deny public network access **so that** only authorized Azure services can reach it.

Acceptance Criteria:
- [ ] `keyvault.bicep`: `publicNetworkAccess` set to `Disabled` or `networkAcls.defaultAction` set to `Deny`
- [ ] `bypass: AzureServices` retained for Container App Managed Identity access
- [ ] Deployment still succeeds with Container App accessing Key Vault via Managed Identity

#### Story 7: Restrict SQL Server Network Access
**As a** security engineer, **I want** SQL Server public access scoped to only necessary IPs **so that** the attack surface is minimized.

Acceptance Criteria:
- [ ] `sql.bicep`: Consider setting `publicNetworkAccess: Disabled` for prod (or restrict via `allowedIpRanges` only)
- [ ] Azure Services firewall rule (`0.0.0.0`) retained for Container App Managed Identity

#### Story 8: Add Secret Scanning and SAST to CI
**As a** platform engineer, **I want** automated security scanning in the CI pipeline **so that** vulnerabilities and leaked secrets are caught before merge.

Acceptance Criteria:
- [ ] GitHub Advanced Security or equivalent secret scanning enabled
- [ ] A SAST step (e.g., `dotnet format --verify-no-changes`, CodeQL, or `security-scan` action) added to `deploy-application.yml`
- [ ] Pipeline fails on detected secrets or critical SAST findings

#### Story 9: Enable Deterministic NuGet Restores
**As a** developer, **I want** `packages.lock.json` committed to source **so that** builds are deterministic and supply chain attacks are mitigated.

Acceptance Criteria:
- [ ] Remove `packages.lock.json` from `.gitignore` (line 107)
- [ ] Generate lock files with `dotnet restore --use-lock-file`
- [ ] Commit the generated lock files
- [ ] CI uses `--locked-mode` for restores

#### Story 10: Fix Remaining Medium-Severity Issues
**As a** security engineer, **I want** medium-severity gaps closed **so that** overall security posture improves.

Acceptance Criteria:
- [ ] Enable blob delete retention in `storage.bicep` and `storage-player-pictures.bicep` (`enabled: true`, `days: 7`)
- [ ] Restrict `ForwardedHeaders` to known proxy networks instead of clearing all (`Program.cs:315-316`)
- [ ] Remove `appInsightsInstrumentationKey` from Bicep outputs (`main.bicep:268`) — use connection string instead
- [ ] Reduce PII logging: change user ID log at `Program.cs:134-136` to `Debug` level or redact

---

### Technical Requirements

#### Files to Modify

| File | Changes |
|------|---------|
| `src/GhcSamplePs.Web/Program.cs` | Guard test auth by environment, enforce FallbackPolicy, add rate limiting, fix ForwardedHeaders, reduce PII logging |
| `src/GhcSamplePs.Web/Controllers/TestAuthController.cs` | Add `[Authorize]` to `GetCurrentUser`, add environment guard |
| `src/GhcSamplePs.Web/appsettings.json` | Remove AzureAd `TenantId`, `ClientId`, `Domain` values |
| `infra/modules/storage-player-pictures.bicep` | Disable public blob access |
| `infra/modules/storage.bicep` | Enable delete retention |
| `infra/modules/keyvault.bicep` | Restrict network access |
| `infra/modules/sql.bicep` | Restrict public network access |
| `infra/modules/containerapp.bicep` | Change transport to `auto`, change environment default |
| `infra/main.bicep` | Change environment default to `Production`, remove InstrumentationKey output |
| `.gitignore` | Remove `packages.lock.json` exclusion |
| `.github/workflows/deploy-application.yml` | Add SAST/secret scanning steps |

#### Integration Points
- `BlobStorageService.cs` — SAS token generation must continue to work after public access removal
- Container App Managed Identity — must still access Key Vault and SQL after network restrictions
- CI/CD — new scanning steps must not break existing deployment flow

#### Performance Requirements
- Rate limiting should not add measurable latency to normal traffic (<1ms overhead per request)

#### Security Requirements
- All changes must pass existing tests (`dotnet test`)
- No secrets introduced in any committed file

---

### Out of Scope

- Implementing full domain event audit trails for player CRUD operations (architectural change)
- Adding VNet integration for Container App (requires infrastructure redesign)
- Migrating to Azure Private Endpoints (requires networking changes)
- Adding Playwright E2E tests for auth flows
- PCI-DSS/SOX/LGPD full compliance audit

---

### Open Questions

1. **Key Vault network restriction**: Should we use Private Endpoints, or is `defaultAction: Deny` with `bypass: AzureServices` sufficient for the current architecture?
2. **SQL Server network access**: Should `publicNetworkAccess` be fully disabled for prod, or keep the parameterized `allowedIpRanges` approach for developer access?
3. **Rate limiting thresholds**: What are the expected traffic patterns? Is 100 req/min per IP appropriate, or should it be higher?
4. **ForwardedHeaders**: Do we know the specific Azure Container Apps proxy IP ranges to whitelist, or should we use the Azure service tag?
5. **SAST tooling**: Is GitHub Advanced Security (CodeQL) available on this repository, or should we use an alternative like `dotnet-security-scan`?


