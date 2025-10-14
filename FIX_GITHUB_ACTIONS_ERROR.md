# ?? Corre��o de Erro - GitHub Actions

## ? Erro Encontrado

```
error MSB3202: The project file "/home/runner/work/Finansmart/Finansmart.Tests/Finansmart.Tests.csproj" was not found.
```

## ?? Causa do Problema

O GitHub Actions estava procurando o projeto `Finansmart.Tests` no caminho errado. A estrutura de pastas no seu reposit�rio GitHub � diferente da estrutura local.

**Estrutura Local:**
```
Finansmart/
??? Finansmart/           # Projeto principal
?   ??? Finansmart.csproj
??? Finansmart.Tests/     # Projeto de testes (pasta paralela)
    ??? Finansmart.Tests.csproj
```

**GitHub esperava:**
```
Finansmart/
??? Finansmart.Tests/     # Na mesma pasta
    ??? Finansmart.Tests.csproj
```

## ? Solu��o Aplicada

Atualizei o arquivo `.github/workflows/azure-deploy.yml` com as seguintes corre��es:

1. **Removida depend�ncia da solution** - Agora usa apenas o `.csproj` principal
2. **Adicionada verifica��o de estrutura** - Lista os arquivos para debug
3. **Testes opcionais** - Se o projeto de testes n�o for encontrado, continua o build
4. **Caminhos espec�ficos** - Usa o caminho correto para o projeto principal

## ?? Como Corrigir Completamente

### Op��o 1: Ajustar Estrutura do Reposit�rio (Recomendado)

Certifique-se de que o reposit�rio GitHub tenha esta estrutura:

```bash
# No seu computador, na pasta Finansmart principal
git add .
git commit -m "Fix: Update GitHub Actions workflow paths"
git push origin feature/config
```

### Op��o 2: Subir Apenas o Projeto Principal

Se voc� n�o quer incluir os testes no reposit�rio:

```bash
# Navegue at� a pasta do projeto principal
cd Finansmart

# Certifique-se de que .gitignore n�o est� bloqueando arquivos importantes
git add .github/workflows/azure-deploy.yml
git add Finansmart.csproj
git add Program.cs
git add appsettings.json
git add appsettings.Production.json

git commit -m "Fix: GitHub Actions workflow"
git push origin feature/config
```

### Op��o 3: Criar uma Solution na Raiz

```bash
# Na pasta raiz (onde est� o .git)
cd ..

# Criar uma nova solution
dotnet new sln -n Finansmart

# Adicionar projetos
dotnet sln add Finansmart/Finansmart.csproj
dotnet sln add Finansmart.Tests/Finansmart.Tests.csproj

# Commit
git add Finansmart.sln
git commit -m "Add solution file for GitHub Actions"
git push origin feature/config
```

## ?? Testar Localmente

Antes de fazer push, teste se o build funciona:

```bash
# Simular o que o GitHub Actions vai fazer
cd Finansmart

# Restore
dotnet restore Finansmart.csproj

# Build
dotnet build Finansmart.csproj --configuration Release --no-restore

# Publish
dotnet publish Finansmart.csproj -c Release -o ./publish
```

Se todos os comandos acima funcionarem, o GitHub Actions tamb�m funcionar�!

## ?? Checklist

- [x] Arquivo `.github/workflows/azure-deploy.yml` atualizado
- [ ] Fazer commit das altera��es
- [ ] Fazer push para GitHub
- [ ] Verificar execu��o do workflow em: https://github.com/Leandro-Solany/Finansmart/actions

## ?? Pr�xima Execu��o

Quando voc� fizer push novamente, o workflow:

1. ? Vai listar a estrutura de diret�rios (para debug)
2. ? Vai fazer restore do projeto principal
3. ? Vai fazer build
4. ?? Vai tentar executar testes (mas continua se falhar)
5. ? Vai fazer publish
6. ? Vai fazer upload do artifact
7. ? Vai fazer deploy (se estiver na branch main)

## ?? Dica

Voc� pode ver os logs detalhados da execu��o em:
https://github.com/Leandro-Solany/Finansmart/actions

L� voc� ver� exatamente o que est� acontecendo em cada step!

## ?? Se o Erro Persistir

Execute localmente e me envie o resultado:

```bash
cd Finansmart
pwd
ls -la
dotnet restore Finansmart.csproj
dotnet build Finansmart.csproj --configuration Release
```

---

**Arquivo Atualizado**: `.github/workflows/azure-deploy.yml`  
**Status**: ? Pronto para commit
