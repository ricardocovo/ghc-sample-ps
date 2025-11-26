# GhcSamplePs - Blazor Clean Architecture Sample

A sample Blazor Server application demonstrating clean architecture principles with Azure Entra ID External Identities integration.

## Project Overview

GhcSamplePs is a modern web application built with:
- **Blazor Server** - Interactive web UI using .NET 10
- **Clean Architecture** - Separation of concerns with layered architecture
- **Azure Entra ID** - Enterprise-grade authentication for external users
- **Azure Container Apps** - Cloud-native hosting platform

## Architecture

The solution follows clean architecture with strict separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                      │
│                   (GhcSamplePs.Web)                         │
│  - Blazor Components                                        │
│  - Pages and Layouts                                        │
│  - UI Services                                              │
└────────────────────────┬────────────────────────────────────┘
                         │ References
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    Business Logic Layer                     │
│                   (GhcSamplePs.Core)                        │
│  - Domain Models                                            │
│  - Business Services                                        │
│  - Repositories                                             │
│  - Validation                                               │
└─────────────────────────────────────────────────────────────┘
```

**Key Principle**: Core project is UI-agnostic and has no dependencies on Web project.

## Projects

### GhcSamplePs.Web
Blazor Server application containing:
- Blazor components (`.razor` files)
- Pages and routing
- Authentication UI
- Layout components
- Client-side validation

**Location**: `src/GhcSamplePs.Web/`  
**Documentation**: `src/GhcSamplePs.Web/README.md`

### GhcSamplePs.Core
Business logic layer containing:
- Domain models and entities
- Service interfaces and implementations
- Repository patterns
- Business validation rules
- Authentication/authorization logic

**Location**: `src/GhcSamplePs.Core/`  
**Documentation**: `src/GhcSamplePs.Core/README.md`

### GhcSamplePs.Core.Tests
Unit tests for business logic:
- Service tests
- Repository tests
- Model validation tests
- Test naming: `MethodName_Condition_ExpectedResult`

**Location**: `tests/GhcSamplePs.Core.Tests/`  
**Documentation**: `tests/GhcSamplePs.Core.Tests/README.md`

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Application framework |
| C# | 14 | Programming language |
| Blazor Server | 10.0 | Web UI framework |
| Azure Entra ID | External Identities | Authentication provider |
| Azure Key Vault | Latest | Secrets management |
| Azure Container Apps | Latest | Cloud hosting |
| xUnit | Latest | Testing framework |

## Prerequisites

To run this application locally, you need:

- [ ] **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- [ ] **Code Editor** - Visual Studio 2022, VS Code, or JetBrains Rider
- [ ] **Azure Subscription** - For Entra ID and cloud resources
- [ ] **Azure CLI** - For Key Vault access (optional)
- [ ] **Git** - For source control

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ricardocovo/ghc-sample-ps.git
cd ghc-sample-ps
```

### 2. Azure Infrastructure Setup

Before running the application, set up Azure Entra ID infrastructure:

1. **Create Entra ID Tenant** - Follow the detailed guide
2. **Register Application** - Configure OAuth settings
3. **Set Up Identity Providers** - Microsoft, Google, etc.
4. **Create Test Users** - For development testing
5. **Configure Key Vault** - For secrets management

📖 **Complete Guide**: [Azure Entra ID Setup Guide](docs/Azure_EntraID_Setup_Guide.md)

### 3. Configure Development Environment

After Azure infrastructure is ready:

1. **Update Configuration Files**
   - Edit `src/GhcSamplePs.Web/appsettings.Development.json`
   - Add Tenant ID and Client ID

2. **Store Client Secret**
   ```bash
   cd src/GhcSamplePs.Web
   dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"
   ```

3. **Install HTTPS Certificate**
   ```bash
   dotnet dev-certs https --trust
   ```

📖 **Complete Guide**: [Development Environment Setup](docs/Development_Environment_Setup.md)

### 4. Build and Run

```bash
# Build the solution
dotnet build

# Run the application
cd src/GhcSamplePs.Web
dotnet run
```

Open browser to: `https://localhost:5001`

## Project Structure

```
ghc-sample-ps/
├── .github/
│   ├── instructions/          # Development guidelines
│   └── agents/                # AI agent configurations
├── docs/
│   ├── specs/                 # Feature specifications
│   ├── Azure_EntraID_Setup_Guide.md
│   ├── Development_Environment_Setup.md
│   ├── Infrastructure_Verification_Checklist.md
│   ├── Azure_EntraID_Configuration_Reference.md
│   └── Authorization_Testing_Guide.md
├── src/
│   ├── GhcSamplePs.Web/      # Blazor Server application
│   └── GhcSamplePs.Core/     # Business logic library
├── tests/
│   └── GhcSamplePs.Core.Tests/  # Unit tests
├── GhcSamplePs.sln           # Solution file
├── global.json               # SDK version
└── README.md                 # This file
```

## Development Guidelines

### Code Standards

This project follows strict coding standards documented in:

- **C# Standards**: `.github/instructions/csharp.instructions.md`
- **Blazor Architecture**: `.github/instructions/blazor-architecture.instructions.md`
- **DDD & .NET Practices**: `.github/instructions/dotnet-architecture-good-practices.instructions.md`

### Key Principles

1. **Separation of Concerns**
   - UI logic in Web project
   - Business logic in Core project
   - Core never references Web

2. **Dependency Injection**
   - Use DI throughout the application
   - Register services in `Program.cs`
   - Inject via constructors or `@inject` in Blazor

3. **Async/Await**
   - Use async for all I/O operations
   - Name async methods with `Async` suffix

4. **Testing**
   - Write tests for all business logic
   - Minimum 85% code coverage for Core
   - Test naming: `MethodName_Condition_ExpectedResult`

### Branching Strategy

- `main` - Production-ready code
- `develop` - Integration branch
- `feature/*` - Feature branches
- `fix/*` - Bug fix branches

### Commit Messages

Follow conventional commits:
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `refactor:` - Code refactoring
- `test:` - Test additions or changes
- `chore:` - Maintenance tasks

## Building and Testing

### Build

```bash
# Clean build
dotnet clean
dotnet build

# Build specific project
dotnet build src/GhcSamplePs.Web/GhcSamplePs.Web.csproj
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/GhcSamplePs.Core.Tests/GhcSamplePs.Core.Tests.csproj
```

### Code Coverage

```bash
# Install dotnet-coverage tool (one-time)
dotnet tool install -g dotnet-coverage

# Generate coverage report
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

## Azure Entra ID Integration

### Authentication Features

- ✅ **Multiple Identity Providers** - Microsoft, Google, Email signup
- ✅ **Self-Service Registration** - User sign-up flows
- ✅ **Role-Based Access Control** - Admin and User roles
- ✅ **Secure Token Management** - OAuth 2.0 / OpenID Connect
- ✅ **Single Sign-On** - Seamless authentication experience

### Configuration

Authentication is configured via Azure Entra ID External Identities:

- **Tenant**: Configured in Azure Portal
- **App Registration**: OAuth client credentials
- **User Flows**: Sign-up and sign-in experiences
- **App Roles**: Admin and User roles

📖 **Configuration Guide**: [Azure Entra ID Configuration Reference](docs/Azure_EntraID_Configuration_Reference.md)

### Development vs Production

| Environment | Configuration | Secret Storage |
|-------------|---------------|----------------|
| Development | `appsettings.Development.json` | User Secrets |
| Staging | Environment variables | Azure Key Vault |
| Production | Environment variables | Azure Key Vault |

## Deployment

### Azure Container Apps

The application is designed to run in Azure Container Apps:

1. **Build Container Image**
   ```bash
   dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer
   ```

2. **Configure Environment Variables**
   - Set Azure AD configuration
   - Reference Key Vault for secrets
   - Configure managed identity

3. **Deploy to Azure**
   ```bash
   az containerapp create --name ghcsampleps \
     --resource-group rg-ghcsampleps \
     --environment env-ghcsampleps \
     --image [your-container-registry]/ghcsampleps:latest
   ```

### Environment Variables (Production)

```bash
AZUREAD__INSTANCE=https://login.microsoftonline.com/
AZUREAD__DOMAIN=your-tenant.onmicrosoft.com
AZUREAD__TENANTID=your-tenant-id
AZUREAD__CLIENTID=your-client-id
AZUREAD__CLIENTSECRET=@Microsoft.KeyVault(SecretUri=https://...)
```

## Documentation

### Setup and Configuration
- [Azure Entra ID Setup Guide](docs/Azure_EntraID_Setup_Guide.md) - Complete infrastructure setup
- [Development Environment Setup](docs/Development_Environment_Setup.md) - Local development configuration
- [Infrastructure Verification Checklist](docs/Infrastructure_Verification_Checklist.md) - Verify setup completion
- [Configuration Reference](docs/Azure_EntraID_Configuration_Reference.md) - Quick reference guide

### Feature Specifications
- [Entra ID External Identities Integration](docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
- [MudBlazor Mobile Integration](docs/specs/MudBlazor_Mobile_Integration_Specification.md)
- [Upgrade to .NET 10](docs/specs/UpgradeToDotNet10_Specification.md)

### Testing & Quality
- [Authorization Testing Guide](docs/Authorization_Testing_Guide.md) - Comprehensive authorization testing documentation

### Additional Documentation
- [Performance Guide](docs/PERFORMANCE.md)
- [PWA Implementation](docs/PWA_Implementation_Summary.md)

## Security

### Best Practices

- ✅ Never commit secrets to source control
- ✅ Use Azure Key Vault for production secrets
- ✅ Use user secrets for development
- ✅ Rotate client secrets every 6 months
- ✅ Enable HTTPS for all endpoints
- ✅ Review security logs regularly

### Secrets Management

| Secret Type | Development | Production |
|-------------|-------------|------------|
| Client Secret | User Secrets | Azure Key Vault |
| Connection Strings | User Secrets | Azure Key Vault |
| API Keys | User Secrets | Azure Key Vault |

⚠️ **NEVER** commit to source control:
- Client secrets
- Connection strings
- API keys
- User credentials
- Personal access tokens

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| "AADSTS50011: Reply URL mismatch" | Verify redirect URI in Azure app registration |
| "Invalid client secret" | Regenerate secret and update configuration |
| "Certificate not trusted" | Run `dotnet dev-certs https --trust` |
| Application won't start | Check port 5001 is not in use |
| Build errors | Clean and rebuild: `dotnet clean && dotnet build` |

### Getting Help

1. Check documentation in `docs/` folder
2. Review troubleshooting sections in setup guides
3. Check Azure Portal for configuration issues
4. Review application logs
5. Contact team for support

## Contributing

1. Create a feature branch from `develop`
2. Make changes following coding standards
3. Write tests for new functionality
4. Update documentation
5. Submit pull request

### Code Review Checklist

- [ ] Code follows project standards
- [ ] Tests written and passing
- [ ] Documentation updated
- [ ] No secrets committed
- [ ] Build succeeds
- [ ] Code coverage maintained

## Resources

### Microsoft Documentation
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Entra ID External Identities](https://learn.microsoft.com/en-us/entra/external-id/)
- [Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/)

### Project-Specific
- [Repository Instructions](.github/copilot-instructions.md)
- [C# Standards](.github/instructions/csharp.instructions.md)
- [Blazor Architecture](.github/instructions/blazor-architecture.instructions.md)

## License

[Specify your license here]

## Support

For questions or issues:
- Create an issue in the repository
- Contact the development team
- Review documentation in `docs/` folder

---

**Version**: 1.0  
**Last Updated**: 2025-11-24  
**Maintained By**: Development Team

## Quick Start Checklist

- [ ] Clone repository
- [ ] Install .NET 10 SDK
- [ ] Complete Azure Entra ID setup
- [ ] Configure development environment
- [ ] Set user secrets
- [ ] Build solution
- [ ] Run application
- [ ] Access https://localhost:5001

📖 **Start Here**: [Azure Entra ID Setup Guide](docs/Azure_EntraID_Setup_Guide.md)
