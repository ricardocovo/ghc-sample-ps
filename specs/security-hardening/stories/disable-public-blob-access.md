# User Story: Disable Public Blob Access for Player Pictures

## As a
Security engineer

## I want
public access to player picture blobs to be disabled

## So that
only authenticated/authorized users can access player images

### Acceptance Criteria
- [ ] `storage-player-pictures.bicep` disables public blob access
- [ ] Access to blobs requires authentication
- [ ] Automated test verifies public access is denied
