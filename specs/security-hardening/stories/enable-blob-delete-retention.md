# Enable blob delete retention & fix ForwardedHeaders

## Story
As an operations engineer, I want to enable blob delete retention and ensure ForwardedHeaders are configured correctly, so that accidental deletions can be recovered and reverse proxy scenarios are secure.

## Acceptance Criteria
- Blob delete retention is enabled in `storage.bicep` and `storage-player-pictures.bicep`.
- `Program.cs` configures ForwardedHeaders securely.
