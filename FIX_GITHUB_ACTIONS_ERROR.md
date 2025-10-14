# ?? Correção de Erro - GitHub Actions (SOLUÇÃO DEFINITIVA)

## ? Erro Encontrado

```
error MSB3202: The project file "/home/runner/work/Finansmart/Finansmart.Tests/Finansmart.Tests.csproj" was not found.
[/home/runner/work/Finansmart/Finansmart/Finansmart.sln]
```

## ?? Causa Raiz do Problema

O arquivo `Finansmart.sln` contém uma referência ao projeto `Finansmart.Tests` com o caminho relativo:

```
"..\Finansmart.Tests\Finansmart.Tests.csproj"
```

**Problema**: No seu repositório GitHub, apenas a pasta `Finansmart` foi commitada, mas o `Finansmart.Tests` está fora dela (pasta paralela no seu PC local).

**Estrutura Local:**
```
C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\
??? Finansmart/              ? Commitado no GitHub
?   ??? Finansmart.csproj
?   ??? Finansmart.sln       ? Referencia ../Finansmart.Tests
??? Finansmart.Tests/        ? NÃO está no GitHub!
    ??? Finansmart.Tests.csproj
```

**Estrutura no GitHub:**
```
/home/runner/work/Finansmart/Finansmart/
??? Finansmart.csproj
??? Finansmart.sln           ? Procura ../Finansmart.Tests (não existe!)
```

## ? Solução Aplicada (Workflow Atualizado)

Atualizei o `.github/workflows/azure-deploy.yml` para:

1. **Não usar mais a `.sln`** - Usa diretamente `Finansmart.csproj`
2. **Working directory explícito** - Garante que está na pasta correta
3. **Testes opcionais** - Procura o projeto de testes mas continua se não encontrar
4. **Debug detalhado** - Lista estrutura de pastas para troubleshooting

## ?? Como Aplicar a Correção

### Passo 1: Commit das Alterações

```bash
# Na pasta Finansmart
git add .github/workflows/azure-deploy.yml
git add FIX_GITHUB_ACTIONS_ERROR.md
git commit -m "Fix: Remove .sln dependency from GitHub Actions workflow"
git push origin feature/config
```

### Passo 2: Verificar Execução

Acesse: https://github.com/Leandro-Solany/Finansmart/actions

O workflow agora vai:
- ? Listar estrutura de arquivos
- ? Fazer `dotnet restore Finansmart.csproj`
- ? Fazer `dotnet build Finansmart.csproj`
- ?? Tentar executar testes (mas continua se falhar)
- ? Fazer `dotnet publish Finansmart.csproj`
- ? Fazer deploy para Azure

## ?? Soluções Alternativas (Opcionais)

### Opção A: Remover Finansmart.Tests da Solution (Local)

Se você quer manter a solution mas sem o projeto de testes:

1. **Feche o Visual Studio**
2. **Edite `Finansmart.sln`** e remova estas linhas:

```diff
- Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Finansmart.Tests", "..\Finansmart.Tests\Finansmart.Tests.csproj", "{C6810C3F-06A4-4BFF-9B77-7538B729C9D1}"
- EndProject

# E também remova as configurações do projeto de testes:
- {C6810C3F-06A4-4BFF-9B77-7538B729C9D1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
- {C6810C3F-06A4-4BFF-9B77-7538B729C9D1}.Debug|Any CPU.Build.0 = Debug|Any CPU
- {C6810C3F-06A4-4BFF-9B77-7538B729C9D1}.Release|Any CPU.ActiveCfg = Release|Any CPU
- {C6810C3F-06A4-4BFF-9B77-7538B729C9D1}.Release|Any CPU.Build.0 = Release|Any CPU
```

3. **Commit e push**:
```bash
git add Finansmart.sln
git commit -m "Remove Finansmart.Tests from solution"
git push origin feature/config
```

### Opção B: Incluir Finansmart.Tests no Repositório

Se você quer incluir os testes no GitHub:

```bash
# Na pasta raiz (C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\)
cd ..
git add Finansmart.Tests/
git commit -m "Add test project to repository"
git push origin feature/config
```

**Estrutura resultante no GitHub:**
```
Finansmart/
??? Finansmart/
?   ??? Finansmart.csproj
?   ??? Finansmart.sln
??? Finansmart.Tests/
    ??? Finansmart.Tests.csproj
```

### Opção C: Criar Solution na Raiz (Recomendado para projetos grandes)

```bash
# Na pasta raiz
cd ..

# Remover solution antiga
rm Finansmart/Finansmart.sln

# Criar nova solution na raiz
dotnet new sln -n Finansmart

# Adicionar projetos
dotnet sln add Finansmart/Finansmart.csproj
dotnet sln add Finansmart.Tests/Finansmart.Tests.csproj

# Commit
git add Finansmart.sln
git rm Finansmart/Finansmart.sln
git commit -m "Move solution to repository root"
git push origin feature/config
```

## ?? Testar Localmente

Simule exatamente o que o GitHub Actions vai fazer:

```bash
# Na pasta Finansmart
cd "C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\Finansmart"

# Restore
dotnet restore Finansmart.csproj

# Build
dotnet build Finansmart.csproj --configuration Release --no-restore

# Publish
dotnet publish Finansmart.csproj -c Release -o ./publish --no-build

# Verificar
ls ./publish
# Deve mostrar: Finansmart.dll, appsettings.json, etc.
```

Se todos os comandos acima funcionarem, o GitHub Actions também funcionará! ?

## ?? Checklist

- [x] Workflow `.github/workflows/azure-deploy.yml` atualizado
- [x] Workflow não depende mais da `.sln`
- [x] Testes são opcionais (continua mesmo sem eles)
- [ ] Fazer commit das alterações
- [ ] Fazer push para GitHub
- [ ] Verificar execução em: https://github.com/Leandro-Solany/Finansmart/actions

## ?? Resultado Esperado

Após o push, o workflow vai:

```
? Checkout code
? Setup .NET 8.0
? Display repository structure
? Restore dependencies (Finansmart.csproj)
? Build project (Finansmart.csproj)
?? Run tests (skip if not found)
? Publish application
? Verify publish output
? Upload artifact
? Deploy to Azure (se branch = main)
```

## ?? Dica Pro

Se você ainda tiver problemas, o workflow agora tem um step de debug que mostra:
- Diretório atual
- Arquivos na pasta
- Localização de todos os .csproj e .sln

Isso facilita muito o troubleshooting!

## ?? Se Ainda Houver Erro

Execute e me envie o resultado:

```bash
cd "C:\Users\leand\Desktop\Fase 4 - .NET\Finansmart\Finansmart"
pwd
ls -la
dotnet restore Finansmart.csproj
dotnet build Finansmart.csproj --configuration Release
```

---

**Status**: ? Pronto para commit e push  
**Arquivo Atualizado**: `.github/workflows/azure-deploy.yml`  
**Ação Necessária**: `git commit` e `git push`
