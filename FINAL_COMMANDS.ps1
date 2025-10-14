# Execute estes comandos um por vez no PowerShell:

# 1. Criar Web App
az webapp create `
  --name finansmart-app `
  --resource-group finansmart-rg `
  --plan finansmart-plan `
  --runtime 'DOTNETCORE:8.0'

# 2. Obter Publish Profile
az webapp deployment list-publishing-profiles `
  --name finansmart-app `
  --resource-group finansmart-rg `
  --xml | Out-File -FilePath publish-profile.xml -Encoding UTF8

# 3. Abrir arquivo
notepad publish-profile.xml

# 4. Copiar conteúdo (Ctrl+A, Ctrl+C)

# 5. Abrir GitHub Secrets
Start-Process "https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"

# 6. Adicionar secret
# Name: AZURE_WEBAPP_PUBLISH_PROFILE
# Value: [Colar XML]

# 7. Fazer push
git add .
git commit -m "chore: Configure Azure deployment"
git push origin master
