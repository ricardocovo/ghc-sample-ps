# Content-Type Allowlist on Blob Upload

## Description
Update `BlobStorageService` to enforce a content-type allowlist for uploads. Only allow safe image types (e.g., jpeg, png, gif).

## Acceptance Criteria
- Only allowed content-types accepted
- Rejected uploads return clear error
- Tests cover allowed and rejected types
