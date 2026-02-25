# Security Headers Middleware

## Description
Add middleware to the HTTP pipeline to set security headers: Content-Security-Policy (CSP), X-Frame-Options, X-Content-Type-Options, Referrer-Policy, and Strict-Transport-Security.

## Acceptance Criteria
- Middleware sets all required headers on every response
- CSP is restrictive but allows app functionality
- No duplicate or conflicting headers
