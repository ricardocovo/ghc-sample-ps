# User Story: HTTP Security Headers

**As** a security-conscious developer
**I want** all HTTP responses to include strict security headers (CSP, X-Frame-Options, etc.)
**So that** the application is protected against common web vulnerabilities like XSS and clickjacking.

## Acceptance Criteria
- All responses include Content-Security-Policy, X-Frame-Options, X-Content-Type-Options, Referrer-Policy, and Strict-Transport-Security headers
- Headers are set via middleware in Program.cs
- No duplicate or conflicting headers
