using Finansmart.Api.Controllers;
using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Finansmart.Tests
{
    public class CursosControllerTests
    {
        private DbContextOptions<DatabaseContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetCursos_ReturnsPaginatedCursos()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 1, Nome = "Finanças Pessoais", Descricao = "Desc.", AvaliacaoMedia = 4.8M });
                context.Cursos.Add(new Curso { Id = 2, Nome = "Investimentos", Descricao = "Desc.", AvaliacaoMedia = 4.5M });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new CursosController(context);

                // Act
                var result = await controller.GetCursos(1, 1); // página 1, tamanho 1

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                dynamic data = okResult.Value;

                Assert.Equal(2, (int)data.Total);
                Assert.Equal(1, ((System.Collections.IEnumerable)data.Cursos).Cast<object>().Count());
            }
        }

        [Fact]
        public async Task GetProgresso_ReturnsUserProgresso()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 1, Nome = "Curso Teste", Descricao = "D", AvaliacaoMedia = 4.9M });
                context.CursoProgresso.Add(new CursoProgresso
                {
                    CursoId = 1,
                    UsuarioId = 10,
                    PercentualConcluido = 100,
                    ModulosCompletos = "1;2;3",
                    Badges = "Badge1;Badge2",
                    DataConclusao = DateTime.Now
                });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new CursosController(context);

                // Act
                var result = await controller.GetProgresso(1, 10);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                dynamic data = okResult.Value;

                Assert.Equal(1, (int)data.CursoId);
                Assert.Equal(10, (int)data.UsuarioId);
                Assert.Contains("Badge1", data.Badges);
            }
        }

        [Fact]
        public async Task GetProgresso_ReturnsNotFound_WhenNoProgresso()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 1, Nome = "Curso Teste", Descricao = "D", AvaliacaoMedia = 4.9M });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new CursosController(context);

                var result = await controller.GetProgresso(1, 999);
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task AvaliarCurso_RegistersEvaluation_AndUpdatesAverage()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 1, Nome = "Curso Teste", Descricao = "D", AvaliacaoMedia = 0 });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new CursosController(context);
                var dto = new AvaliacaoCursoDto
                {
                    UsuarioId = 10,
                    Nota = 5,
                    Comentario = "Excelente!"
                };

                var result = await controller.AvaliarCurso(1, dto);
                var okResult = Assert.IsType<OkObjectResult>(result);

                // Verifica se avaliacao foi criada
                var avaliacao = context.Avaliacoes.FirstOrDefault(a => a.CursoId == 1 && a.UsuarioId == 10);
                Assert.NotNull(avaliacao);
                Assert.Equal(5, avaliacao.Nota);

                // Verifica se media foi atualizada
                var curso = context.Cursos.Find(1);
                Assert.Equal(5, curso.AvaliacaoMedia);
            }
        }

        [Fact]
        public async Task AvaliarCurso_ReturnsBadRequest_WhenAlreadyEvaluated()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Cursos.Add(new Curso { Id = 1, Nome = "Curso Teste", Descricao = "D", AvaliacaoMedia = 0 });
                context.Avaliacoes.Add(new Avaliacao { CursoId = 1, UsuarioId = 10, Nota = 4, Comentario = "Ok", DataAvaliacao = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new CursosController(context);
                var dto = new AvaliacaoCursoDto
                {
                    UsuarioId = 10,
                    Nota = 5,
                    Comentario = "Teste duplicado"
                };

                var result = await controller.AvaliarCurso(1, dto);
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }
    }
}
