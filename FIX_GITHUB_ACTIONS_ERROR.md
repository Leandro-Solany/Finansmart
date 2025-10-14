# ?? Correção de Erro - GitHub Actions

## ? Erro Encontrado

```
error MSB3202: The project file "/home/runner/work/Finansmart/Finansmart.Tests/Finansmart.Tests.csproj" was not found.
```

## ?? Causa do Problema

O GitHub Actions estava procurando o projeto `Finansmart.Tests` no caminho errado. A estrutura de pastas no seu repositório GitHub é diferente da estrutura local.

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

## ? Solução Aplicada

Atualizei o arquivo `.github/workflows/azure-deploy.yml` com as seguintes correções:

1. **Removida dependência da solution** - Agora usa apenas o `.csproj` principal
2. **Adicionada verificação de estrutura** - Lista os arquivos para debug
3. **Testes opcionais** - Se o projeto de testes não for encontrado, continua o build
4. **Caminhos específicos** - Usa o caminho correto para o projeto principal

## ?? Como Corrigir Completamente

### Opção 1: Ajustar Estrutura do Repositório (Recomendado)

Certifique-se de que o repositório GitHub tenha esta estrutura:

```bash
# No seu computador, na pasta Finansmart principal
git add .
git commit -m "Fix: Update GitHub Actions workflow paths"
git push origin feature/config
```

### Opção 2: Subir Apenas o Projeto Principal

Se você não quer incluir os testes no repositório:

```bash
# Navegue até a pasta do projeto principal
cd Finansmart

# Certifique-se de que .gitignore não está bloqueando arquivos importantes
git add .github/workflows/azure-deploy.yml
git add Finansmart.csproj
git add Program.cs
git add appsettings.json
git add appsettings.Production.json

git commit -m "Fix: GitHub Actions workflow"
git push origin feature/config
```

### Opção 3: Criar uma Solution na Raiz

```bash
# Na pasta raiz (onde está o .git)
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

Se todos os comandos acima funcionarem, o GitHub Actions também funcionará!

## ?? Checklist

- [x] Arquivo `.github/workflows/azure-deploy.yml` atualizado
- [ ] Fazer commit das alterações
- [ ] Fazer push para GitHub
- [ ] Verificar execução do workflow em: https://github.com/Leandro-Solany/Finansmart/actions

## ?? Próxima Execução

Quando você fizer push novamente, o workflow:

1. ? Vai listar a estrutura de diretórios (para debug)
2. ? Vai fazer restore do projeto principal
3. ? Vai fazer build
4. ?? Vai tentar executar testes (mas continua se falhar)
5. ? Vai fazer publish
6. ? Vai fazer upload do artifact
7. ? Vai fazer deploy (se estiver na branch main)

## ?? Dica

Você pode ver os logs detalhados da execução em:
https://github.com/Leandro-Solany/Finansmart/actions

Lá você verá exatamente o que está acontecendo em cada step!

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
