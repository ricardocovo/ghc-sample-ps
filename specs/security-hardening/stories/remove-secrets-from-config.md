# User Story: Remove Secrets from Config

## Summary
As a maintainer, I want no real secrets or keys in source-controlled config files.

## Acceptance Criteria
- All secrets, keys, and sensitive values are removed from appsettings.json.
- .gitignore prevents accidental commit of local secrets files.
- README documents secret management best practices.
