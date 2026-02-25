# Enable deterministic NuGet restores

## Story
As a developer, I want to ensure NuGet package restores are deterministic and lock files are committed, so that builds are reproducible and secure.

## Acceptance Criteria
- `.gitignore` allows lock files to be committed.
- Lock files are present and up to date.
- Build is reproducible across environments.
