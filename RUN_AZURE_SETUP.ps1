# ========================================
# COMANDOS AZURE - Execute linha por linha
# ========================================

# 1. VERIFICAR INSTALAÇÃO
Write-Host "Verificando Azure CLI..." -ForegroundColor Yellow
az --version
Write-Host ""

# 2. LOGIN NO AZURE
Write-Host "Fazendo login no Azure..." -ForegroundColor Yellow
Write-Host "Uma janela do navegador vai abrir. Faça login e volte aqui." -ForegroundColor Cyan
az login
Write-Host "? Login realizado!" -ForegroundColor Green
Write-Host ""

# 3. CRIAR RESOURCE GROUP
Write-Host "Criando Resource Group..." -ForegroundColor Yellow
az group create --name finansmart-rg --location brazilsouth
Write-Host "? Resource Group criado!" -ForegroundColor Green
Write-Host ""

# 4. CRIAR APP SERVICE PLAN
Write-Host "Criando App Service Plan..." -ForegroundColor Yellow
az appservice plan create `
  --name finansmart-plan `
  --resource-group finansmart-rg `
  --sku B1 `
  --is-linux
Write-Host "? App Service Plan criado!" -ForegroundColor Green
Write-Host ""

# 5. CRIAR WEB APP
Write-Host "Criando Web App..." -ForegroundColor Yellow
az webapp create `
  --name finansmart-app `
  --resource-group finansmart-rg `
  --plan finansmart-plan `
  --runtime "DOTNET|8.0"
Write-Host "? Web App criado!" -ForegroundColor Green
Write-Host ""

# 6. OBTER PUBLISH PROFILE
Write-Host "Obtendo Publish Profile..." -ForegroundColor Yellow
az webapp deployment list-publishing-profiles `
  --name finansmart-app `
  --resource-group finansmart-rg `
  --xml | Out-File -FilePath publish-profile.xml -Encoding UTF8
Write-Host "? Publish Profile salvo em: publish-profile.xml" -ForegroundColor Green
Write-Host ""

# 7. ABRIR ARQUIVOS
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PRÓXIMOS PASSOS MANUAIS:" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Vou abrir o arquivo publish-profile.xml" -ForegroundColor White
Write-Host "2. Copie TODO o conteúdo (Ctrl+A, Ctrl+C)" -ForegroundColor White
Write-Host ""
Write-Host "Pressione Enter para abrir o arquivo..." -ForegroundColor Yellow
Read-Host

notepad publish-profile.xml

Write-Host ""
Write-Host "3. Agora vou abrir o GitHub Secrets" -ForegroundColor White
Write-Host "4. Clique em 'New repository secret'" -ForegroundColor White
Write-Host "5. Name: AZURE_WEBAPP_PUBLISH_PROFILE" -ForegroundColor Green
Write-Host "6. Value: Cole o XML (Ctrl+V)" -ForegroundColor Green
Write-Host "7. Clique em 'Add secret'" -ForegroundColor White
Write-Host ""
Write-Host "Pressione Enter para abrir o GitHub..." -ForegroundColor Yellow
Read-Host

Start-Process "https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? SETUP CONCLUÍDO!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? INFORMAÇÕES:" -ForegroundColor Yellow
Write-Host "  • Resource Group: finansmart-rg" -ForegroundColor White
Write-Host "  • App Service Plan: finansmart-plan" -ForegroundColor White
Write-Host "  • Web App: finansmart-app" -ForegroundColor White
Write-Host "  • URL: https://finansmart-app.azurewebsites.net" -ForegroundColor Cyan
Write-Host "  • Swagger: https://finansmart-app.azurewebsites.net/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? DEPOIS DE ADICIONAR O SECRET:" -ForegroundColor Yellow
Write-Host "  git add ." -ForegroundColor Gray
Write-Host "  git commit -m 'chore: Configure Azure deployment'" -ForegroundColor Gray
Write-Host "  git push origin master" -ForegroundColor Gray
Write-Host ""
