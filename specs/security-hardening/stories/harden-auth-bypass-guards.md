# User Story: Harden Auth Bypass Guards

## As a
Security engineer

## I want
test authentication and bypass logic to be disabled in production environments

## So that
no unauthorized access is possible in production

### Acceptance Criteria
- [ ] Test/bypass auth code is only enabled in Development
- [ ] Production builds cannot enable test auth via config or code
- [ ] Automated test verifies bypass is not possible in production
