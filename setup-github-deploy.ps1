# ?? Script Automático: Configurar Azure Deploy no GitHub

# PASSO 1: Verificar se Azure CLI está instalado
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Setup Azure Deploy - GitHub Actions" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/7] Verificando Azure CLI..." -ForegroundColor Yellow
$azCli = Get-Command az -ErrorAction SilentlyContinue
if (-not $azCli) {
    Write-Host "? Azure CLI não encontrado!" -ForegroundColor Red
    Write-Host "?? Baixe em: https://aka.ms/installazurecliwindows" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Após instalar, execute este script novamente." -ForegroundColor Yellow
    pause
    exit 1
}
Write-Host "? Azure CLI encontrado" -ForegroundColor Green
Write-Host ""

# PASSO 2: Login no Azure
Write-Host "[2/7] Fazendo login no Azure..." -ForegroundColor Yellow
try {
    az account show 2>$null | Out-Null
    Write-Host "? Já está logado no Azure" -ForegroundColor Green
}
catch {
    Write-Host "?? Abrindo janela de login..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Erro ao fazer login!" -ForegroundColor Red
        pause
        exit 1
    }
    Write-Host "? Login realizado com sucesso" -ForegroundColor Green
}
Write-Host ""

# PASSO 3: Verificar/Criar Resource Group
Write-Host "[3/7] Verificando Resource Group..." -ForegroundColor Yellow
$rgExists = az group exists --name finansmart-rg
if ($rgExists -eq "false") {
    Write-Host "?? Criando Resource Group 'finansmart-rg'..." -ForegroundColor Yellow
    az group create --name finansmart-rg --location brazilsouth
    Write-Host "? Resource Group criado" -ForegroundColor Green
}
else {
    Write-Host "? Resource Group 'finansmart-rg' já existe" -ForegroundColor Green
}
Write-Host ""

# PASSO 4: Verificar/Criar App Service Plan
Write-Host "[4/7] Verificando App Service Plan..." -ForegroundColor Yellow
$planExists = az appservice plan show --name finansmart-plan --resource-group finansmart-rg 2>$null
if (-not $planExists) {
    Write-Host "?? Criando App Service Plan 'finansmart-plan'..." -ForegroundColor Yellow
    az appservice plan create `
        --name finansmart-plan `
        --resource-group finansmart-rg `
        --sku B1 `
        --is-linux
    Write-Host "? App Service Plan criado" -ForegroundColor Green
}
else {
    Write-Host "? App Service Plan 'finansmart-plan' já existe" -ForegroundColor Green
}
Write-Host ""

# PASSO 5: Verificar/Criar Web App
Write-Host "[5/7] Verificando Web App..." -ForegroundColor Yellow
$appExists = az webapp show --name finansmart-app --resource-group finansmart-rg 2>$null
if (-not $appExists) {
    Write-Host "?? Criando Web App 'finansmart-app'..." -ForegroundColor Yellow
    az webapp create `
        --name finansmart-app `
        --resource-group finansmart-rg `
        --plan finansmart-plan `
        --runtime "DOTNET|8.0"
    Write-Host "? Web App criado" -ForegroundColor Green
}
else {
    Write-Host "? Web App 'finansmart-app' já existe" -ForegroundColor Green
}
Write-Host ""

# PASSO 6: Obter Publish Profile
Write-Host "[6/7] Obtendo Publish Profile..." -ForegroundColor Yellow
$publishProfile = az webapp deployment list-publishing-profiles `
    --name finansmart-app `
    --resource-group finansmart-rg `
    --xml

if ($publishProfile) {
    $publishProfilePath = "publish-profile.xml"
    $publishProfile | Out-File -FilePath $publishProfilePath -Encoding UTF8
    Write-Host "? Publish Profile salvo em: $publishProfilePath" -ForegroundColor Green
}
else {
    Write-Host "? Erro ao obter Publish Profile" -ForegroundColor Red
    pause
    exit 1
}
Write-Host ""

# PASSO 7: Instruções para GitHub
Write-Host "[7/7] Configuração do GitHub Secret" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? PRÓXIMOS PASSOS MANUAIS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1?? Abra o arquivo: $publishProfilePath" -ForegroundColor White
Write-Host "   - Execute: notepad $publishProfilePath" -ForegroundColor Gray
Write-Host ""
Write-Host "2?? Copie TODO o conteúdo do arquivo (Ctrl+A, Ctrl+C)" -ForegroundColor White
Write-Host ""
Write-Host "3?? Acesse o GitHub:" -ForegroundColor White
Write-Host "   https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions" -ForegroundColor Cyan
Write-Host ""
Write-Host "4?? Clique em 'New repository secret'" -ForegroundColor White
Write-Host ""
Write-Host "5?? Preencha:" -ForegroundColor White
Write-Host "   Name: AZURE_WEBAPP_PUBLISH_PROFILE" -ForegroundColor Green
Write-Host "   Value: [Cole o conteúdo XML completo]" -ForegroundColor Green
Write-Host ""
Write-Host "6?? Clique em 'Add secret'" -ForegroundColor White
Write-Host ""
Write-Host "7?? Faça push para testar:" -ForegroundColor White
Write-Host "   git add ." -ForegroundColor Gray
Write-Host "   git commit -m 'chore: Configure Azure deployment'" -ForegroundColor Gray
Write-Host "   git push origin main" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Setup Azure Completo!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? INFORMAÇÕES DO DEPLOY:" -ForegroundColor Yellow
Write-Host "  Resource Group: finansmart-rg" -ForegroundColor White
Write-Host "  App Service Plan: finansmart-plan (B1)" -ForegroundColor White
Write-Host "  Web App: finansmart-app" -ForegroundColor White
Write-Host "  URL: https://finansmart-app.azurewebsites.net" -ForegroundColor Cyan
Write-Host "  Swagger: https://finansmart-app.azurewebsites.net/swagger" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? SEGURANÇA:" -ForegroundColor Yellow
Write-Host "  ?? NÃO COMMITE o arquivo publish-profile.xml!" -ForegroundColor Red
Write-Host "  ? Ele já está no .gitignore" -ForegroundColor Green
Write-Host ""

Write-Host "? Deseja abrir o arquivo publish-profile.xml agora? (S/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq 'S' -or $response -eq 's') {
    notepad $publishProfilePath
}

Write-Host ""
Write-Host "? Deseja abrir a página de GitHub Secrets agora? (S/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq 'S' -or $response -eq 's') {
    Start-Process "https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"
}

Write-Host ""
Write-Host "Pressione qualquer tecla para finalizar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
