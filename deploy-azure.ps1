# Script de Deploy Automatizado para Azure - Finansmart
# Execute este script no PowerShell com privilégios de administrador

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupName = "finansmart-rg",
    
    [Parameter(Mandatory=$false)]
    [string]$AppServicePlanName = "finansmart-plan",
    
    [Parameter(Mandatory=$false)]
    [string]$WebAppName = "finansmart-app",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "brazilsouth",
    
    [Parameter(Mandatory=$false)]
    [string]$Sku = "B1",
    
    [Parameter(Mandatory=$true)]
    [string]$OracleConnectionString
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Deploy Finansmart para Azure App Service" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se Azure CLI está instalado
Write-Host "Verificando Azure CLI..." -ForegroundColor Yellow
$azCli = Get-Command az -ErrorAction SilentlyContinue
if (-not $azCli) {
    Write-Host "Azure CLI não encontrado! Instale de: https://docs.microsoft.com/cli/azure/install-azure-cli" -ForegroundColor Red
    exit 1
}
Write-Host "? Azure CLI encontrado" -ForegroundColor Green

# Login no Azure
Write-Host ""
Write-Host "Fazendo login no Azure..." -ForegroundColor Yellow
az login
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao fazer login no Azure!" -ForegroundColor Red
    exit 1
}
Write-Host "? Login realizado com sucesso" -ForegroundColor Green

# Criar Resource Group
Write-Host ""
Write-Host "Criando Resource Group: $ResourceGroupName..." -ForegroundColor Yellow
az group create --name $ResourceGroupName --location $Location
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao criar Resource Group!" -ForegroundColor Red
    exit 1
}
Write-Host "? Resource Group criado" -ForegroundColor Green

# Criar App Service Plan
Write-Host ""
Write-Host "Criando App Service Plan: $AppServicePlanName..." -ForegroundColor Yellow
az appservice plan create `
    --name $AppServicePlanName `
    --resource-group $ResourceGroupName `
    --sku $Sku `
    --is-linux
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao criar App Service Plan!" -ForegroundColor Red
    exit 1
}
Write-Host "? App Service Plan criado" -ForegroundColor Green

# Criar Web App
Write-Host ""
Write-Host "Criando Web App: $WebAppName..." -ForegroundColor Yellow
az webapp create `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --plan $AppServicePlanName `
    --runtime "DOTNET|8.0"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao criar Web App!" -ForegroundColor Red
    exit 1
}
Write-Host "? Web App criado" -ForegroundColor Green

# Configurar Connection String
Write-Host ""
Write-Host "Configurando Connection String..." -ForegroundColor Yellow
az webapp config connection-string set `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --settings DatabaseConnection="$OracleConnectionString" `
    --connection-string-type Custom
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao configurar Connection String!" -ForegroundColor Red
    exit 1
}
Write-Host "? Connection String configurada" -ForegroundColor Green

# Configurar HTTPS Only
Write-Host ""
Write-Host "Habilitando HTTPS obrigatório..." -ForegroundColor Yellow
az webapp update `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --set httpsOnly=true
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao configurar HTTPS!" -ForegroundColor Red
    exit 1
}
Write-Host "? HTTPS obrigatório habilitado" -ForegroundColor Green

# Habilitar Logging
Write-Host ""
Write-Host "Habilitando logging..." -ForegroundColor Yellow
az webapp log config `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --application-logging filesystem `
    --detailed-error-messages true `
    --failed-request-tracing true `
    --web-server-logging filesystem
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao habilitar logging!" -ForegroundColor Red
    exit 1
}
Write-Host "? Logging habilitado" -ForegroundColor Green

# Obter URL do Web App
Write-Host ""
Write-Host "Obtendo informações do Web App..." -ForegroundColor Yellow
$webAppUrl = az webapp show `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --query "defaultHostName" `
    --output tsv

# Obter Publish Profile
Write-Host ""
Write-Host "Gerando Publish Profile..." -ForegroundColor Yellow
$publishProfile = az webapp deployment list-publishing-profiles `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --xml

# Salvar Publish Profile em arquivo
$publishProfilePath = "publish-profile.xml"
$publishProfile | Out-File -FilePath $publishProfilePath -Encoding utf8
Write-Host "? Publish Profile salvo em: $publishProfilePath" -ForegroundColor Green

# Resumo
Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Deploy Concluído com Sucesso!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Informações do Deploy:" -ForegroundColor Yellow
Write-Host "  Resource Group: $ResourceGroupName"
Write-Host "  App Service Plan: $AppServicePlanName"
Write-Host "  Web App: $WebAppName"
Write-Host "  URL: https://$webAppUrl"
Write-Host "  Swagger: https://$webAppUrl/swagger"
Write-Host ""
Write-Host "Próximos Passos:" -ForegroundColor Yellow
Write-Host "  1. Adicione o Publish Profile ao GitHub Secrets:"
Write-Host "     - Vá para: https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"
Write-Host "     - Nome: AZURE_WEBAPP_PUBLISH_PROFILE"
Write-Host "     - Valor: Conteúdo do arquivo $publishProfilePath"
Write-Host ""
Write-Host "  2. Atualize o nome do Web App no arquivo .github/workflows/azure-deploy.yml:"
Write-Host "     - AZURE_WEBAPP_NAME: $WebAppName"
Write-Host ""
Write-Host "  3. Faça push do código para o GitHub:"
Write-Host "     git add ."
Write-Host "     git commit -m 'Add Azure deployment'"
Write-Host "     git push origin feature/config"
Write-Host ""
Write-Host "  4. Monitore os logs:"
Write-Host "     az webapp log tail --name $WebAppName --resource-group $ResourceGroupName"
Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
