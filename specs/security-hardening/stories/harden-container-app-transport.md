# User Story: Harden Container App Transport & Env Defaults

## As a
Security engineer

## I want
all container app traffic to use secure transport and secure environment defaults

## So that
traffic is encrypted and containers are not exposed to unnecessary risk

### Acceptance Criteria
- [ ] `containerapp.bicep` enforces HTTPS-only ingress
- [ ] `main.bicep` sets secure environment defaults
- [ ] Automated test verifies only HTTPS is allowed
