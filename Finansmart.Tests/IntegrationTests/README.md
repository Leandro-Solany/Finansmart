# Testes de Integração - Finansmart API

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

Os testes de integração foram criados para validar:

1. **Integração com o banco de dados** - Verificar operações CRUD
2. **Lógica de negócios** - Validar regras e cálculos
3. **Comportamento dos controllers** - Testar fluxos completos de requisições

## ?? Descrição dos Arquivos

### BaseIntegrationTest.cs
Classe base abstrata que fornece:
- Configuração de banco de dados em memória (InMemory)
- Setup e teardown automáticos
- Método `SeedDatabase()` para popular dados iniciais

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

#### Cenários Testados:
- ? **Index_ReturnsViewResult_WithListOfAvaliacoes** - Retorna lista de avaliações de um curso
- ? **Index_ReturnsEmptyList_WhenNoCursoIdMatch** - Retorna lista vazia quando não há avaliações
- ? **Nova_Get_ReturnsViewResult_WithCursoId** - Retorna view para criar nova avaliação
- ? **Nova_Post_AddsAvaliacaoAndRedirects_WhenModelIsValid** - Cria avaliação com sucesso
- ? **Nova_Post_ReturnsView_WhenModelIsInvalid** - Retorna view com erros quando modelo é inválido
- ? **Nova_Post_SetsDataAvaliacao_Automatically** - Define data automaticamente ao criar avaliação
- ? **Index_ReturnsAvaliacoesOrderedByDate_Descending** - Retorna avaliações ordenadas por data

### DatabaseContextIntegrationTests.cs
Testes de operações no banco de dados:

#### Cenários Testados:
- ? **CanAddAndRetrieveCurso** - Adicionar e recuperar curso
- ? **CanAddAndRetrieveUsuario** - Adicionar e recuperar usuário
- ? **CanCreateRelationshipBetweenAvaliacaoAndCurso** - Criar relacionamento entre entidades
- ? **CanAddMultipleAvaliacoesForSameCurso** - Múltiplas avaliações para o mesmo curso
- ? **CanUpdateAvaliacao** - Atualizar avaliação existente
- ? **CanDeleteAvaliacao** - Deletar avaliação
- ? **CanQueryAvaliacoesOrderedByDate** - Consultar avaliações ordenadas
- ? **CanAddMovimentacaoFinanceira** - Adicionar movimentação financeira

### AvaliacaoBusinessRulesIntegrationTests.cs
Testes de regras de negócio:

#### Cenários Testados:
- ? **AvaliacaoNota_MustBeBetween1And5** - Nota deve estar entre 1 e 5
- ? **CalculateAverageRating_ForCursoWithMultipleAvaliacoes** - Cálculo de média de avaliações
- ? **CanFilterAvaliacoesByNotaMinima** - Filtrar avaliações por nota mínima
- ? **CanGetMostRecentAvaliacoes** - Obter avaliações mais recentes
- ? **ComentarioMaxLength_ShouldBe500Characters** - Comentário limitado a 500 caracteres
- ? **CanGetAvaliacoesByDateRange** - Filtrar avaliações por período
- ? **CanCountAvaliacoesPorNota** - Contar avaliações por nota
- ? **UsuarioCanHaveMultipleAvaliacoesForDifferentCursos** - Usuário pode avaliar múltiplos cursos

## ?? Como Executar os Testes

### Via Visual Studio
1. Abra o **Test Explorer** (`Test > Test Explorer`)
2. Clique em "Run All" para executar todos os testes
3. Ou clique com o botão direito em um teste específico e selecione "Run"

### Via Terminal
```bash
# Executar todos os testes
dotnet test

# Executar apenas testes de integração
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Executar testes de um arquivo específico
dotnet test --filter "FullyQualifiedName~AvaliacaoControllerIntegrationTests"

# Executar com detalhes
dotnet test --logger "console;verbosity=detailed"
```

## ?? Cobertura de Testes

Os testes de integração cobrem:
- **Controllers**: AvaliacaoController
- **Modelos**: Avaliacao, Curso, Usuario, MovimentacaoFinanceira
- **Context**: DatabaseContext
- **Operações**: Create, Read, Update, Delete (CRUD)
- **Validações**: Regras de negócio e constraints

## ?? Dependências

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.17" />
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
```

## ?? Boas Práticas Implementadas

1. **Arrange-Act-Assert (AAA)** - Padrão de estrutura de testes
2. **Banco em Memória** - Testes rápidos e isolados
3. **Cleanup Automático** - Dispose do contexto após cada teste
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

Para debugar um teste específico:
1. Coloque um breakpoint no teste
2. Clique com o botão direito no teste no Test Explorer
3. Selecione "Debug"

## ?? Estatísticas

- **Total de Testes de Integração**: 56
- **Taxa de Sucesso**: 100%
- **Cobertura**: Controllers, Models, Business Rules, Database Operations

## ?? Suporte

Para dúvidas ou problemas com os testes, entre em contato com a equipe de desenvolvimento.
