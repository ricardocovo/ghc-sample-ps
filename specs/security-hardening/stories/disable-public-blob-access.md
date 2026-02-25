# User Story: Disable Public Blob Access

## Summary
As an operator, I want to ensure that blob storage containers do not allow public access, so that sensitive files are never exposed to the internet.

## Acceptance Criteria
- Bicep template for player pictures storage disables public blob/container access.
- No blobs or containers are accessible without authentication.
- Configuration is validated with Azure CLI or portal.
