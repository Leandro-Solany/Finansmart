# ?? CHECKLIST: Resolver Erro "No credentials found"

## ? Erro
```
Error: Deployment failed, Error: No credentials found.
```

## ? Verificações

### [ ] 1. Secret Existe no GitHub?
- Acesse: https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions
- Deve ter: `AZURE_WEBAPP_PUBLISH_PROFILE`

### [ ] 2. Nome do Secret Está EXATO?
- ? CORRETO: `AZURE_WEBAPP_PUBLISH_PROFILE`
- ? ERRADO: Qualquer outra variação

### [ ] 3. XML Está Completo?
O arquivo `publish-profile-NEW.xml` deve ter:
```xml
<?xml version="1.0" encoding="utf-8"?>
<publishData>
  <publishProfile ...>
    ...
  </publishProfile>
</publishData>
```

### [ ] 4. Copiou TODO o Conteúdo?
- Incluindo primeira linha: `<?xml version...`
- Incluindo última linha: `</publishData>`

### [ ] 5. App Service Está Rodando?
```powershell
az webapp show --name finansmart-app --resource-group finansmart-app_group --query "state"
```
Deve retornar: `"Running"`

---

## ?? Solução Rápida

Execute no PowerShell:

```powershell
# 1. Reobter publish profile
az webapp deployment list-publishing-profiles --name finansmart-app --resource-group finansmart-app_group --xml | Out-File -FilePath publish-profile-FINAL.xml -Encoding UTF8

# 2. Abrir arquivo
notepad publish-profile-FINAL.xml

# 3. Copiar: Ctrl+A, Ctrl+C

# 4. Abrir GitHub
Start-Process "https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"

# 5. Adicionar/Update secret
#    Name: AZURE_WEBAPP_PUBLISH_PROFILE
#    Value: [Colar XML]

# 6. Testar workflow
Start-Process "https://github.com/Leandro-Solany/Finansmart/actions/workflows/azure-deploy.yml"
```

---

## ?? Status

- [ ] Publish profile reobti do
- [ ] XML copiado completamente
- [ ] Secret adicionado/atualizado no GitHub
- [ ] Nome conferido: `AZURE_WEBAPP_PUBLISH_PROFILE`
- [ ] Workflow executado novamente
- [ ] Deploy bem-sucedido

---

## ?? Se Ainda Não Funcionar

Tente usar Azure Login em vez de Publish Profile:

### Criar Service Principal:
```powershell
az ad sp create-for-rbac --name "finansmart-github" --role contributor --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/finansmart-app_group --sdk-auth
```

### Adicionar como Secret:
- Name: `AZURE_CREDENTIALS`
- Value: [JSON retornado]

### Atualizar workflow:
Usar `azure/login@v1` em vez de `publish-profile`

---

**Execute a solução rápida acima!** ??
