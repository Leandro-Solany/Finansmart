# ? AÇÃO IMEDIATA: Resolver Erro de Deploy

## ?? Execute AGORA

### **Windows (PowerShell)**

```powershell
.\setup-github-deploy.ps1
```

### **Linux/Mac (Bash)**

```bash
chmod +x setup-github-deploy.sh
./setup-github-deploy.sh
```

---

## ?? O Que o Script Faz

1. ? Verifica Azure CLI
2. ? Faz login no Azure
3. ? Cria Resource Group
4. ? Cria App Service Plan
5. ? Cria Web App
6. ? Gera `publish-profile.xml`
7. ? Mostra instruções GitHub

**?? Tempo**: 5-10 minutos  
**?? Requer**: Azure CLI instalado

---

## ?? Após Executar o Script

### **1. Copiar Publish Profile**

```powershell
# Windows
notepad publish-profile.xml
# Ctrl+A ? Ctrl+C
```

```bash
# Mac
cat publish-profile.xml | pbcopy

# Linux
cat publish-profile.xml | xclip -selection clipboard
```

### **2. Adicionar ao GitHub**

?? **Abra**: https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions

```
? New repository secret

Name: AZURE_WEBAPP_PUBLISH_PROFILE
Value: [Cole o XML]

Add secret
```

### **3. Push para Testar**

```bash
git add .
git commit -m "chore: Configure Azure deployment"
git push origin main
```

---

## ? Resultado

```
? Build: Success
? Deploy: Success
? App: https://finansmart-app.azurewebsites.net
? Swagger: https://finansmart-app.azurewebsites.net/swagger
```

---

## ?? Se Não Tiver Azure CLI

### **Windows**

```powershell
winget install -e --id Microsoft.AzureCLI
```

Ou: https://aka.ms/installazurecliwindows

### **Mac**

```bash
brew install azure-cli
```

### **Linux (Ubuntu/Debian)**

```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

---

## ?? Documentação Completa

- **Guia Detalhado**: `SOLVE_CREDENTIALS_ERROR.md`
- **Azure Deploy**: `AZURE_DEPLOY_GUIDE.md`
- **Docker**: `DOCKER_GUIDE.md`

---

## ?? Status Atual

| Item | Status |
|------|--------|
| **Código** | ? OK |
| **Build** | ? OK |
| **Testes** | ? 66/66 |
| **Docker** | ? Configurado |
| **Azure Infra** | ? Executar script |
| **GitHub Secret** | ? Adicionar manualmente |
| **Deploy** | ? Após secret |

---

**?? EXECUTE AGORA: `.\setup-github-deploy.ps1`**

---

**Criado em**: ${new Date().toLocaleString()}  
**Tempo estimado**: 10 minutos  
**Status**: ? Pronto para executar
