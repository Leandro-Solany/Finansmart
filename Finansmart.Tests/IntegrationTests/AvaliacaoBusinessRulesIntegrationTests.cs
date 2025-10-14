using Finansmart.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Finansmart.Tests.IntegrationTests
{
    public class AvaliacaoBusinessRulesIntegrationTests : BaseIntegrationTest
    {
        protected override void SeedDatabase()
        {
            var curso = new Curso
            {
                Id = 1,
                Nome = "Finanças Pessoais Avançado",
                Descricao = "Curso avançado de finanças",
                AvaliacaoMedia = 0
            };

            var usuarios = new[]
            {
                new Usuario { Id = 1, Nome = "Usuario 1", Email = "user1@test.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now },
                new Usuario { Id = 2, Nome = "Usuario 2", Email = "user2@test.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now },
                new Usuario { Id = 3, Nome = "Usuario 3", Email = "user3@test.com", Status = "Ativo", Nivel = "Aluno", DataCadastro = DateTime.Now }
            };

            Context.Cursos.Add(curso);
            Context.Usuarios.AddRange(usuarios);
            Context.SaveChanges();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task AvaliacaoNota_MustBeBetween1And5(int nota)
        {
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = nota,
                Comentario = $"Nota {nota}",
                DataAvaliacao = DateTime.Now
            };

            Context.Avaliacoes.Add(avaliacao);
            await Context.SaveChangesAsync();

            var saved = await Context.Avaliacoes.FirstOrDefaultAsync(a => a.Nota == nota);
            Assert.NotNull(saved);
            Assert.InRange(saved.Nota, 1, 5);
        }

        [Fact]
        public async Task CalculateAverageRating_ForCursoWithMultipleAvaliacoes()
        {
            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = 1, UsuarioId = 1, Nota = 5, Comentario = "Excelente", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 1, UsuarioId = 2, Nota = 4, Comentario = "Bom", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 1, UsuarioId = 3, Nota = 3, Comentario = "Regular", DataAvaliacao = DateTime.Now }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var avaliacoesDoCurso = await Context.Avaliacoes
                .Where(a => a.CursoId == 1)
                .ToListAsync();

            var mediaCalculada = avaliacoesDoCurso.Average(a => a.Nota);

            Assert.Equal(4.0, mediaCalculada);
            Assert.Equal(3, avaliacoesDoCurso.Count);
        }

        [Fact]
        public async Task CanFilterAvaliacoesByNotaMinima()
        {
            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = 1, UsuarioId = 1, Nota = 5, Comentario = "Excelente", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 1, UsuarioId = 2, Nota = 4, Comentario = "Bom", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 1, UsuarioId = 3, Nota = 2, Comentario = "Ruim", DataAvaliacao = DateTime.Now }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var avaliacoesPositivas = await Context.Avaliacoes
                .Where(a => a.CursoId == 1 && a.Nota >= 4)
                .ToListAsync();

            Assert.Equal(2, avaliacoesPositivas.Count);
            Assert.All(avaliacoesPositivas, a => Assert.True(a.Nota >= 4));
        }

        [Fact]
        public async Task CanGetMostRecentAvaliacoes()
        {
            var baseDate = DateTime.Now;
            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = 1, UsuarioId = 1, Nota = 5, Comentario = "Primeira", DataAvaliacao = baseDate.AddDays(-10) },
                new Avaliacao { CursoId = 1, UsuarioId = 2, Nota = 4, Comentario = "Segunda", DataAvaliacao = baseDate.AddDays(-5) },
                new Avaliacao { CursoId = 1, UsuarioId = 3, Nota = 3, Comentario = "Terceira", DataAvaliacao = baseDate.AddDays(-1) }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var recentAvaliacoes = await Context.Avaliacoes
                .Where(a => a.CursoId == 1)
                .OrderByDescending(a => a.DataAvaliacao)
                .Take(2)
                .ToListAsync();

            Assert.Equal(2, recentAvaliacoes.Count);
            Assert.Equal(3, recentAvaliacoes[0].Nota);
            Assert.Equal(4, recentAvaliacoes[1].Nota);
        }

        [Fact]
        public async Task ComentarioMaxLength_ShouldBe500Characters()
        {
            var comentarioLongo = new string('A', 500);
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 5,
                Comentario = comentarioLongo,
                DataAvaliacao = DateTime.Now
            };

            Context.Avaliacoes.Add(avaliacao);
            await Context.SaveChangesAsync();

            var saved = await Context.Avaliacoes.FirstAsync();

            Assert.Equal(500, saved.Comentario.Length);
        }

        [Fact]
        public async Task CanGetAvaliacoesByDateRange()
        {
            var baseDate = DateTime.Now;
            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = 1, UsuarioId = 1, Nota = 5, Comentario = "Primeira", DataAvaliacao = baseDate.AddDays(-30) },
                new Avaliacao { CursoId = 1, UsuarioId = 2, Nota = 4, Comentario = "Segunda", DataAvaliacao = baseDate.AddDays(-15) },
                new Avaliacao { CursoId = 1, UsuarioId = 3, Nota = 3, Comentario = "Terceira", DataAvaliacao = baseDate.AddDays(-5) }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var dataInicio = baseDate.AddDays(-20);
            var avaliacoesRecentes = await Context.Avaliacoes
                .Where(a => a.CursoId == 1 && a.DataAvaliacao >= dataInicio)
                .ToListAsync();

            Assert.Equal(2, avaliacoesRecentes.Count);
        }

        [Fact]
        public async Task CanCountAvaliacoesPorNota()
        {
            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = 1, UsuarioId = 1, Nota = 5, Comentario = "Excelente 1", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 1, UsuarioId = 2, Nota = 5, Comentario = "Excelente 2", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 1, UsuarioId = 3, Nota = 4, Comentario = "Bom", DataAvaliacao = DateTime.Now }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var notasCinco = await Context.Avaliacoes
                .Where(a => a.CursoId == 1 && a.Nota == 5)
                .CountAsync();

            var notasQuatro = await Context.Avaliacoes
                .Where(a => a.CursoId == 1 && a.Nota == 4)
                .CountAsync();

            Assert.Equal(2, notasCinco);
            Assert.Equal(1, notasQuatro);
        }

        [Fact]
        public async Task UsuarioCanHaveMultipleAvaliacoesForDifferentCursos()
        {
            var curso2 = new Curso
            {
                Id = 2,
                Nome = "Investimentos",
                Descricao = "Curso de investimentos",
                AvaliacaoMedia = 0
            };

            Context.Cursos.Add(curso2);
            await Context.SaveChangesAsync();

            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = 1, UsuarioId = 1, Nota = 5, Comentario = "Excelente curso 1", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = 2, UsuarioId = 1, Nota = 4, Comentario = "Bom curso 2", DataAvaliacao = DateTime.Now }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var avaliacoesUsuario = await Context.Avaliacoes
                .Where(a => a.UsuarioId == 1)
                .ToListAsync();

            Assert.Equal(2, avaliacoesUsuario.Count);
            Assert.Contains(avaliacoesUsuario, a => a.CursoId == 1);
            Assert.Contains(avaliacoesUsuario, a => a.CursoId == 2);
        }
    }
}
