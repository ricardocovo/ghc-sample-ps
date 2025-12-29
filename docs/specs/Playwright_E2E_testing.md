# Plan: Add Playwright E2E Testing to Blazor Application

Set up comprehensive end-to-end testing with Playwright for the Blazor web application, including local development support and CI/CD integration for automated testing on every build.

## Steps

1. Create E2E test project with Playwright.NUnit package in tests/ directory following playwright-dotnet.instructions.md
2. Configure test environment with in-memory database, mock authentication, and test data seeding in new test project
3. Implement core test scenarios for authentication flows, player management, admin dashboard, and navigation using page object pattern
4. Add GitHub Actions workflow for .github/workflows/test.yml to run unit tests and Playwright E2E tests on every push
5. Set up local development with npm scripts and dotnet test integration for running tests locally

## Further Considerations

- Authentication strategy - Mock Entra ID vs test tenant vs anonymous testing? Recommend mocking for speed
- Test data management - Fresh database per test run vs shared fixtures? Recommend isolated per test
- Browser coverage - Chromium only vs multi-browser testing? Start with Chromium, add others later
- CI performance - Parallel test execution vs sequential? Configure parallel for faster builds
