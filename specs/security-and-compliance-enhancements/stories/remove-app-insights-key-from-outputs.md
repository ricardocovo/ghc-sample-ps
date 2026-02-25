# Remove App Insights Key from Outputs

## Description
Update `main.bicep` to remove Application Insights instrumentation key from outputs. Prevents accidental exposure.

## Acceptance Criteria
- No App Insights key in any Bicep output
- Verified via deployment output
