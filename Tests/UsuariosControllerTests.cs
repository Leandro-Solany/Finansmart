using Finansmart.Api.Controllers;
using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Finansmart.Tests
{
    public class UsuariosControllerTests
    {
        private DbContextOptions<DatabaseContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetUsuarios_ReturnsPaginatedAndFiltered()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Usuarios.Add(new Usuario { Id = 1, Nome = "Ana", Email = "ana@email.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now });
                context.Usuarios.Add(new Usuario { Id = 2, Nome = "Bruno", Email = "bruno@email.com", Status = "Inativo", Nivel = "Administrador", DataCadastro = DateTime.Now });
                context.Usuarios.Add(new Usuario { Id = 3, Nome = "Clara", Email = "clara@email.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                // Act
                var result = await controller.GetUsuarios(nome: "a", status: "Ativo", nivel: null, page: 1, pageSize: 2);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                dynamic data = okResult.Value;

                Assert.Equal(2, (int)data.Total); // Ana e Clara
                Assert.Equal(2, ((System.Collections.IEnumerable)data.Usuarios).Cast<object>().Count());
            }
        }

        [Fact]
        public async Task GetUsuario_ReturnsUsuario_WhenFound()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Usuarios.Add(new Usuario { Id = 5, Nome = "Davi", Email = "davi@email.com", Status = "Ativo", Nivel = "Mentor", DataCadastro = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var result = await controller.GetUsuario(5);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var usuario = Assert.IsType<Usuario>(okResult.Value);
                Assert.Equal("Davi", usuario.Nome);
            }
        }

        [Fact]
        public async Task GetUsuario_ReturnsNotFound_WhenNotExists()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options)) { }
            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var result = await controller.GetUsuario(999);
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task CreateUsuario_CreatesUserAndReturnsCreated()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options)) { }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var usuario = new Usuario
                {
                    Nome = "Erika",
                    Email = "erika@email.com",
                    Status = "Ativo",
                    Nivel = "Aluno"
                };

                var result = await controller.CreateUsuario(usuario);

                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                var usuarioCriado = Assert.IsType<Usuario>(createdResult.Value);

                Assert.Equal("Erika", usuarioCriado.Nome);
                Assert.NotEqual(default(DateTime), usuarioCriado.DataCadastro);

                // Verifica se foi salvo no banco
                var dbUser = context.Usuarios.SingleOrDefault(u => u.Email == "erika@email.com");
                Assert.NotNull(dbUser);
            }
        }

        [Fact]
        public async Task UpdateUsuario_UpdatesFields()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Usuarios.Add(new Usuario { Id = 7, Nome = "Fernanda", Email = "fernanda@email.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var usuarioAtualizado = new Usuario
                {
                    Nome = "Fernanda Silva",
                    Email = "fer@email.com",
                    Status = "Inativo",
                    Nivel = "Mentor"
                };

                var result = await controller.UpdateUsuario(7, usuarioAtualizado);

                Assert.IsType<NoContentResult>(result);

                var userDb = context.Usuarios.Find(7);
                Assert.Equal("Fernanda Silva", userDb.Nome);
                Assert.Equal("fer@email.com", userDb.Email);
                Assert.Equal("Inativo", userDb.Status);
                Assert.Equal("Mentor", userDb.Nivel);
            }
        }

        [Fact]
        public async Task UpdateUsuario_ReturnsNotFound_WhenUserNotExists()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options)) { }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var usuario = new Usuario
                {
                    Nome = "Ghost",
                    Email = "ghost@email.com",
                    Status = "Ativo",
                    Nivel = "Aluno"
                };

                var result = await controller.UpdateUsuario(99, usuario);
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteUsuario_RemovesUser()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.Usuarios.Add(new Usuario { Id = 20, Nome = "Heitor", Email = "heitor@email.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now });
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var result = await controller.DeleteUsuario(20);
                Assert.IsType<NoContentResult>(result);

                var userDb = context.Usuarios.Find(20);
                Assert.Null(userDb);
            }
        }

        [Fact]
        public async Task DeleteUsuario_ReturnsNotFound_WhenUserNotExists()
        {
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options)) { }

            using (var context = new DatabaseContext(options))
            {
                var controller = new UsuariosController(context);

                var result = await controller.DeleteUsuario(999);
                Assert.IsType<NotFoundResult>(result);
            }
        }
    }
}
