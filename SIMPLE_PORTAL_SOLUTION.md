# ?? SOLUÇÃO SIMPLES - VIA PORTAL AZURE

## ?? Problemas com az login?

Use o Portal Azure diretamente! É mais fácil e rápido!

---

## ? PASSO 1: Obter Publish Profile via Portal

### 1.1 - Abra o Portal Azure
?? https://portal.azure.com

### 1.2 - Navegue até o App Service
1. No menu esquerdo, clique em **"App Services"**
2. Procure e clique em **"finansmart-app"**
   - Se não existir, você precisa criar primeiro!

### 1.3 - Baixar Publish Profile
1. No topo da página, clique em **"Get publish profile"** ou **"Obter perfil de publicação"**
2. Um arquivo `.PublishSettings` será baixado automaticamente

### 1.4 - Abrir o Arquivo
1. Vá para a pasta **Downloads**
2. Encontre o arquivo: `finansmart-app.PublishSettings`
3. Abra com **Notepad**
4. Pressione **Ctrl+A** (selecionar tudo)
5. Pressione **Ctrl+C** (copiar)

---

## ? PASSO 2: Adicionar ao GitHub Secrets

### 2.1 - Abrir GitHub Secrets
?? https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions

### 2.2 - Criar Secret
1. Clique em **"New repository secret"**
2. **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
3. **Secret**: Pressione **Ctrl+V** (colar o XML)
4. Clique em **"Add secret"**

---

## ? PASSO 3: Fazer Push

No PowerShell, execute:

```powershell
git add .
git commit -m "chore: Configure Azure deployment credentials"
git push origin master
```

---

## ? PASSO 4: Verificar Deploy

Vá para:
?? https://github.com/Leandro-Solany/Finansmart/actions

O workflow vai executar automaticamente e o deploy será feito!

---

## ?? Se o App Service Não Existir

Execute no PowerShell (um comando por vez):

```powershell
# Login via browser (mais fácil)
az login

# Criar Resource Group
az group create --name finansmart-rg --location brazilsouth

# Criar App Service Plan
az appservice plan create --name finansmart-plan --resource-group finansmart-rg --sku B1 --is-linux

# Criar Web App
az webapp create --name finansmart-app --resource-group finansmart-rg --plan finansmart-plan --runtime "DOTNET|8.0"
```

Depois volte ao **PASSO 1** acima.

---

## ?? RESUMO RÁPIDO

1. **Portal Azure** ? App Services ? finansmart-app ? **Get publish profile**
2. **Abrir arquivo baixado** ? Ctrl+A ? Ctrl+C
3. **GitHub** ? Settings ? Secrets ? New secret
4. **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. **Value**: Ctrl+V
6. **Push**: `git push origin master`

---

**?? COMECE AQUI: https://portal.azure.com**
