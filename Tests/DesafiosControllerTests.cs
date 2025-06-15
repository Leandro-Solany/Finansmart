using Finansmart.Data;
using Finansmart.Models;
using Finansmart.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace Finansmart.Tests
{
    public class DesafiosControllerTests
    {
        private DbContextOptions<DatabaseContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetDesafios_ReturnsActiveDesafios()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Desafios.Add(new Desafio { Id = 1, Titulo = "Desafio 1", Ativo = true, Pontuacao = 50 });
                context.Desafios.Add(new Desafio { Id = 2, Titulo = "Desafio 2", Ativo = false, Pontuacao = 100 });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DesafiosController(context);

                // Act
                var result = await controller.GetDesafios();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                dynamic data = okResult.Value;
                Assert.Equal(1, (int)data.Total);
                Assert.Single(data.Desafios);
            }
        }

        [Fact]
        public async Task ConcluirDesafio_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Desafios.Add(new Desafio { Id = 1, Titulo = "Desafio 1", Ativo = true, Pontuacao = 120 });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DesafiosController(context);
                var dto = new ConcluirDesafioDto { DesafioId = 1, UsuarioId = 10 };

                // Act
                var result = await controller.ConcluirDesafio(dto);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                dynamic data = okResult.Value;
                Assert.Equal("Desafio concluído com sucesso!", (string)data.Mensagem);
                Assert.Equal("Master", (string)data.BadgeRecebido);
                Assert.Equal(120, (int)data.Pontuacao);
            }
        }

        [Fact]
        public async Task ConcluirDesafio_ReturnsNotFound_WhenDesafioNotFoundOrInactive()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Desafios.Add(new Desafio { Id = 1, Titulo = "Desafio 1", Ativo = false, Pontuacao = 50 });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DesafiosController(context);
                var dto = new ConcluirDesafioDto { DesafioId = 1, UsuarioId = 10 };

                // Act
                var result = await controller.ConcluirDesafio(dto);

                // Assert
                Assert.IsType<NotFoundObjectResult>(result);
            }
        }

        [Fact]
        public async Task ConcluirDesafio_ReturnsBadRequest_WhenAlreadyConcluded()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Desafios.Add(new Desafio { Id = 1, Titulo = "Desafio 1", Ativo = true, Pontuacao = 80 });
                context.DesafioUsuarios.Add(new DesafioUsuario
                {
                    UsuarioId = 10,
                    DesafioId = 1,
                    Concluido = true
                });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DesafiosController(context);
                var dto = new ConcluirDesafioDto { DesafioId = 1, UsuarioId = 10 };

                // Act
                var result = await controller.ConcluirDesafio(dto);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }
    }
}
