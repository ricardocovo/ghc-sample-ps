# Database Connection String Setup Guide

This guide provides step-by-step instructions for configuring database connection strings for local development and production environments in the GhcSamplePs application.

## Overview

### Purpose
Database connection strings contain sensitive information (server names, credentials) and must be managed securely. This guide covers:

- **Development**: Using .NET User Secrets to store connection strings locally
- **Staging/Production**: Using Azure Key Vault or environment variables

### Configuration Hierarchy
ASP.NET Core loads configuration from multiple sources in order:
1. `appsettings.json` (base configuration)
2. `appsettings.{Environment}.json` (environment-specific)
3. User Secrets (Development only)
4. Environment variables
5. Command-line arguments

Connection strings should **never** be committed to source control.

## Prerequisites

Before starting, ensure you have:

- ✅ **.NET 10 SDK** installed - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- ✅ **SQL Server LocalDB** or **Azure SQL Database** access
- ✅ **dotnet CLI** available (comes with .NET SDK)
- ✅ **Code editor** (Visual Studio 2022, VS Code, or JetBrains Rider)

### Verify Prerequisites

```bash
# Check .NET SDK version
dotnet --version

# Check if LocalDB is installed (Windows)
SqlLocalDB info
```

## LocalDB Setup (Recommended for Development)

SQL Server LocalDB is a lightweight version of SQL Server designed for development. It's included with Visual Studio and SQL Server Express.

### Step 1: Verify LocalDB Installation

**Windows (PowerShell or Command Prompt)**:
```powershell
# List LocalDB instances
SqlLocalDB info

# Check specific instance
SqlLocalDB info MSSQLLocalDB
```

Expected output:
```
Name:               MSSQLLocalDB
Version:            15.0.4153.1
Shared name:        
Owner:              YourDomain\YourUser
Auto-create:        Yes
State:              Running
Last start time:    [date/time]
Instance pipe name: np:\\.\pipe\LOCALDB#[...]
```

If LocalDB is not installed, see [Install LocalDB](#install-localdb-windows).

### Step 2: Initialize User Secrets

Navigate to the Web project and initialize user secrets:

```bash
cd src/GhcSamplePs.Web
dotnet user-secrets init
```

This adds a `<UserSecretsId>` element to your project file. User secrets are stored outside the project directory, ensuring they're never committed to source control.

### Step 3: Set Connection String

Store the connection string in user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### Step 4: Verify Setup

List all configured secrets to verify:

```bash
dotnet user-secrets list
```

Expected output:
```
ConnectionStrings:DefaultConnection = Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true
```

### LocalDB Connection String Format

```
Server=(localdb)\mssqllocaldb;Database={DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true
```

| Parameter | Description |
|-----------|-------------|
| `Server` | LocalDB instance name (default: `(localdb)\mssqllocaldb`) |
| `Database` | Database name to create/use |
| `Trusted_Connection` | Use Windows authentication |
| `MultipleActiveResultSets` | Enable MARS for Entity Framework |

### Install LocalDB (Windows)

If LocalDB is not installed:

**Option 1: Via SQL Server Express Installer**
1. Download [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
2. Run installer and select "LocalDB" feature
3. Restart terminal and verify installation

**Option 2: Via Visual Studio Installer**
1. Open Visual Studio Installer
2. Modify your installation
3. Select "SQL Server Express LocalDB" under "Individual components"
4. Apply changes

## Azure SQL Setup (Optional)

For teams using Azure SQL or for environments closer to production.

### Step 1: Create Azure SQL Database

**Option A: Azure Portal**
1. Go to [Azure Portal](https://portal.azure.com)
2. Create a resource > Databases > SQL Database
3. Configure:
   - **Database name**: `GhcSamplePs`
   - **Server**: Create new or select existing
   - **Compute + storage**: Choose appropriate tier (Basic/Standard for dev)
4. Review + Create

**Option B: Azure CLI**
```bash
# Create resource group
az group create --name rg-ghcsampleps --location eastus

# Create SQL server
az sql server create \
  --name sql-ghcsampleps \
  --resource-group rg-ghcsampleps \
  --location eastus \
  --admin-user sqladmin \
  --admin-password '<YourStrongPassword>'

# Create database
az sql db create \
  --resource-group rg-ghcsampleps \
  --server sql-ghcsampleps \
  --name GhcSamplePs \
  --edition Basic
```

### Step 2: Configure Firewall Rules

Allow your IP address to connect:

**Azure Portal:**
1. Navigate to your SQL server
2. Security > Networking
3. Add your client IP address
4. Save

**Azure CLI:**
```bash
# Add firewall rule for your IP
az sql server firewall-rule create \
  --resource-group rg-ghcsampleps \
  --server sql-ghcsampleps \
  --name AllowMyIP \
  --start-ip-address <your-ip> \
  --end-ip-address <your-ip>
```

### Step 3: Get Connection String from Azure Portal

1. Navigate to your SQL Database in Azure Portal
2. Settings > Connection strings
3. Copy the ADO.NET connection string
4. Replace `{your_password}` with your actual password

### Step 4: Set Azure SQL Connection String

```bash
cd src/GhcSamplePs.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:sql-ghcsampleps.database.windows.net,1433;Initial Catalog=GhcSamplePs;Persist Security Info=False;User ID=sqladmin;Password=<YourPassword>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### Azure SQL Connection String Format

```
Server=tcp:{server}.database.windows.net,1433;Initial Catalog={database};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

| Parameter | Description |
|-----------|-------------|
| `Server` | Azure SQL server FQDN with port 1433 |
| `Initial Catalog` | Database name |
| `User ID` | SQL authentication username |
| `Password` | SQL authentication password |
| `Encrypt` | Enable TLS encryption (required for Azure SQL) |
| `Connection Timeout` | Connection timeout in seconds |

### Azure AD Authentication (Recommended for Production)

For enhanced security, use Azure AD authentication instead of SQL credentials:

```
Server=tcp:{server}.database.windows.net,1433;Initial Catalog={database};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

This uses managed identity or Azure AD credentials instead of SQL username/password.

## Verify Connection

### Step 1: Build and Run Application

```bash
cd src/GhcSamplePs.Web
dotnet run
```

### Step 2: Check Startup Logs

Look for successful database connection in the logs:
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (15ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT 1
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20240101000000_InitialCreate'.
```

### Step 3: Access Application

Open browser to: `https://localhost:5001`

### Step 4: Check Health Endpoint

The application exposes a health check endpoint at `/health` that verifies database connectivity:

```bash
curl https://localhost:5001/health
```

Expected response for healthy database connection:
```
Healthy
```

## Troubleshooting

### Common Errors and Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| `Connection string 'DefaultConnection' not found` | User Secrets not configured | Run `dotnet user-secrets set` command (see Step 3) |
| `Cannot connect to LocalDB` | LocalDB not installed or not running | Install LocalDB or run `SqlLocalDB start MSSQLLocalDB` |
| `Login failed for user` | Incorrect credentials | Verify username/password in connection string |
| `A network-related or instance-specific error` | Server name incorrect or not accessible | Verify server name and network connectivity |
| `The wait operation timed out` | Network/firewall blocking connection | Check firewall rules; increase `Connection Timeout` |
| `Cannot open database` | Database doesn't exist | Run migrations with `dotnet ef database update` |
| `The certificate chain was issued by an authority that is not trusted` | SSL certificate not trusted | For LocalDB, use `TrustServerCertificate=True`; for Azure, ensure proper SSL config |
| `User secrets not loading` | Not in Development environment | Set `ASPNETCORE_ENVIRONMENT=Development` |
| `MARS not supported` | Azure SQL or connection mode issue | Set `MultipleActiveResultSets=False` for Azure SQL |

### Verify User Secrets Location

User secrets are stored at:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **Linux/macOS**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

You can find the `<user_secrets_id>` in your `.csproj` file.

### Reset User Secrets

If you need to start fresh:

```bash
cd src/GhcSamplePs.Web

# Clear all secrets
dotnet user-secrets clear

# Re-initialize and set again
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

### View Detailed Connection Errors

Enable detailed errors in development by checking `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

## Multiple Environment Setup

### Development
Use User Secrets for local development:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;..."
```

### Staging
Use environment variables or Azure Key Vault:
```bash
# Environment variable (PowerShell)
$env:ConnectionStrings__DefaultConnection = "Server=tcp:sql-staging.database.windows.net,1433;..."

# Environment variable (Linux/macOS)
export ConnectionStrings__DefaultConnection="Server=tcp:sql-staging.database.windows.net,1433;..."
```

**Note**: Use double underscores (`__`) to represent the colon (`:`) separator in environment variable names.

### Production
Use Azure Key Vault with Managed Identity:

1. Store connection string in Key Vault
2. Configure app to use Key Vault configuration provider
3. Grant managed identity access to Key Vault

```csharp
// In Program.cs (already configured in production setup)
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Configuration Priority

| Environment | Primary Source | Fallback |
|-------------|----------------|----------|
| Development | User Secrets | `appsettings.Development.json` |
| Staging | Environment Variables | Azure Key Vault |
| Production | Azure Key Vault | Environment Variables |

## Security Best Practices

### ✅ DO
- **Use User Secrets** for development - keeps credentials out of source control
- **Use Azure Key Vault** for production - centralized, audited secret management
- **Use Managed Identity** when possible - no credentials to manage
- **Use Windows Authentication** for LocalDB - no password required
- **Rotate credentials** regularly - at least every 90 days
- **Use separate databases** per environment - never share production data
- **Encrypt connections** - use `Encrypt=True` for all Azure SQL connections

### ❌ DON'T
- **Commit connection strings** to source control - even in `appsettings.json`
- **Use same credentials** across environments - isolate access
- **Share credentials** via email or chat - use secure secret sharing
- **Use weak passwords** for SQL authentication - follow password policies
- **Disable encryption** in production - always use TLS

### Secure Connection String Checklist

- [ ] Connection string stored in User Secrets (development) or Key Vault (production)
- [ ] No secrets committed to source control
- [ ] Strong password used for SQL authentication (if applicable)
- [ ] Encryption enabled (`Encrypt=True`)
- [ ] Firewall rules restrict access to known IPs
- [ ] Managed identity used where possible
- [ ] Credentials rotated on schedule

## Database Migration Commands

Once the connection is configured, use these commands to manage database migrations:

```bash
# Add a new migration
dotnet ef migrations add MigrationName \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web

# Apply migrations to database
dotnet ef database update \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web

# Remove last migration (if not applied)
dotnet ef migrations remove \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web

# Generate idempotent SQL script for production
dotnet ef migrations script \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web \
  --idempotent \
  --output migration.sql
```

## Database Schema

### Players Table

The `Players` table stores player information:

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | int | No | Primary key, auto-increment |
| UserId | nvarchar(450) | No | Owner's user identifier |
| Name | nvarchar(200) | No | Player's name |
| DateOfBirth | datetime2 | No | Player's date of birth |
| Gender | nvarchar(50) | Yes | Optional gender |
| PhotoUrl | nvarchar(500) | Yes | Optional photo URL |
| CreatedAt | datetime2 | No | Record creation timestamp |
| CreatedBy | nvarchar(450) | No | User who created record |
| UpdatedAt | datetime2 | Yes | Last update timestamp |
| UpdatedBy | nvarchar(450) | Yes | User who last updated |

**Indexes:**
- `IX_Players_UserId` - Filter by owner
- `IX_Players_Name` - Search by name
- `IX_Players_DateOfBirth` - Age-based queries
- `IX_Players_UserId_Name` - Composite index for user's player lookups

### TeamPlayers Table

The `TeamPlayers` table stores player team assignments:

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| TeamPlayerId | int | No | Primary key, auto-increment |
| PlayerId | int | No | FK to Players(Id) |
| TeamName | nvarchar(200) | No | Name of the team |
| ChampionshipName | nvarchar(200) | No | Name of the championship |
| JoinedDate | datetime2 | No | Date player joined team |
| LeftDate | datetime2 | Yes | Date player left team (null = active) |
| CreatedAt | datetime2 | No | Record creation timestamp |
| CreatedBy | nvarchar(450) | No | User who created record |
| UpdatedAt | datetime2 | Yes | Last update timestamp |
| UpdatedBy | nvarchar(450) | Yes | User who last updated |

**Indexes:**
- `IX_TeamPlayers_PlayerId` - Get player's teams
- `IX_TeamPlayers_TeamName` - Query by team name
- `IX_TeamPlayers_IsActive` - Filter active/inactive (on LeftDate)
- `IX_TeamPlayers_PlayerId_IsActive` - Composite for active player teams
- `IX_TeamPlayers_PlayerId_TeamName_ChampionshipName` - Duplicate detection

**Foreign Key:**
- `FK_TeamPlayers_Players_PlayerId` → `Players(Id)` with **CASCADE DELETE**

### Cascade Delete Behavior

When a Player is deleted:
- All associated TeamPlayer records are automatically deleted
- This maintains referential integrity
- No orphaned team assignments remain

## Migration History

### InitialCreate (20251128205245)
- Creates `Players` table with all columns
- Creates indexes for UserId, Name, DateOfBirth

### AddCompositeIndexUserIdName (20251201163057)
- Adds composite index `IX_Players_UserId_Name`

### AddTeamPlayersTable (20251201203656)
- Creates `TeamPlayers` table with all columns
- Creates FK to Players with cascade delete
- Creates all indexes listed above

### Rollback Procedure

To rollback the TeamPlayers migration:

```bash
# Rollback to before AddTeamPlayersTable
dotnet ef database update AddCompositeIndexUserIdName \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web

# Remove migration if needed
dotnet ef migrations remove \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web
```

**Warning:** Rolling back will delete all data in the TeamPlayers table.

## Quick Reference

### LocalDB Connection String (Copy-Paste Ready)
```
Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true
```

### Set Connection String Command
```bash
cd src/GhcSamplePs.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### Verify Connection String
```bash
dotnet user-secrets list
```

## Related Documentation

- [Development Environment Setup](Development_Environment_Setup.md) - Complete development setup
- [Azure Entra ID Setup Guide](Azure_EntraID_Setup_Guide.md) - Authentication configuration
- [GhcSamplePs.Core README](../src/GhcSamplePs.Core/README.md) - Entity Framework and DbContext documentation
- [GhcSamplePs.Web README](../src/GhcSamplePs.Web/README.md) - Web application configuration

## Resources

### Microsoft Documentation
- [Safe Storage of App Secrets in Development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Entity Framework Core Connection Strings](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-strings)
- [Azure SQL Documentation](https://learn.microsoft.com/en-us/azure/azure-sql/)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

---

**Document Version**: 1.0  
**Last Updated**: 2025-12-01  
**Maintained By**: Development Team  
**Review Schedule**: When database configuration changes
