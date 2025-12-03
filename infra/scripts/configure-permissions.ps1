param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$ContainerAppName,
    
    [Parameter(Mandatory=$true)]
    [string]$SqlServerName,
    
    [Parameter(Mandatory=$true)]
    [string]$DatabaseName
)

# Get Container App Managed Identity
$app = Get-AzContainerApp -ResourceGroupName $ResourceGroupName -Name $ContainerAppName
$identityId = $app.IdentityPrincipalId

Write-Host "Container App Identity: $identityId" -ForegroundColor Cyan

# SQL Database permissions (must be done via SQL commands)
Write-Host "`nConfiguring SQL Database permissions..." -ForegroundColor Yellow
Write-Host "Run these SQL commands as the Entra ID admin:" -ForegroundColor Cyan
Write-Host @"

-- Connect to $DatabaseName database
CREATE USER [$ContainerAppName] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [$ContainerAppName];
ALTER ROLE db_datawriter ADD MEMBER [$ContainerAppName];
ALTER ROLE db_ddladmin ADD MEMBER [$ContainerAppName];
GO

"@ -ForegroundColor White
