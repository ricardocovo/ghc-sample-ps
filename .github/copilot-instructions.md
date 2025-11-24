# GhcSamplePs - Blazor Clean Architecture Project

## Architecture Overview

This is a Blazor web application following **clean architecture** with strict separation of concerns:

- **GhcSamplePs.Web** - Blazor UI layer (components, pages, client-side logic)
- **GhcSamplePs.Core** - Business logic layer (services, repositories, domain models)
- **GhcSamplePs.Core.Tests** - Unit tests for Core project

**Critical Rule**: Core project is UI-agnostic and must never reference Web project.

## Project Structure

```
src/
├── GhcSamplePs.Web/         # UI components, pages, Blazor-specific code
└── GhcSamplePs.Core/        # Business logic, services, repositories, models
    ├── Models/              # Domain entities
    ├── Services/            # Business logic (interfaces + implementations)
    ├── Repositories/        # Data access (interfaces + implementations)
    ├── Validation/          # Business validation rules
    └── Extensions/          # Extension methods
```

## Key Patterns

### Dependency Direction
- Web → Core (Web depends on Core)
- Core has NO dependencies on Web or any UI framework

### Service Layer Pattern
- Define service interfaces in `Services/Interfaces/`
- Implement in `Services/Implementations/`
- Register in `Program.cs` using dependency injection
- Inject services into Blazor components using `@inject`

### Business Logic Location
- ✅ Business logic belongs in Core services
- ❌ NO business logic in `.razor` files or component code-behind
- Blazor components should only handle UI concerns and call services

## Development Workflows

### Build and Run
```powershell
dotnet build                           # Build entire solution
cd src/GhcSamplePs.Web && dotnet run  # Run web app
dotnet test                           # Run all tests
```

### Adding New Features
1. Create service interface and implementation in Core
2. Write unit tests in Core.Tests
3. Register service in Web/Program.cs
4. Create Blazor component and inject service
5. Update README files at all affected levels

## Code Conventions

- Follow `.github/instructions/csharp.instructions.md` for C# standards
- Follow `.github/instructions/blazor-architecture.instructions.md` for architecture patterns
- Use dependency injection throughout
- Keep components small and focused
- Use async/await for all I/O operations
- Write tests for all business logic in Core

## Testing Strategy

- All Core services must have unit tests in Core.Tests
- Test naming: `WhenCondition_ThenExpectedBehavior`
- Use xUnit framework (already configured)
- Mock external dependencies only, never Core implementation code

## Important Reminders

- Core project is the heart of business logic - keep it UI-agnostic and testable
- Always update README files when making significant changes
- Ensure `.gitignore` is comprehensive before committing
- Use conventional commit messages: `feat:`, `fix:`, `refactor:`, `test:`

## Deployment & Hosting

### Target Platform
- **Hosting**: Azure Container Apps
- **Database**: Azure SQL Server
- **Containerization**: All components run in Docker containers

### Container Requirements
- Create `Dockerfile` for GhcSamplePs.Web
- Use multi-stage builds for optimized image size
- Base image: `mcr.microsoft.com/dotnet/aspnet:10.0` for runtime
- Build image: `mcr.microsoft.com/dotnet/sdk:10.0`
- Expose appropriate ports (typically 8080 for non-root containers)

### Database Configuration
- Use Entity Framework Core with SQL Server provider
- Connection strings stored in Azure Key Vault or Container Apps secrets
- Apply migrations during deployment or startup
- Use `appsettings.json` for local development with LocalDB/SQL Express
- Use environment variables for production connection strings

### Azure Container Apps Considerations
- Configure health checks and readiness probes
- Set appropriate resource limits (CPU/memory)
- Use managed identity for Azure SQL authentication when possible
- Configure horizontal scaling rules based on HTTP traffic or CPU
- Enable container app ingress for external access
