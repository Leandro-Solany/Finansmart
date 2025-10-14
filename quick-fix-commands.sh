# ========================================
# SOLUÇÃO DEFINITIVA - Execute Estes Comandos
# ========================================

# 1. VERIFICAR QUE ESTÁ NA PASTA CORRETA
cd "C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\Finansmart"
pwd
# Deve mostrar: C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\Finansmart

# 2. TESTAR LOCALMENTE PRIMEIRO (Opcional mas recomendado)
dotnet restore Finansmart.csproj
dotnet build Finansmart.csproj --configuration Release --no-restore
dotnet publish Finansmart.csproj -c Release -o ./publish --no-build

# 3. VERIFICAR SE O PUBLISH FUNCIONOU
ls ./publish
# Deve mostrar: Finansmart.dll, appsettings.json, web.config, etc.

# 4. FAZER COMMIT DAS ALTERAÇÕES
git add .github/workflows/azure-deploy.yml
git add FIX_GITHUB_ACTIONS_ERROR.md
git commit -m "Fix: Remove .sln dependency, use Finansmart.csproj directly"

# 5. FAZER PUSH PARA O GITHUB
git push origin feature/config

# 6. VERIFICAR EXECUÇÃO NO GITHUB
# Abra no navegador: https://github.com/Leandro-Solany/Finansmart/actions
# Você verá o workflow executando em tempo real

# ========================================
# O workflow agora vai funcionar porque:
# ? Não usa mais Finansmart.sln
# ? Usa diretamente Finansmart.csproj
# ? Não depende de arquivos externos
# ? Tem debug para mostrar estrutura de pastas
# ========================================

# COMANDOS PARA POWERSHELL (se preferir):
# cd "C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\Finansmart"
# dotnet restore Finansmart.csproj
# dotnet build Finansmart.csproj --configuration Release --no-restore
# dotnet publish Finansmart.csproj -c Release -o ./publish --no-build
# git add .github/workflows/azure-deploy.yml FIX_GITHUB_ACTIONS_ERROR.md
# git commit -m "Fix: Remove .sln dependency, use Finansmart.csproj directly"
# git push origin feature/config
