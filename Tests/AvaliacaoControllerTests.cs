using Finansmart.Api.Controllers;
using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Finansmart.Tests
{
    public class AvaliacaoControllerTests
    {
        private DbContextOptions<DatabaseContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAvaliacoes_ReturnsPaginatedList()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 1, Nome = "Curso 1", Descricao = "Desc" });
                context.Avaliacoes.Add(new Avaliacao { Id = 1, CursoId = 1, UsuarioId = 2, Nota = 4, Comentario = "Bom", DataAvaliacao = DateTime.Now });
                context.Avaliacoes.Add(new Avaliacao { Id = 2, CursoId = 1, UsuarioId = 3, Nota = 5, Comentario = "Ótimo", DataAvaliacao = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new AvaliacaoController(context);

                // Act
                var result = await controller.GetAvaliacoes(1, 1, 1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                dynamic data = okResult.Value;
                Assert.Equal(2, (int)data.Total);
                Assert.Single(data.Avaliacoes);
            }
        }

        [Fact]
        public async Task AvaliarCurso_RegistersNewAvaliacao()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 2, Nome = "Curso Teste" });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new AvaliacaoController(context);
                var dto = new AvaliacaoCursoDto
                {
                    UsuarioId = 10,
                    Nota = 5,
                    Comentario = "Excelente"
                };

                var result = await controller.AvaliarCurso(2, dto);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Avaliação registrada com sucesso!", okResult.Value);

                // Confirma que salvou
                var avaliacao = context.Avaliacoes.FirstOrDefault(a => a.CursoId == 2 && a.UsuarioId == 10);
                Assert.NotNull(avaliacao);
                Assert.Equal(5, avaliacao.Nota);
            }
        }

        [Fact]
        public async Task AvaliarCurso_ReturnsBadRequest_WhenAlreadyEvaluated()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 3, Nome = "Curso Avaliação" });
                context.Avaliacoes.Add(new Avaliacao { CursoId = 3, UsuarioId = 99, Nota = 3, Comentario = "Normal", DataAvaliacao = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new AvaliacaoController(context);
                var dto = new AvaliacaoCursoDto
                {
                    UsuarioId = 99,
                    Nota = 4,
                    Comentario = "Tentando duplicar"
                };

                var result = await controller.AvaliarCurso(3, dto);

                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task AvaliarCurso_ReturnsNotFound_WhenCursoDoesNotExist()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options)) { }

            using (var context = new DatabaseContext(options))
            {
                var controller = new AvaliacaoController(context);
                var dto = new AvaliacaoCursoDto
                {
                    UsuarioId = 15,
                    Nota = 3,
                    Comentario = "Curso não existe"
                };

                var result = await controller.AvaliarCurso(999, dto);

                Assert.IsType<NotFoundObjectResult>(result);
            }
        }
    }
}
