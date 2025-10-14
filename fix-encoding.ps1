# Script para corrigir encoding de arquivos
# Execute no PowerShell: .\fix-encoding.ps1

Write-Host "Corrigindo encoding de arquivos markdown..." -ForegroundColor Yellow

$files = @(
    "Finansmart.Tests\IntegrationTests\README.md",
    "AZURE_DEPLOY_GUIDE.md"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "Processando: $file" -ForegroundColor Cyan
        
        # Ler com encoding correto
        $content = Get-Content $file -Encoding UTF8 -Raw
        
        # Salvar com BOM UTF-8
        $utf8 = New-Object System.Text.UTF8Encoding $true
        [System.IO.File]::WriteAllText($file, $content, $utf8)
        
        Write-Host "  ? Corrigido" -ForegroundColor Green
    }
    else {
        Write-Host "  ? Arquivo não encontrado: $file" -ForegroundColor Red
    }
}

Write-Host "`nConcluído!" -ForegroundColor Green
