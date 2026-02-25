# Disable public blob access for player pictures

## Story
As an infrastructure engineer, I want to ensure that the player pictures blob storage does not allow public access, so that only authenticated requests can retrieve images.

## Acceptance Criteria
- Public access is disabled in `storage-player-pictures.bicep`.
- Access is only possible via authorized application/API calls.
- Existing blobs are not exposed publicly.
