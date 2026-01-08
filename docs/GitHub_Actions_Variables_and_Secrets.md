# GitHub Actions: Default Variables and Secrets Reference

This document explains all the variables, contexts, and secrets available by default when writing GitHub Actions workflows.

## Table of Contents

- [Contexts Overview](#contexts-overview)
- [GitHub Context](#github-context)
- [Environment Variables](#environment-variables)
- [Secrets](#secrets)
- [Runner Context](#runner-context)
- [Job Context](#job-context)
- [Steps Context](#steps-context)
- [Strategy and Matrix Context](#strategy-and-matrix-context)
- [Needs Context](#needs-context)
- [Inputs Context](#inputs-context)

---

## Contexts Overview

GitHub Actions provides several **contexts** that contain information about workflow runs, jobs, steps, and more. You access them using the `${{ }}` expression syntax.

| Context | Description |
|---------|-------------|
| `github` | Information about the workflow run and the event that triggered it |
| `env` | Environment variables set in the workflow, job, or step |
| `vars` | Repository, organization, or environment variables |
| `secrets` | Secrets available to the workflow |
| `job` | Information about the currently running job |
| `steps` | Information about the steps in the current job |
| `runner` | Information about the runner executing the job |
| `strategy` | Information about the matrix strategy |
| `matrix` | The matrix properties defined in the workflow |
| `needs` | Outputs from jobs that the current job depends on |
| `inputs` | Inputs for reusable workflows or manually triggered workflows |

---

## GitHub Context

The `github` context contains information about the workflow run and the event that triggered it.

### Common Properties

| Property | Description | Example Value |
|----------|-------------|---------------|
| `github.actor` | The username that triggered the workflow | `"octocat"` |
| `github.repository` | Owner and repository name | `"owner/repo"` |
| `github.repository_owner` | Repository owner | `"owner"` |
| `github.ref` | Branch or tag ref that triggered the workflow | `"refs/heads/main"` |
| `github.ref_name` | Short ref name (branch or tag) | `"main"` |
| `github.sha` | Full commit SHA | `"a1b2c3d4..."` |
| `github.event_name` | Name of the event that triggered the workflow | `"push"` |
| `github.workflow` | Name of the workflow | `"CI"` |
| `github.run_id` | Unique identifier for the workflow run | `1234567890` |
| `github.run_number` | Sequential run number for the workflow | `42` |
| `github.job` | The job_id of the current job | `"build"` |
| `github.action` | The unique identifier of the action | `"__run"` |
| `github.workspace` | Default working directory on the runner | `"/home/runner/work/repo/repo"` |
| `github.token` | Auto-generated token for authentication | `"ghs_xxx..."` |
| `github.server_url` | GitHub server URL | `"https://github.com"` |
| `github.api_url` | GitHub API URL | `"https://api.github.com"` |
| `github.base_ref` | Base branch for pull requests | `"main"` |
| `github.head_ref` | Head branch for pull requests | `"feature-branch"` |

### Code Examples

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    steps:
      - name: Print GitHub context properties
        run: |
          echo "Actor: ${{ github.actor }}"
          echo "Repository: ${{ github.repository }}"
          echo "Repository Owner: ${{ github.repository_owner }}"
          echo "Ref: ${{ github.ref }}"
          echo "Ref Name: ${{ github.ref_name }}"
          echo "SHA: ${{ github.sha }}"
          echo "Short SHA: ${{ github.sha.substring(0, 7) }}"
          echo "Event: ${{ github.event_name }}"
          echo "Workflow: ${{ github.workflow }}"
          echo "Run ID: ${{ github.run_id }}"
          echo "Run Number: ${{ github.run_number }}"
          echo "Job: ${{ github.job }}"
          echo "Workspace: ${{ github.workspace }}"

      - name: Conditional based on branch
        if: github.ref_name == 'main'
        run: echo "This is the main branch!"

      - name: Conditional based on event
        if: github.event_name == 'pull_request'
        run: |
          echo "PR from: ${{ github.head_ref }}"
          echo "PR to: ${{ github.base_ref }}"
```

### Using GITHUB_TOKEN

The `github.token` (also available as `secrets.GITHUB_TOKEN`) is automatically created for each workflow run:

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    steps:
      - name: Use GitHub token to call API
        run: |
          curl -H "Authorization: token ${{ github.token }}" \
            ${{ github.api_url }}/repos/${{ github.repository }}/issues

      - name: Create a release (using token)
        uses: actions/create-release@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          tag_name: v1.0.0
          release_name: Release v1.0.0
```

---

## Environment Variables

### Default Environment Variables

GitHub automatically sets these environment variables for every workflow run. Access them in shell commands using `$VARIABLE_NAME`.

| Variable | Description |
|----------|-------------|
| `GITHUB_ACTIONS` | Always `true` when running in GitHub Actions |
| `GITHUB_ACTOR` | The user that triggered the workflow |
| `GITHUB_REPOSITORY` | Owner/repository name |
| `GITHUB_REF` | Branch or tag ref |
| `GITHUB_REF_NAME` | Short ref name |
| `GITHUB_SHA` | Commit SHA |
| `GITHUB_WORKFLOW` | Workflow name |
| `GITHUB_RUN_ID` | Unique workflow run ID |
| `GITHUB_RUN_NUMBER` | Sequential run number |
| `GITHUB_JOB` | Current job ID |
| `GITHUB_WORKSPACE` | Working directory path |
| `GITHUB_EVENT_NAME` | Event name that triggered the workflow |
| `GITHUB_EVENT_PATH` | Path to the event payload JSON file |
| `GITHUB_SERVER_URL` | GitHub server URL |
| `GITHUB_API_URL` | GitHub API URL |
| `GITHUB_OUTPUT` | File path for setting step outputs |
| `GITHUB_ENV` | File path for setting environment variables |
| `GITHUB_STEP_SUMMARY` | File path for job summary markdown |
| `RUNNER_OS` | Operating system of the runner |
| `RUNNER_ARCH` | Architecture of the runner |
| `RUNNER_NAME` | Name of the runner |
| `RUNNER_TEMP` | Temporary directory path |
| `RUNNER_TOOL_CACHE` | Tool cache directory path |

### Code Examples

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    steps:
      - name: Print default environment variables
        run: |
          echo "Running on GitHub Actions: $GITHUB_ACTIONS"
          echo "Actor: $GITHUB_ACTOR"
          echo "Repository: $GITHUB_REPOSITORY"
          echo "Ref: $GITHUB_REF"
          echo "SHA: $GITHUB_SHA"
          echo "Workspace: $GITHUB_WORKSPACE"
          echo "Runner OS: $RUNNER_OS"
          echo "Runner Arch: $RUNNER_ARCH"

      - name: Set output using GITHUB_OUTPUT
        id: set-version
        run: echo "version=1.0.0" >> $GITHUB_OUTPUT

      - name: Use the output
        run: echo "Version is ${{ steps.set-version.outputs.version }}"

      - name: Set dynamic environment variable
        run: echo "MY_VAR=hello" >> $GITHUB_ENV

      - name: Use dynamic environment variable
        run: echo "MY_VAR is $MY_VAR"

      - name: Add to job summary
        run: |
          echo "## Build Results 🎉" >> $GITHUB_STEP_SUMMARY
          echo "| Status | Value |" >> $GITHUB_STEP_SUMMARY
          echo "|--------|-------|" >> $GITHUB_STEP_SUMMARY
          echo "| Build | ✅ Success |" >> $GITHUB_STEP_SUMMARY
```

### Custom Environment Variables

Define custom environment variables at workflow, job, or step level:

```yaml
env:
  WORKFLOW_VAR: "workflow-level"

jobs:
  example:
    runs-on: ubuntu-latest
    env:
      JOB_VAR: "job-level"
    steps:
      - name: Step with env
        env:
          STEP_VAR: "step-level"
        run: |
          echo "Workflow: $WORKFLOW_VAR"
          echo "Job: $JOB_VAR"
          echo "Step: $STEP_VAR"
          echo "Using context: ${{ env.WORKFLOW_VAR }}"
```

---

## Secrets

### Default Secrets

| Secret | Description |
|--------|-------------|
| `secrets.GITHUB_TOKEN` | Automatically created token for repository operations |

### User-Defined Secrets Levels

Secrets can be defined at three levels:

1. **Repository secrets** - Available to the specific repository
2. **Environment secrets** - Available only when using a specific environment
3. **Organization secrets** - Shared across multiple repositories

### Code Examples

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    environment: production  # Required for environment secrets
    steps:
      - name: Use GITHUB_TOKEN
        run: |
          curl -H "Authorization: Bearer ${{ secrets.GITHUB_TOKEN }}" \
            https://api.github.com/repos/${{ github.repository }}

      - name: Use custom secret
        env:
          API_KEY: ${{ secrets.API_KEY }}
        run: |
          # Secret is masked in logs
          echo "Using API key..."
          curl -H "Authorization: $API_KEY" https://api.example.com

      - name: Check if secret exists
        run: |
          if [ -n "${{ secrets.OPTIONAL_SECRET }}" ]; then
            echo "Secret is set"
          else
            echo "Secret is not set"
          fi
```

> **Security Note**: Secrets are automatically masked in logs. Never echo secrets directly or use them in URLs that might be logged.

---

## Runner Context

The `runner` context contains information about the runner executing the current job.

| Property | Description | Example Value |
|----------|-------------|---------------|
| `runner.name` | Name of the runner | `"Hosted Agent"` |
| `runner.os` | Operating system | `"Linux"`, `"Windows"`, `"macOS"` |
| `runner.arch` | CPU architecture | `"X64"`, `"ARM64"` |
| `runner.temp` | Temp directory path | `"/home/runner/work/_temp"` |
| `runner.tool_cache` | Tool cache directory | `"/opt/hostedtoolcache"` |

### Code Examples

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    steps:
      - name: Print runner info
        run: |
          echo "Runner Name: ${{ runner.name }}"
          echo "Runner OS: ${{ runner.os }}"
          echo "Runner Arch: ${{ runner.arch }}"
          echo "Temp Dir: ${{ runner.temp }}"
          echo "Tool Cache: ${{ runner.tool_cache }}"

      - name: OS-specific command
        run: |
          if [ "${{ runner.os }}" == "Linux" ]; then
            echo "Running on Linux"
          elif [ "${{ runner.os }}" == "Windows" ]; then
            echo "Running on Windows"
          fi
```

---

## Job Context

The `job` context contains information about the currently running job.

| Property | Description |
|----------|-------------|
| `job.status` | Current status: `success`, `failure`, or `cancelled` |
| `job.container` | Container information (if using containers) |
| `job.services` | Service containers defined for the job |

### Code Examples

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
    steps:
      - name: Check job status
        if: always()
        run: echo "Job status is ${{ job.status }}"

      - name: Access service info
        run: |
          echo "Postgres host: ${{ job.services.postgres.id }}"
```

---

## Steps Context

The `steps` context contains information about steps that have already run and have an `id`.

| Property | Description |
|----------|-------------|
| `steps.<step_id>.outputs.<name>` | Output value from a step |
| `steps.<step_id>.outcome` | Result before `continue-on-error` |
| `steps.<step_id>.conclusion` | Result after `continue-on-error` |

### Code Examples

```yaml
jobs:
  example:
    runs-on: ubuntu-latest
    steps:
      - name: Set output
        id: my-step
        run: echo "result=hello world" >> $GITHUB_OUTPUT

      - name: Use output
        run: echo "Result was: ${{ steps.my-step.outputs.result }}"

      - name: Failing step
        id: might-fail
        continue-on-error: true
        run: exit 1

      - name: Check previous step
        run: |
          echo "Outcome: ${{ steps.might-fail.outcome }}"
          echo "Conclusion: ${{ steps.might-fail.conclusion }}"
          # outcome = failure, conclusion = success (due to continue-on-error)
```

---

## Strategy and Matrix Context

The `strategy` and `matrix` contexts are used for matrix builds.

### Strategy Properties

| Property | Description |
|----------|-------------|
| `strategy.fail-fast` | Whether to cancel all jobs if one fails |
| `strategy.job-index` | Index of current job in matrix |
| `strategy.job-total` | Total number of jobs in matrix |
| `strategy.max-parallel` | Maximum parallel jobs |

### Code Examples

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    strategy:
      fail-fast: false
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        node: [18, 20, 22]
        include:
          - os: ubuntu-latest
            node: 22
            experimental: true
        exclude:
          - os: macos-latest
            node: 18
    steps:
      - name: Print matrix info
        run: |
          echo "OS: ${{ matrix.os }}"
          echo "Node: ${{ matrix.node }}"
          echo "Experimental: ${{ matrix.experimental }}"
          echo "Job Index: ${{ strategy.job-index }}"
          echo "Total Jobs: ${{ strategy.job-total }}"
```

---

## Needs Context

The `needs` context contains outputs from dependent jobs.

### Code Examples

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    outputs:
      version: ${{ steps.version.outputs.version }}
      artifact-name: ${{ steps.version.outputs.artifact }}
    steps:
      - name: Determine version
        id: version
        run: |
          echo "version=1.2.3" >> $GITHUB_OUTPUT
          echo "artifact=myapp-1.2.3" >> $GITHUB_OUTPUT

  deploy:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Use outputs from build job
        run: |
          echo "Deploying version: ${{ needs.build.outputs.version }}"
          echo "Artifact: ${{ needs.build.outputs.artifact-name }}"

      - name: Check build result
        run: echo "Build result: ${{ needs.build.result }}"

  final:
    needs: [build, deploy]
    runs-on: ubuntu-latest
    if: always()
    steps:
      - name: Check all job results
        run: |
          echo "Build: ${{ needs.build.result }}"
          echo "Deploy: ${{ needs.deploy.result }}"
```

---

## Inputs Context

The `inputs` context is used for:

1. **Manually triggered workflows** (`workflow_dispatch`)
2. **Reusable workflows** (`workflow_call`)

### Workflow Dispatch Inputs

```yaml
name: Manual Workflow

on:
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deployment environment'
        required: true
        default: 'staging'
        type: choice
        options:
          - staging
          - production
      version:
        description: 'Version to deploy'
        required: true
        type: string
      dry-run:
        description: 'Perform dry run only'
        required: false
        type: boolean
        default: false

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Print inputs
        run: |
          echo "Environment: ${{ inputs.environment }}"
          echo "Version: ${{ inputs.version }}"
          echo "Dry Run: ${{ inputs.dry-run }}"

      - name: Conditional step
        if: inputs.dry-run == false
        run: echo "Performing actual deployment..."
```

### Reusable Workflow Inputs

```yaml
# .github/workflows/reusable-build.yml
name: Reusable Build

on:
  workflow_call:
    inputs:
      node-version:
        description: 'Node.js version'
        required: false
        type: string
        default: '20'
    secrets:
      NPM_TOKEN:
        description: 'NPM authentication token'
        required: true

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/setup-node@v4
        with:
          node-version: ${{ inputs.node-version }}
          registry-url: 'https://registry.npmjs.org'

      - name: Install and build
        env:
          NODE_AUTH_TOKEN: ${{ secrets.NPM_TOKEN }}
        run: |
          npm ci
          npm run build
```

```yaml
# .github/workflows/main.yml - Calling the reusable workflow
name: Main CI

on: [push]

jobs:
  call-build:
    uses: ./.github/workflows/reusable-build.yml
    with:
      node-version: '22'
    secrets:
      NPM_TOKEN: ${{ secrets.NPM_TOKEN }}
```

---

## Quick Reference Cheat Sheet

```yaml
# Common patterns
${{ github.repository }}           # owner/repo
${{ github.ref_name }}             # branch or tag name
${{ github.sha }}                  # full commit SHA
${{ github.actor }}                # user who triggered
${{ github.event_name }}           # push, pull_request, etc.

${{ secrets.GITHUB_TOKEN }}        # auto-generated token
${{ secrets.MY_SECRET }}           # custom secret

${{ vars.MY_VARIABLE }}            # repository/org/env variable

${{ runner.os }}                   # Linux, Windows, macOS
${{ runner.arch }}                 # X64, ARM64

${{ steps.step-id.outputs.name }}  # step output
${{ needs.job-id.outputs.name }}   # job output
${{ needs.job-id.result }}         # success, failure, cancelled

${{ matrix.key }}                  # matrix value
${{ inputs.name }}                 # workflow_dispatch input

${{ env.MY_VAR }}                  # environment variable (context)
$MY_VAR                            # environment variable (shell)
```

---

## Additional Resources

- [GitHub Actions Contexts Documentation](https://docs.github.com/en/actions/learn-github-actions/contexts)
- [GitHub Actions Environment Variables](https://docs.github.com/en/actions/learn-github-actions/variables)
- [GitHub Actions Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Workflow Syntax Reference](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
