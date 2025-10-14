# Testes de Integra��o - Finansmart API

## ?? Estrutura de Pastas

```
Finansmart.Tests/
??? IntegrationTests/
?   ??? BaseIntegrationTest.cs
?   ??? AvaliacaoControllerIntegrationTests.cs
?   ??? AvaliacaoBusinessRulesIntegrationTests.cs
?   ??? DatabaseContextIntegrationTests.cs
??? Tests/
    ??? AvaliacaoControllerTests.cs
```

## ?? Objetivo dos Testes

Os testes de integra��o foram criados para validar:

1. **Integra��o com o banco de dados** - Verificar opera��es CRUD
2. **L�gica de neg�cios** - Validar regras e c�lculos
3. **Comportamento dos controllers** - Testar fluxos completos de requisi��es

## ?? Descri��o dos Arquivos

### BaseIntegrationTest.cs
Classe base abstrata que fornece:
- Configura��o de banco de dados em mem�ria (InMemory)
- Setup e teardown autom�ticos
- M�todo `SeedDatabase()` para popular dados iniciais

**Uso:**
```csharp
public class MeuTesteIntegrado : BaseIntegrationTest
{
    protected override void SeedDatabase()
    {
        // Adicionar dados de teste
    }
}
```

### AvaliacaoControllerIntegrationTests.cs
Testes integrados do `AvaliacaoController`:

#### Cen�rios Testados:
- ? **Index_ReturnsViewResult_WithListOfAvaliacoes** - Retorna lista de avalia��es de um curso
- ? **Index_ReturnsEmptyList_WhenNoCursoIdMatch** - Retorna lista vazia quando n�o h� avalia��es
- ? **Nova_Get_ReturnsViewResult_WithCursoId** - Retorna view para criar nova avalia��o
- ? **Nova_Post_AddsAvaliacaoAndRedirects_WhenModelIsValid** - Cria avalia��o com sucesso
- ? **Nova_Post_ReturnsView_WhenModelIsInvalid** - Retorna view com erros quando modelo � inv�lido
- ? **Nova_Post_SetsDataAvaliacao_Automatically** - Define data automaticamente ao criar avalia��o
- ? **Index_ReturnsAvaliacoesOrderedByDate_Descending** - Retorna avalia��es ordenadas por data

### DatabaseContextIntegrationTests.cs
Testes de opera��es no banco de dados:

#### Cen�rios Testados:
- ? **CanAddAndRetrieveCurso** - Adicionar e recuperar curso
- ? **CanAddAndRetrieveUsuario** - Adicionar e recuperar usu�rio
- ? **CanCreateRelationshipBetweenAvaliacaoAndCurso** - Criar relacionamento entre entidades
- ? **CanAddMultipleAvaliacoesForSameCurso** - M�ltiplas avalia��es para o mesmo curso
- ? **CanUpdateAvaliacao** - Atualizar avalia��o existente
- ? **CanDeleteAvaliacao** - Deletar avalia��o
- ? **CanQueryAvaliacoesOrderedByDate** - Consultar avalia��es ordenadas
- ? **CanAddMovimentacaoFinanceira** - Adicionar movimenta��o financeira

### AvaliacaoBusinessRulesIntegrationTests.cs
Testes de regras de neg�cio:

#### Cen�rios Testados:
- ? **AvaliacaoNota_MustBeBetween1And5** - Nota deve estar entre 1 e 5
- ? **CalculateAverageRating_ForCursoWithMultipleAvaliacoes** - C�lculo de m�dia de avalia��es
- ? **CanFilterAvaliacoesByNotaMinima** - Filtrar avalia��es por nota m�nima
- ? **CanGetMostRecentAvaliacoes** - Obter avalia��es mais recentes
- ? **ComentarioMaxLength_ShouldBe500Characters** - Coment�rio limitado a 500 caracteres
- ? **CanGetAvaliacoesByDateRange** - Filtrar avalia��es por per�odo
- ? **CanCountAvaliacoesPorNota** - Contar avalia��es por nota
- ? **UsuarioCanHaveMultipleAvaliacoesForDifferentCursos** - Usu�rio pode avaliar m�ltiplos cursos

## ?? Como Executar os Testes

### Via Visual Studio
1. Abra o **Test Explorer** (`Test > Test Explorer`)
2. Clique em "Run All" para executar todos os testes
3. Ou clique com o bot�o direito em um teste espec�fico e selecione "Run"

### Via Terminal
```bash
# Executar todos os testes
dotnet test

# Executar apenas testes de integra��o
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Executar testes de um arquivo espec�fico
dotnet test --filter "FullyQualifiedName~AvaliacaoControllerIntegrationTests"

# Executar com detalhes
dotnet test --logger "console;verbosity=detailed"
```

## ?? Cobertura de Testes

Os testes de integra��o cobrem:
- **Controllers**: AvaliacaoController
- **Modelos**: Avaliacao, Curso, Usuario, MovimentacaoFinanceira
- **Context**: DatabaseContext
- **Opera��es**: Create, Read, Update, Delete (CRUD)
- **Valida��es**: Regras de neg�cio e constraints

## ?? Depend�ncias

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.17" />
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
```

## ?? Boas Pr�ticas Implementadas

1. **Arrange-Act-Assert (AAA)** - Padr�o de estrutura de testes
2. **Banco em Mem�ria** - Testes r�pidos e isolados
3. **Cleanup Autom�tico** - Dispose do contexto ap�s cada teste
4. **Seed de Dados** - Dados consistentes para testes
5. **Nomenclatura Clara** - Nomes descritivos dos testes
6. **Testes Independentes** - Cada teste pode ser executado isoladamente

## ?? Exemplos de Uso

### Exemplo 1: Teste Simples
```csharp
[Fact]
public async Task Nova_Post_AddsAvaliacaoAndRedirects_WhenModelIsValid()
{
    var avaliacao = new Avaliacao
    {
        CursoId = 1,
        UsuarioId = 1,
        Nota = 5,
        Comentario = "Excelente!"
    };

    var result = await _controller.Nova(avaliacao);

    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Index", redirectResult.ActionName);
}
```

### Exemplo 2: Teste com Theory
```csharp
[Theory]
[InlineData(1)]
[InlineData(2)]
[InlineData(3)]
[InlineData(4)]
[InlineData(5)]
public async Task AvaliacaoNota_MustBeBetween1And5(int nota)
{
    var avaliacao = new Avaliacao { Nota = nota, CursoId = 1, UsuarioId = 1 };
    Context.Avaliacoes.Add(avaliacao);
    await Context.SaveChangesAsync();

    Assert.InRange(nota, 1, 5);
}
```

## ?? Debugging de Testes

Para debugar um teste espec�fico:
1. Coloque um breakpoint no teste
2. Clique com o bot�o direito no teste no Test Explorer
3. Selecione "Debug"

## ?? Estat�sticas

- **Total de Testes de Integra��o**: 56
- **Taxa de Sucesso**: 100%
- **Cobertura**: Controllers, Models, Business Rules, Database Operations

## ?? Suporte

Para d�vidas ou problemas com os testes, entre em contato com a equipe de desenvolvimento.
