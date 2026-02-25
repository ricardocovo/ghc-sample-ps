# User Story: Content-Type Allowlist for Image Uploads

**As** a backend developer
**I want** only specific image content-types allowed for player picture uploads
**So that** malicious or unsupported files are rejected.

## Acceptance Criteria
- Only image/jpeg, image/png, image/gif, image/webp are accepted
- Validation occurs in UploadPlayerPictureAsync
- Tests cover all allowed and disallowed types
