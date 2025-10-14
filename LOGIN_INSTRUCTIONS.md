# ?? LOGIN NO AZURE - INSTRUÇÕES

## ? PASSO A PASSO

### 1. Abra o navegador e vá para:
https://microsoft.com/devicelogin

### 2. Digite o código:
**AWZVRD8GS**

### 3. Clique em "Next"

### 4. Faça login com o email CORRETO do Azure

### 5. Autorize o Azure CLI

### 6. Volte ao PowerShell

---

## ?? Depois do Login

Execute este comando para obter o Publish Profile:

```powershell
az webapp deployment list-publishing-profiles --name finansmart-app --resource-group finansmart-rg --xml | Out-File -FilePath publish-profile.xml -Encoding UTF8
```

Ou execute o script completo:

```powershell
.\RUN_AZURE_SETUP.ps1
```

---

**?? ABRIR AGORA: https://microsoft.com/devicelogin**
**?? CÓDIGO: AWZVRD8GS**
