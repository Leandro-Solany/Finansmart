# ========================================
# CONFIGURAR CONNECTION STRING NO AZURE
# ========================================

Write-Host "Configurando Connection String no Azure Web App..." -ForegroundColor Yellow
Write-Host ""

# Connection String do appsettings.json
$connectionString = "Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = oracle.fiap.com.br)(PORT = 1521)))(CONNECT_DATA = (SID = orcl)));Persist Security Info=True;User ID=RM556505;Password=110191;Pooling=True;Connection Timeout=60;"

# Configurar no Azure Web App
az webapp config connection-string set `
  --name finansmart-app `
  --resource-group finansmart-rg `
  --settings DatabaseConnection="$connectionString" `
  --connection-string-type Custom

Write-Host "? Connection String configurada!" -ForegroundColor Green
Write-Host ""
Write-Host "?? INFORMAÇÃO:" -ForegroundColor Yellow
Write-Host "  • A aplicação no Azure agora pode acessar o banco da FIAP" -ForegroundColor White
Write-Host "  • Connection String: DatabaseConnection" -ForegroundColor White
Write-Host ""
Write-Host "?? IMPORTANTE:" -ForegroundColor Yellow
Write-Host "  • O banco da FIAP precisa estar acessível pela internet" -ForegroundColor White
Write-Host "  • Se houver firewall, pode não funcionar em produção" -ForegroundColor White
Write-Host ""
