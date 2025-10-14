# ?? Guia de Deploy no Azure - Finansmart

## ?? Pré-requisitos

1. **Conta Azure** - Criar em [https://azure.microsoft.com/free/](https://azure.microsoft.com/free/)
2. **Azure CLI** - Instalar de [https://docs.microsoft.com/cli/azure/install-azure-cli](https://docs.microsoft.com/cli/azure/install-azure-cli)
3. **Git** - Para versionamento de código
4. **Repositório GitHub** - Seu código já está em: https://github.com/Leandro-Solany/Finansmart

## ?? Opções de Deploy

### Opção 1: GitHub Actions (Recomendado para GitHub)

#### Passo 1: Criar Azure App Service

```bash
# Login no Azure
az login

# Criar Resource Group
az group create --name finansmart-rg --location brazilsouth

# Criar App Service Plan (B1 = Basic, custo baixo)
az appservice plan create \
  --name finansmart-plan \
  --resource-group finansmart-rg \
  --sku B1 \
  --is-linux

# Criar Web App
az webapp create \
  --name finansmart-app \
  --resource-group finansmart-rg \
  --plan finansmart-plan \
  --runtime "DOTNET|8.0"
```

#### Passo 2: Configurar Connection String do Oracle

```bash
az webapp config connection-string set \
  --name finansmart-app \
  --resource-group finansmart-rg \
  --settings DatabaseConnection="SUA_CONNECTION_STRING_ORACLE" \
  --connection-string-type Custom
```

#### Passo 3: Obter Publish Profile

```bash
az webapp deployment list-publishing-profiles \
  --name finansmart-app \
  --resource-group finansmart-rg \
  --xml
```

#### Passo 4: Configurar GitHub Secrets

1. Vá para: https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions
2. Clique em "New repository secret"
3. Adicione:
   - **Nome**: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - **Valor**: Cole o XML do publish profile do passo anterior

#### Passo 5: Fazer Push e Deploy

```bash
git add .
git commit -m "Add Azure deployment configuration"
git push origin feature/config
```

O GitHub Actions vai automaticamente fazer o build e deploy! ??

---

### Opção 2: Azure DevOps Pipelines

#### Passo 1: Criar Service Connection

1. Acesse: https://dev.azure.com
2. Crie um novo projeto
3. Vá em **Project Settings** > **Service connections**
4. Clique em **New service connection** > **Azure Resource Manager**
5. Escolha **Service principal (automatic)**
6. Selecione sua subscription e resource group
7. Nomeie como: `Finansmart-Azure-Connection`

#### Passo 2: Criar Pipeline

1. Vá em **Pipelines** > **Create Pipeline**
2. Selecione **GitHub**
3. Autorize o Azure DevOps a acessar seu repositório
4. Selecione **Existing Azure Pipelines YAML file**
5. Escolha o arquivo `azure-pipelines.yml`
6. Clique em **Run**

---

### Opção 3: Deploy Manual via Visual Studio

#### Passo 1: Instalar Azure Workload

1. Abra Visual Studio Installer
2. Modifique sua instalação
3. Marque **Azure development**
4. Clique em **Modify**

#### Passo 2: Publicar via Visual Studio

1. No Visual Studio, clique com o botão direito no projeto **Finansmart**
2. Selecione **Publish**
3. Escolha **Azure**
4. Selecione **Azure App Service (Linux)**
5. Faça login na sua conta Azure
6. Selecione ou crie um novo App Service
7. Clique em **Finish** e depois **Publish**

---

## ?? Configurações Importantes

### 1. Configurar HTTPS Only

```bash
az webapp update \
  --name finansmart-app \
  --resource-group finansmart-rg \
  --set httpsOnly=true
```

### 2. Configurar Custom Domain (Opcional)

```bash
az webapp config hostname add \
  --webapp-name finansmart-app \
  --resource-group finansmart-rg \
  --hostname www.seudominio.com.br
```

### 3. Habilitar Application Insights (Monitoramento)

```bash
az monitor app-insights component create \
  --app finansmart-insights \
  --location brazilsouth \
  --resource-group finansmart-rg \
  --application-type web

# Obter Instrumentation Key
az monitor app-insights component show \
  --app finansmart-insights \
  --resource-group finansmart-rg \
  --query instrumentationKey
```

Adicione ao `appsettings.json`:
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "SUA_KEY_AQUI"
  }
}
```

### 4. Configurar Swagger em Produção (Opcional)

Edite `Program.cs` para habilitar Swagger em produção:

```csharp
// Remova ou comente esta linha:
// if (!app.Environment.IsDevelopment())

// Mantenha Swagger sempre ativo:
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finansmart API v1");
});
```

---

## ?? Monitoramento e Logs

### Ver Logs em Tempo Real

```bash
az webapp log tail \
  --name finansmart-app \
  --resource-group finansmart-rg
```

### Habilitar Logging

```bash
az webapp log config \
  --name finansmart-app \
  --resource-group finansmart-rg \
  --application-logging filesystem \
  --detailed-error-messages true \
  --failed-request-tracing true \
  --web-server-logging filesystem
```

### Acessar via Portal Azure

1. Acesse: https://portal.azure.com
2. Procure por `finansmart-app`
3. Vá em **Monitoring** > **Log stream**

---

## ?? Segurança

### 1. Usar Azure Key Vault para Secrets

```bash
# Criar Key Vault
az keyvault create \
  --name finansmart-vault \
  --resource-group finansmart-rg \
  --location brazilsouth

# Adicionar Connection String
az keyvault secret set \
  --vault-name finansmart-vault \
  --name "DatabaseConnection" \
  --value "SUA_CONNECTION_STRING"

# Dar permissão ao App Service
az webapp identity assign \
  --name finansmart-app \
  --resource-group finansmart-rg

# Configurar referência no App Settings
az webapp config appsettings set \
  --name finansmart-app \
  --resource-group finansmart-rg \
  --settings DatabaseConnection="@Microsoft.KeyVault(SecretUri=https://finansmart-vault.vault.azure.net/secrets/DatabaseConnection/)"
```

---

## ?? Estimativa de Custos

### App Service Plan B1 (Basic)
- **Preço**: ~R$ 55/mês
- **Recursos**: 1.75 GB RAM, 100 GB storage
- **Indicado para**: Desenvolvimento/Testes

### App Service Plan S1 (Standard)
- **Preço**: ~R$ 300/mês
- **Recursos**: 1.75 GB RAM, 50 GB storage, Custom domains, SSL
- **Indicado para**: Produção

### Oracle Database
- **Azure Oracle**: Varia conforme uso
- **Oracle Cloud**: Free tier disponível com 2 OCPU

---

## ?? Troubleshooting

### Erro: "Application Error"
```bash
# Ver logs
az webapp log tail --name finansmart-app --resource-group finansmart-rg

# Verificar configurações
az webapp config show --name finansmart-app --resource-group finansmart-rg
```

### Erro: Connection String não funciona
```bash
# Testar connection string
az webapp config connection-string list \
  --name finansmart-app \
  --resource-group finansmart-rg
```

### Erro: Build falha no pipeline
- Verifique se o .NET 8.0 SDK está configurado
- Confirme que todos os pacotes NuGet estão restaurando corretamente
- Rode `dotnet build` localmente para verificar erros

---

## ?? Suporte

- **Documentação Azure**: https://docs.microsoft.com/azure/app-service/
- **Fórum Azure**: https://docs.microsoft.com/answers/products/azure
- **Stack Overflow**: Tag `azure-app-service`

---

## ? Checklist de Deploy

- [ ] Criar Azure App Service
- [ ] Configurar Connection String do Oracle
- [ ] Configurar GitHub Secrets (se usar GitHub Actions)
- [ ] Fazer push do código
- [ ] Verificar logs após deploy
- [ ] Testar endpoints da API
- [ ] Configurar domínio customizado (opcional)
- [ ] Habilitar Application Insights
- [ ] Configurar backup automático
- [ ] Testar rollback em caso de erro

---

**Boa sorte com seu deploy! ??**
