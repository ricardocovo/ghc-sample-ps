param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "canadacentral",
    
    [Parameter(Mandatory=$false)]
    [string]$ParameterFile = "./infra/main.bicepparam"
)

# Login check
$context = Get-AzContext
if (-not $context) {
    Write-Host "Please login to Azure..." -ForegroundColor Yellow
    Connect-AzAccount
}

# Create resource group if not exists
$rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (-not $rg) {
    Write-Host "Creating resource group: $ResourceGroupName" -ForegroundColor Green
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location
}

# Validate deployment
Write-Host "Validating Bicep deployment..." -ForegroundColor Cyan
$validation = Test-AzResourceGroupDeployment `
    -ResourceGroupName $ResourceGroupName `
    -TemplateParameterFile $ParameterFile `
    -TemplateFile "./infra/main.bicep"

if ($validation) {
    Write-Host "Validation failed:" -ForegroundColor Red
    $validation | Format-List
    exit 1
}

# Deploy infrastructure
Write-Host "Deploying infrastructure to Azure..." -ForegroundColor Green
$deployment = New-AzResourceGroupDeployment `
    -Name "ghcsampleps-$(Get-Date -Format 'yyyyMMddHHmmss')" `
    -ResourceGroupName $ResourceGroupName `
    -TemplateParameterFile $ParameterFile `
    -TemplateFile "./infra/main.bicep" `
    -Verbose

if ($deployment.ProvisioningState -eq 'Succeeded') {
    Write-Host "`nDeployment successful!" -ForegroundColor Green
    Write-Host "Container App URL: $($deployment.Outputs.containerAppUrl.Value)" -ForegroundColor Cyan
    Write-Host "SQL Server: $($deployment.Outputs.sqlServerFqdn.Value)" -ForegroundColor Cyan
} else {
    Write-Host "Deployment failed!" -ForegroundColor Red
    exit 1
}
