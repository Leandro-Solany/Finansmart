using Finansmart.Controllers;
using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Finansmart.Tests.IntegrationTests
{
    public class AvaliacaoControllerIntegrationTests : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly AvaliacaoController _controller;

        public AvaliacaoControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DatabaseContext(options);
            _controller = new AvaliacaoController(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var curso = new Curso
            {
                Id = 1,
                Nome = "Curso de Finanças Pessoais",
                Descricao = "Aprenda a gerenciar suas finanças",
                AvaliacaoMedia = 0
            };

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Teste Usuario",
                Email = "teste@finansmart.com",
                Status = "Ativo",
                Nivel = "Aluno",
                DataCadastro = DateTime.Now
            };

            _context.Cursos.Add(curso);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfAvaliacoes()
        {
            var avaliacao1 = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 5,
                Comentario = "Excelente curso!",
                DataAvaliacao = DateTime.Now
            };

            var avaliacao2 = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 4,
                Comentario = "Muito bom!",
                DataAvaliacao = DateTime.Now.AddDays(-1)
            };

            _context.Avaliacoes.AddRange(avaliacao1, avaliacao2);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Avaliacao>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            Assert.Equal(1, viewResult.ViewData["CursoId"]);
        }

        [Fact]
        public async Task Index_ReturnsEmptyList_WhenNoCursoIdMatch()
        {
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 5,
                Comentario = "Excelente!",
                DataAvaliacao = DateTime.Now
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(999);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Avaliacao>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Nova_Get_ReturnsViewResult_WithCursoId()
        {
            var result = _controller.Nova(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(1, viewResult.ViewData["CursoId"]);
        }

        [Fact]
        public async Task Nova_Post_AddsAvaliacaoAndRedirects_WhenModelIsValid()
        {
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 5,
                Comentario = "Curso excelente!"
            };

            var result = await _controller.Nova(avaliacao);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues!["cursoId"]);

            var savedAvaliacao = await _context.Avaliacoes.FirstOrDefaultAsync(a => a.CursoId == 1);
            Assert.NotNull(savedAvaliacao);
            Assert.Equal(5, savedAvaliacao.Nota);
            Assert.Equal("Curso excelente!", savedAvaliacao.Comentario);
            Assert.NotNull(savedAvaliacao.DataAvaliacao);
        }

        [Fact]
        public async Task Nova_Post_ReturnsView_WhenModelIsInvalid()
        {
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 10,
                Comentario = "Teste"
            };

            _controller.ModelState.AddModelError("Nota", "A nota deve ser entre 1 e 5.");

            var result = await _controller.Nova(avaliacao);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(avaliacao, viewResult.Model);
            Assert.Equal(1, viewResult.ViewData["CursoId"]);
        }

        [Fact]
        public async Task Nova_Post_SetsDataAvaliacao_Automatically()
        {
            var beforeTime = DateTime.Now.AddSeconds(-1);
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 4,
                Comentario = "Bom curso"
            };

            await _controller.Nova(avaliacao);
            var afterTime = DateTime.Now.AddSeconds(1);

            var savedAvaliacao = await _context.Avaliacoes.FirstOrDefaultAsync();
            Assert.NotNull(savedAvaliacao);
            Assert.NotNull(savedAvaliacao.DataAvaliacao);
            Assert.True(savedAvaliacao.DataAvaliacao >= beforeTime && savedAvaliacao.DataAvaliacao <= afterTime);
        }

        [Fact]
        public async Task Index_ReturnsAvaliacoesOrderedByDate_Descending()
        {
            var avaliacao1 = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 5,
                Comentario = "Primeira",
                DataAvaliacao = DateTime.Now.AddDays(-5)
            };

            var avaliacao2 = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 4,
                Comentario = "Segunda",
                DataAvaliacao = DateTime.Now.AddDays(-2)
            };

            var avaliacao3 = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 3,
                Comentario = "Terceira (mais recente)",
                DataAvaliacao = DateTime.Now
            };

            _context.Avaliacoes.AddRange(avaliacao1, avaliacao2, avaliacao3);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Avaliacao>>(viewResult.Model).ToList();
            Assert.Equal(3, model.Count);
            Assert.Equal("Terceira (mais recente)", model[0].Comentario);
            Assert.Equal("Segunda", model[1].Comentario);
            Assert.Equal("Primeira", model[2].Comentario);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
