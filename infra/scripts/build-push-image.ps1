param(
    [Parameter(Mandatory=$true)]
    [string]$RegistryName,
    
    [Parameter(Mandatory=$false)]
    [string]$ImageTag = "latest"
)

$imageName = "ghcsampleps-web"

# Build Docker image
Write-Host "Building Docker image..." -ForegroundColor Green
docker build -t "$imageName:$ImageTag" -f ./src/GhcSamplePs.Web/Dockerfile .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker build failed!" -ForegroundColor Red
    exit 1
}

# Login to Azure Container Registry
Write-Host "Logging into Azure Container Registry..." -ForegroundColor Cyan
az acr login --name $RegistryName

# Tag image for ACR
$acrLoginServer = "$RegistryName.azurecr.io"
docker tag "$imageName:$ImageTag" "$acrLoginServer/$imageName:$ImageTag"

# Push to ACR
Write-Host "Pushing image to ACR..." -ForegroundColor Green
docker push "$acrLoginServer/$imageName:$ImageTag"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nImage pushed successfully!" -ForegroundColor Green
    Write-Host "Image: $acrLoginServer/$imageName:$ImageTag" -ForegroundColor Cyan
}
