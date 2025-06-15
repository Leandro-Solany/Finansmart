using Finansmart.Api.Controllers;
using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Finansmart.Tests
{
    public class DashboardControllerTests
    {
        private DbContextOptions<DatabaseContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetDashboardPeriodo_ReturnsDashboardData()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.MovimentacaoFinanceira.Add(new MovimentacaoFinanceira
                {
                    UsuarioId = 10,
                    Valor = 500,
                    Receita = true,
                    Data = new DateTime(2025, 6, 10),
                    Descricao = "Salário"
                });
                context.MovimentacaoFinanceira.Add(new MovimentacaoFinanceira
                {
                    UsuarioId = 10,
                    Valor = 100,
                    Receita = false,
                    Data = new DateTime(2025, 6, 15),
                    Descricao = "Compra"
                });

                context.CursoProgresso.Add(new CursoProgresso
                {
                    CursoId = 1,
                    UsuarioId = 10,
                    PercentualConcluido = 100,
                    DataConclusao = new DateTime(2025, 6, 12),
                    ModulosCompletos = "1;2;3",
                    Badges = "Badge1"
                });

                context.DesafioUsuarios.Add(new DesafioUsuario
                {
                    UsuarioId = 10,
                    DesafioId = 1,
                    Concluido = true,
                    DataConclusao = new DateTime(2025, 6, 13),
                    PontuacaoRecebida = 60,
                    BadgeRecebido = "Master"
                });

                context.Cursos.Add(new Curso { Id = 1, Nome = "Finanças Pessoais", Descricao = "Curso", AvaliacaoMedia = 5.0M });
                context.Desafios.Add(new Desafio { Id = 1, Titulo = "Desafio 1", Ativo = true, Pontuacao = 60 });

                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DashboardController(context);

                // Act
                var result = await controller.GetDashboardPeriodo(10, 6, 2025);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var dashboard = Assert.IsType<DashboardPeriodoDto>(okResult.Value);

                Assert.Equal(400, dashboard.SaldoPeriodo); // 500 receita - 100 despesa
                Assert.Equal(500, dashboard.TotalReceitas);
                Assert.Equal(100, dashboard.TotalDespesas);
                Assert.Equal(1, dashboard.CursosConcluidosPeriodo);
                Assert.Equal(1, dashboard.DesafiosConcluidosPeriodo);
                Assert.Equal(60, dashboard.PontuacaoPeriodo);
                Assert.Contains("Badge1", dashboard.BadgesPeriodo);
                Assert.Contains("Master", dashboard.BadgesPeriodo);
                Assert.Contains("Finanças Pessoais", dashboard.UltimosCursosPeriodo);
                Assert.Contains("Desafio 1", dashboard.UltimosDesafiosPeriodo);
            }
        }

        [Fact]
        public async Task GetDashboardPeriodo_ReturnsEmptyDashboard_WhenNoData()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DashboardController(context);

                // Act
                var result = await controller.GetDashboardPeriodo(99, 1, 2025);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var dashboard = Assert.IsType<DashboardPeriodoDto>(okResult.Value);

                Assert.Equal(0, dashboard.SaldoPeriodo);
                Assert.Equal(0, dashboard.TotalReceitas);
                Assert.Equal(0, dashboard.TotalDespesas);
                Assert.Equal(0, dashboard.CursosConcluidosPeriodo);
                Assert.Equal(0, dashboard.DesafiosConcluidosPeriodo);
                Assert.Equal(0, dashboard.PontuacaoPeriodo);
                Assert.Empty(dashboard.BadgesPeriodo);
                Assert.Empty(dashboard.UltimosCursosPeriodo);
                Assert.Empty(dashboard.UltimosDesafiosPeriodo);
            }
        }

        [Fact]
        public async Task GetDashboardPeriodo_ReturnsDataForSpecifiedPeriod()
        {
            // Arrange
            var options = GetDbOptions();
            using (var context = new DatabaseContext(options))
            {
                // Receita e despesa em meses diferentes
                context.MovimentacaoFinanceira.Add(new MovimentacaoFinanceira
                {
                    UsuarioId = 11,
                    Valor = 200,
                    Receita = true,
                    Data = new DateTime(2025, 6, 2),
                    Descricao = "Extra"
                });
                context.MovimentacaoFinanceira.Add(new MovimentacaoFinanceira
                {
                    UsuarioId = 11,
                    Valor = 50,
                    Receita = false,
                    Data = new DateTime(2025, 5, 28),
                    Descricao = "Compra maio"
                });

                // Curso concluído em junho
                context.CursoProgresso.Add(new CursoProgresso
                {
                    CursoId = 2,
                    UsuarioId = 11,
                    PercentualConcluido = 100,
                    DataConclusao = new DateTime(2025, 6, 10),
                    ModulosCompletos = "1;2",
                    Badges = ""
                });

                // Curso concluído em maio (não deve contar para junho)
                context.CursoProgresso.Add(new CursoProgresso
                {
                    CursoId = 3,
                    UsuarioId = 11,
                    PercentualConcluido = 100,
                    DataConclusao = new DateTime(2025, 5, 20),
                    ModulosCompletos = "",
                    Badges = ""
                });

                context.Cursos.Add(new Curso { Id = 2, Nome = "Investimentos", Descricao = "Curso", AvaliacaoMedia = 4.8M });
                context.Cursos.Add(new Curso { Id = 3, Nome = "Economia Básica", Descricao = "Curso", AvaliacaoMedia = 4.2M });

                context.SaveChanges();
            }

            using (var context = new DatabaseContext(options))
            {
                var controller = new DashboardController(context);

                // Act
                var result = await controller.GetDashboardPeriodo(11, 6, 2025);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var dashboard = Assert.IsType<DashboardPeriodoDto>(okResult.Value);

                Assert.Equal(200, dashboard.SaldoPeriodo);
                Assert.Equal(200, dashboard.TotalReceitas);
                Assert.Equal(0, dashboard.TotalDespesas);
                Assert.Equal(1, dashboard.CursosConcluidosPeriodo); // Só 1 curso em junho
                Assert.Contains("Investimentos", dashboard.UltimosCursosPeriodo);
                Assert.DoesNotContain("Economia Básica", dashboard.UltimosCursosPeriodo);
            }
        }
    }
}
