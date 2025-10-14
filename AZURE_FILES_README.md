# ?? Arquivos de Configuração Azure - Finansmart

## ? Arquivos Criados

### 1. `.github/workflows/azure-deploy.yml`
**GitHub Actions workflow para deploy automático**
- Build automático quando há push para main/master
- Executa testes antes do deploy
- Deploy automático para Azure App Service
- Requer GitHub Secret: `AZURE_WEBAPP_PUBLISH_PROFILE`

### 2. `azure-pipelines.yml`
**Azure DevOps Pipeline configuration**
- Pipeline completo com stages de Build e Deploy
- Executa testes unitários e de integração
- Publica resultados de testes e code coverage
- Deploy para Azure App Service em branch main

### 3. `azure-resources.json`
**Azure Resource Manager (ARM) Template**
- Define infraestrutura como código
- Cria App Service Plan
- Cria Web App com .NET 8.0
- Configura connection strings
- Habilita HTTPS obrigatório

### 4. `AZURE_DEPLOY_GUIDE.md`
**Guia completo de deploy**
- 3 opções de deploy (GitHub Actions, Azure DevOps, Visual Studio)
- Comandos Azure CLI passo a passo
- Configurações de segurança
- Troubleshooting
- Estimativa de custos

### 5. `deploy-azure.ps1`
**Script PowerShell para deploy automatizado**
- Cria toda infraestrutura Azure com um comando
- Configura connection strings
- Habilita logging e HTTPS
- Gera publish profile automaticamente

### 6. `appsettings.Production.json`
**Configurações de produção**
- Logging configurado para produção
- Placeholders para connection strings
- Configuração do Application Insights
- Swagger habilitado

### 7. `.gitignore` (atualizado)
**Proteção de segredos**
- Ignora arquivos de publicação
- Protege connection strings
- Ignora arquivos Azure específicos

## ?? Quick Start

### Opção 1: Script PowerShell (Mais Rápido)

```powershell
.\deploy-azure.ps1 -OracleConnectionString "sua_connection_string_aqui"
```

### Opção 2: GitHub Actions

1. Execute o script para criar infraestrutura:
```powershell
.\deploy-azure.ps1 -OracleConnectionString "sua_connection_string"
```

2. Adicione o publish profile nos GitHub Secrets:
   - Vá para: https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions
   - Adicione secret `AZURE_WEBAPP_PUBLISH_PROFILE`

3. Faça push:
```bash
git add .
git commit -m "Add Azure deployment config"
git push origin feature/config
```

### Opção 3: Manual via Portal Azure

1. Acesse: https://portal.azure.com
2. Use o template ARM: `azure-resources.json`
3. Configure connection strings manualmente
4. Deploy via Visual Studio Publish

## ?? Checklist Pré-Deploy

- [ ] Conta Azure criada
- [ ] Azure CLI instalado
- [ ] Connection string do Oracle disponível
- [ ] Código commitado no GitHub
- [ ] Testes passando localmente
- [ ] appsettings.Production.json configurado

## ?? Configurações Necessárias

### GitHub Secrets
- `AZURE_WEBAPP_PUBLISH_PROFILE` - Obtido via script ou Azure CLI

### Azure App Settings
- `ASPNETCORE_ENVIRONMENT` = Production
- `DatabaseConnection` = Sua connection string Oracle

### Azure Connection Strings
- `DatabaseConnection` = Configurada via Azure CLI ou Portal

## ?? Monitoramento

Após o deploy, monitore via:

```bash
# Ver logs em tempo real
az webapp log tail --name finansmart-app --resource-group finansmart-rg

# Ver métricas
az monitor metrics list --resource finansmart-app
```

Ou acesse o Portal Azure:
- https://portal.azure.com
- Busque por "finansmart-app"
- Veja métricas, logs e performance

## ?? URLs de Acesso

Após deploy bem-sucedido:
- **App**: https://finansmart-app.azurewebsites.net
- **Swagger**: https://finansmart-app.azurewebsites.net/swagger
- **Portal Azure**: https://portal.azure.com

## ?? Dicas

1. **Teste localmente primeiro**: `dotnet run`
2. **Execute testes**: `dotnet test`
3. **Verifique connection string**: Teste conexão Oracle
4. **Use Key Vault**: Para produção, armazene secrets no Azure Key Vault
5. **Habilite Application Insights**: Para monitoramento avançado

## ?? Problemas Comuns

### Build falha
- Verifique se .NET 8.0 SDK está instalado
- Confirme que pacages NuGet restauram corretamente

### Deploy falha
- Verifique publish profile no GitHub Secrets
- Confirme que o nome do app está correto no YAML

### Connection string não funciona
- Teste localmente primeiro
- Verifique formato da connection string
- Confirme que Oracle está acessível da Azure

## ?? Documentação

- [Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [GitHub Actions](https://docs.github.com/actions)
- [Azure DevOps](https://docs.microsoft.com/azure/devops/)
- [.NET 8 Deploy](https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/)

---

**Criado para: Finansmart Project**  
**Branch: feature/config**  
**Repository: https://github.com/Leandro-Solany/Finansmart**
