# User Story: Enable Blob Delete Retention & Fix ForwardedHeaders

## As a
Security engineer

## I want
blob delete retention enabled and ForwardedHeaders configured securely

## So that
accidental or malicious blob deletions can be recovered and header spoofing is prevented

### Acceptance Criteria
- [ ] `storage.bicep` and `storage-player-pictures.bicep` enable delete retention
- [ ] `Program.cs` configures ForwardedHeaders securely
- [ ] Automated test verifies retention and header config
