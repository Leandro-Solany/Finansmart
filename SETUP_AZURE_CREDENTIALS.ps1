# ========================================
# CONFIGURAR AZURE CREDENTIALS PARA GITHUB
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CONFIGURAÇÃO DE CREDENCIAIS AZURE" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Obter Subscription ID
Write-Host "Obtendo Subscription ID..." -ForegroundColor Yellow
$subscriptionId = az account show --query id -o tsv
Write-Host "? Subscription ID: $subscriptionId" -ForegroundColor Green
Write-Host ""

# Criar Service Principal
Write-Host "Criando Service Principal..." -ForegroundColor Yellow
Write-Host "Isso pode levar alguns segundos..." -ForegroundColor Gray
Write-Host ""

$credentials = az ad sp create-for-rbac `
  --name "finansmart-github-deploy" `
  --role contributor `
  --scopes "/subscriptions/$subscriptionId/resourceGroups/finansmart-rg" `
  --sdk-auth

Write-Host "? Service Principal criado!" -ForegroundColor Green
Write-Host ""

# Salvar credenciais em arquivo
$credentials | Out-File -FilePath "azure-credentials.json" -Encoding UTF8

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PRÓXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Vou abrir o arquivo azure-credentials.json" -ForegroundColor White
Write-Host "2. Copie TODO o conteúdo (Ctrl+A, Ctrl+C)" -ForegroundColor White
Write-Host ""
Write-Host "Pressione Enter para abrir o arquivo..." -ForegroundColor Yellow
Read-Host

notepad azure-credentials.json

Write-Host ""
Write-Host "3. Agora vou abrir o GitHub Secrets" -ForegroundColor White
Write-Host "4. Clique em 'New repository secret'" -ForegroundColor White
Write-Host "5. Name: AZURE_CREDENTIALS" -ForegroundColor Green
Write-Host "6. Value: Cole o JSON (Ctrl+V)" -ForegroundColor Green
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
Write-Host "?? PRÓXIMO PASSO:" -ForegroundColor Yellow
Write-Host "  Depois de adicionar o secret AZURE_CREDENTIALS," -ForegroundColor White
Write-Host "  execute o comando:" -ForegroundColor White
Write-Host ""
Write-Host "  git add ." -ForegroundColor Gray
Write-Host "  git commit -m 'chore: Configure Azure deployment'" -ForegroundColor Gray
Write-Host "  git push origin master" -ForegroundColor Gray
Write-Host ""
