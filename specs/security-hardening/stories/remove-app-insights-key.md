# User Story: Remove App Insights Key from Outputs

## Summary
As a security best practice, I want the App Insights instrumentation key to not be output from infrastructure templates, so that it is not accidentally leaked or logged.

## Acceptance Criteria
- main.bicep does not output the App Insights instrumentation key.
- No sensitive keys are present in deployment outputs.
- Change is validated by reviewing deployment output.
