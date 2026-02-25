# User Story: Private Blob Storage

**As** a cloud architect
**I want** player picture blob storage to be private and have soft delete enabled
**So that** uploaded images are not publicly accessible and can be recovered if deleted.

## Acceptance Criteria
- Public blob access is disabled in Bicep
- Soft delete is enabled with 7-day retention
- Existing storage is migrated if needed
