# ? CHECKLIST: Resolver Erro "No credentials found"

## ?? Status Atual

```
? Erro: No credentials found
?? Causa: Secret GitHub não configurado
? Solução: Executar script e adicionar secret
```

---

## ?? Passo a Passo Visual

### ? **PASSO 1: Executar Script** (5 minutos)

```powershell
# Windows PowerShell
.\setup-github-deploy.ps1
```

**Ou:**

```bash
# Linux/Mac
chmod +x setup-github-deploy.sh
./setup-github-deploy.sh
```

**O que acontece:**
- ? Login no Azure
- ? Cria finansmart-rg (Resource Group)
- ? Cria finansmart-plan (App Service Plan)
- ? Cria finansmart-app (Web App)
- ? Gera publish-profile.xml

---

### ? **PASSO 2: Copiar Publish Profile** (1 minuto)

#### **Windows:**

```powershell
notepad publish-profile.xml
```

Depois:
1. Ctrl+A (selecionar tudo)
2. Ctrl+C (copiar)

#### **Mac:**

```bash
cat publish-profile.xml | pbcopy
```

#### **Linux:**

```bash
cat publish-profile.xml | xclip -selection clipboard
```

**Resultado:** XML está copiado na área de transferência

---

### ? **PASSO 3: Adicionar ao GitHub** (2 minutos)

#### **3.1 - Abrir GitHub Secrets**

?? Clique aqui: [GitHub Secrets](https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions)

Ou navegue:
1. Seu repositório ? Settings
2. Secrets and variables ? Actions
3. New repository secret

#### **3.2 - Preencher Formulário**

```
??????????????????????????????????????????
? Name *                                 ?
? AZURE_WEBAPP_PUBLISH_PROFILE           ?
??????????????????????????????????????????
? Secret *                               ?
? [Cole aqui o XML - Ctrl+V]            ?
? <?xml version="1.0" encoding="utf-8"?> ?
? <publishData>                          ?
?   <publishProfile ...>                 ?
?   ...                                  ?
? </publishData>                         ?
??????????????????????????????????????????

         [ Add secret ]
```

**?? IMPORTANTE:**
- ? Nome EXATO: `AZURE_WEBAPP_PUBLISH_PROFILE`
- ? Cole o XML COMPLETO (incluindo primeira e última linha)
- ? Não remova nenhum caractere

---

### ? **PASSO 4: Fazer Push** (1 minuto)

```bash
git add .
git commit -m "chore: Configure Azure deployment credentials"
git push origin main
```

**Ou se estiver em outra branch:**

```bash
git checkout main
git merge feature/config
git push origin main
```

---

### ? **PASSO 5: Verificar Deploy** (3 minutos)

#### **5.1 - GitHub Actions**

?? [Ver Workflows](https://github.com/Leandro-Solany/Finansmart/actions)

Você verá:
```
? Build job
  ? Checkout
  ? Setup .NET
  ? Restore
  ? Build
  ? Publish
  ? Upload artifact

? Deploy job
  ? Download artifact
  ? Deploy to Azure ? DEVE FUNCIONAR AGORA!
```

#### **5.2 - Azure Portal**

?? [Azure Portal](https://portal.azure.com)

1. App Services ? finansmart-app
2. Deployment Center ? Ver logs
3. Status: ? Success

#### **5.3 - Testar App**

?? [App](https://finansmart-app.azurewebsites.net)  
?? [Swagger](https://finansmart-app.azurewebsites.net/swagger)

---

## ?? Checklist Final

- [ ] **Script executado** (`setup-github-deploy.ps1` ou `.sh`)
- [ ] **Azure recursos criados** (rg, plan, webapp)
- [ ] **publish-profile.xml gerado** (arquivo existe)
- [ ] **XML copiado** (Ctrl+A ? Ctrl+C)
- [ ] **GitHub Secret adicionado** (Name: AZURE_WEBAPP_PUBLISH_PROFILE)
- [ ] **Push para main** (commit + push)
- [ ] **Workflow executou** (GitHub Actions)
- [ ] **Deploy bem-sucedido** (status verde)
- [ ] **App acessível** (URL responde)
- [ ] **Swagger funciona** (documentação API)

---

## ?? Resultado Esperado

### **Antes:**

```
? Deploy Failed
? Error: No credentials found
```

### **Depois:**

```
? Build: Success
? Tests: 66/66 Passed
? Deploy: Success
? App: Online

?? https://finansmart-app.azurewebsites.net
?? https://finansmart-app.azurewebsites.net/swagger
```

---

## ?? Troubleshooting Rápido

### ? "Azure CLI not found"

```powershell
winget install -e --id Microsoft.AzureCLI
```

### ? "Secret not found"

Verifique o nome: `AZURE_WEBAPP_PUBLISH_PROFILE` (exato!)

### ? "Invalid publish profile"

XML incompleto. Execute o script novamente.

### ? "Workflow still failing"

1. Delete o secret no GitHub
2. Execute `.\setup-github-deploy.ps1` novamente
3. Adicione o secret novamente
4. Push novamente

---

## ?? Ajuda

- **Script PowerShell**: `setup-github-deploy.ps1`
- **Script Bash**: `setup-github-deploy.sh`
- **Guia Detalhado**: `SOLVE_CREDENTIALS_ERROR.md`
- **Quick Start**: `QUICK_START_DEPLOY.md`

---

## ?? Tempo Total Estimado

- 5 min: Executar script
- 1 min: Copiar XML
- 2 min: Adicionar secret
- 1 min: Push
- 3 min: Verificar deploy

**Total: ~12 minutos**

---

**?? COMECE AGORA: `.\setup-github-deploy.ps1`**

---

**Status**: ? Pronto para executar  
**Dificuldade**: ?? (Fácil)  
**Risco**: Zero (reversível)
