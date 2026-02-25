# User Story: Content-Type Allowlist for Uploads

## Summary
As a developer, I want the image upload service to only accept specific image content-types (jpeg, png, gif, webp), so that malicious files cannot be uploaded.

## Acceptance Criteria
- UploadPlayerPictureAsync only allows image/jpeg, image/png, image/gif, image/webp.
- Attempts to upload other content-types are rejected with a clear error.
- Unit tests cover all allowed and disallowed types.
