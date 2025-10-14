using Finansmart.Data;
using Finansmart.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Finansmart.Tests.IntegrationTests
{
    public class DatabaseContextIntegrationTests : BaseIntegrationTest
    {
        [Fact]
        public async Task CanAddAndRetrieveCurso()
        {
            var curso = new Curso
            {
                Nome = "Investimentos para Iniciantes",
                Descricao = "Aprenda os conceitos básicos de investimentos",
                AvaliacaoMedia = 4.5m
            };

            Context.Cursos.Add(curso);
            await Context.SaveChangesAsync();

            var retrievedCurso = await Context.Cursos.FirstOrDefaultAsync(c => c.Nome == curso.Nome);

            Assert.NotNull(retrievedCurso);
            Assert.Equal(curso.Nome, retrievedCurso.Nome);
            Assert.Equal(curso.Descricao, retrievedCurso.Descricao);
            Assert.Equal(curso.AvaliacaoMedia, retrievedCurso.AvaliacaoMedia);
        }

        [Fact]
        public async Task CanAddAndRetrieveUsuario()
        {
            var usuario = new Usuario
            {
                Nome = "João Silva",
                Email = "joao.silva@finansmart.com",
                Status = "Ativo",
                Nivel = "Aluno",
                DataCadastro = DateTime.Now
            };

            Context.Usuarios.Add(usuario);
            await Context.SaveChangesAsync();

            var retrievedUsuario = await Context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);

            Assert.NotNull(retrievedUsuario);
            Assert.Equal(usuario.Nome, retrievedUsuario.Nome);
            Assert.Equal(usuario.Email, retrievedUsuario.Email);
            Assert.Equal(usuario.Status, retrievedUsuario.Status);
            Assert.Equal(usuario.Nivel, retrievedUsuario.Nivel);
        }

        [Fact]
        public async Task CanCreateRelationshipBetweenAvaliacaoAndCurso()
        {
            var curso = new Curso
            {
                Nome = "Educação Financeira",
                Descricao = "Curso completo de finanças",
                AvaliacaoMedia = 0
            };

            var usuario = new Usuario
            {
                Nome = "Maria Santos",
                Email = "maria@finansmart.com",
                Status = "Ativo",
                Nivel = "Aluno",
                DataCadastro = DateTime.Now
            };

            Context.Cursos.Add(curso);
            Context.Usuarios.Add(usuario);
            await Context.SaveChangesAsync();

            var avaliacao = new Avaliacao
            {
                CursoId = curso.Id,
                UsuarioId = usuario.Id,
                Nota = 5,
                Comentario = "Excelente curso!",
                DataAvaliacao = DateTime.Now
            };

            Context.Avaliacoes.Add(avaliacao);
            await Context.SaveChangesAsync();

            var retrievedAvaliacao = await Context.Avaliacoes
                .FirstOrDefaultAsync(a => a.CursoId == curso.Id);

            Assert.NotNull(retrievedAvaliacao);
            Assert.Equal(curso.Id, retrievedAvaliacao.CursoId);
            Assert.Equal(usuario.Id, retrievedAvaliacao.UsuarioId);
            Assert.Equal(5, retrievedAvaliacao.Nota);
        }

        [Fact]
        public async Task CanAddMultipleAvaliacoesForSameCurso()
        {
            var curso = new Curso
            {
                Nome = "Planejamento Financeiro",
                Descricao = "Aprenda a planejar suas finanças",
                AvaliacaoMedia = 0
            };

            Context.Cursos.Add(curso);
            await Context.SaveChangesAsync();

            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = curso.Id, UsuarioId = 1, Nota = 5, Comentario = "Ótimo!", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = curso.Id, UsuarioId = 2, Nota = 4, Comentario = "Muito bom", DataAvaliacao = DateTime.Now },
                new Avaliacao { CursoId = curso.Id, UsuarioId = 3, Nota = 5, Comentario = "Recomendo!", DataAvaliacao = DateTime.Now }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var retrievedAvaliacoes = await Context.Avaliacoes
                .Where(a => a.CursoId == curso.Id)
                .ToListAsync();

            Assert.Equal(3, retrievedAvaliacoes.Count);
            Assert.All(retrievedAvaliacoes, a => Assert.Equal(curso.Id, a.CursoId));
        }

        [Fact]
        public async Task CanUpdateAvaliacao()
        {
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 3,
                Comentario = "Razoável",
                DataAvaliacao = DateTime.Now
            };

            Context.Avaliacoes.Add(avaliacao);
            await Context.SaveChangesAsync();

            avaliacao.Nota = 5;
            avaliacao.Comentario = "Excelente após atualização!";
            await Context.SaveChangesAsync();

            var updatedAvaliacao = await Context.Avaliacoes.FindAsync(avaliacao.Id);

            Assert.NotNull(updatedAvaliacao);
            Assert.Equal(5, updatedAvaliacao.Nota);
            Assert.Equal("Excelente após atualização!", updatedAvaliacao.Comentario);
        }

        [Fact]
        public async Task CanDeleteAvaliacao()
        {
            var avaliacao = new Avaliacao
            {
                CursoId = 1,
                UsuarioId = 1,
                Nota = 4,
                Comentario = "Bom",
                DataAvaliacao = DateTime.Now
            };

            Context.Avaliacoes.Add(avaliacao);
            await Context.SaveChangesAsync();
            var avaliacaoId = avaliacao.Id;

            Context.Avaliacoes.Remove(avaliacao);
            await Context.SaveChangesAsync();

            var deletedAvaliacao = await Context.Avaliacoes.FindAsync(avaliacaoId);

            Assert.Null(deletedAvaliacao);
        }

        [Fact]
        public async Task CanQueryAvaliacoesOrderedByDate()
        {
            var cursoId = 1;
            var avaliacoes = new[]
            {
                new Avaliacao { CursoId = cursoId, UsuarioId = 1, Nota = 5, Comentario = "Primeira", DataAvaliacao = DateTime.Now.AddDays(-5) },
                new Avaliacao { CursoId = cursoId, UsuarioId = 2, Nota = 4, Comentario = "Segunda", DataAvaliacao = DateTime.Now.AddDays(-2) },
                new Avaliacao { CursoId = cursoId, UsuarioId = 3, Nota = 3, Comentario = "Terceira", DataAvaliacao = DateTime.Now }
            };

            Context.Avaliacoes.AddRange(avaliacoes);
            await Context.SaveChangesAsync();

            var orderedAvaliacoes = await Context.Avaliacoes
                .Where(a => a.CursoId == cursoId)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            Assert.Equal(3, orderedAvaliacoes.Count);
            Assert.True(orderedAvaliacoes[0].DataAvaliacao >= orderedAvaliacoes[1].DataAvaliacao);
            Assert.True(orderedAvaliacoes[1].DataAvaliacao >= orderedAvaliacoes[2].DataAvaliacao);
        }

        [Fact]
        public async Task CanAddMovimentacaoFinanceira()
        {
            var movimentacao = new MovimentacaoFinanceira
            {
                UsuarioId = 1,
                Valor = 1500.00m,
                Receita = 1,
                Data = DateTime.Now,
                Descricao = "Salário"
            };

            Context.MovimentacaoFinanceira.Add(movimentacao);
            await Context.SaveChangesAsync();

            var retrievedMovimentacao = await Context.MovimentacaoFinanceira
                .FirstOrDefaultAsync(m => m.UsuarioId == movimentacao.UsuarioId);

            Assert.NotNull(retrievedMovimentacao);
            Assert.Equal(1500.00m, retrievedMovimentacao.Valor);
            Assert.Equal("Salário", retrievedMovimentacao.Descricao);
        }
    }
}
