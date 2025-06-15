using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finansmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DashboardController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/{usuarioId}?mes=6&ano=2025
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetDashboardPeriodo(
            int usuarioId, 
            [FromQuery] int? mes = null, 
            [FromQuery] int? ano = null)
        {
            // Define o período de consulta
            DateTime? inicio = null, fim = null;
            if (mes.HasValue && ano.HasValue)
            {
                inicio = new DateTime(ano.Value, mes.Value, 1);
                fim = inicio.Value.AddMonths(1);
            }
            else if (ano.HasValue)
            {
                inicio = new DateTime(ano.Value, 1, 1);
                fim = inicio.Value.AddYears(1);
            }

            // 1. Receitas e despesas no período
            var queryMovimentacoes = _context.MovimentacaoFinanceira
                .Where(m => m.UsuarioId == usuarioId);
            if (inicio.HasValue)
                queryMovimentacoes = queryMovimentacoes.Where(m => m.Data >= inicio && m.Data < fim);

            decimal totalReceitas = await queryMovimentacoes
                .Where(m => m.Receita)
                .SumAsync(m => (decimal?)m.Valor) ?? 0;

            decimal totalDespesas = await queryMovimentacoes
                .Where(m => !m.Receita)
                .SumAsync(m => (decimal?)m.Valor) ?? 0;

            decimal saldoPeriodo = totalReceitas - totalDespesas;

            // 2. Cursos concluídos no período
            var queryCurso = _context.CursoProgresso
                .Where(cp => cp.UsuarioId == usuarioId && cp.PercentualConcluido >= 100);
            if (inicio.HasValue)
                queryCurso = queryCurso.Where(cp => cp.DataConclusao >= inicio && cp.DataConclusao < fim);
            int cursosConcluidosPeriodo = await queryCurso.CountAsync();

            // 3. Desafios concluídos e pontuação no período
            var queryDesafio = _context.DesafioUsuarios
                .Where(du => du.UsuarioId == usuarioId && du.Concluido);
            if (inicio.HasValue)
                queryDesafio = queryDesafio.Where(du => du.DataConclusao >= inicio && du.DataConclusao < fim);
            var desafiosConcluidos = await queryDesafio.ToListAsync();

            int desafiosConcluidosPeriodo = desafiosConcluidos.Count;
            int pontuacaoPeriodo = desafiosConcluidos.Sum(du => du.PontuacaoRecebida);

            var badgesPeriodo = desafiosConcluidos
                .Where(du => !string.IsNullOrEmpty(du.BadgeRecebido))
                .Select(du => du.BadgeRecebido)
                .Distinct()
                .ToList();

            // 4. Últimos cursos concluídos no período (nomes)
            var ultimosCursosPeriodo = await queryCurso
                .OrderByDescending(cp => cp.DataConclusao)
                .Take(3)
                .Join(_context.Cursos,
                    cp => cp.CursoId,
                    c => c.Id,
                    (cp, c) => c.Nome)
                .ToListAsync();

            // 5. Últimos desafios concluídos no período (títulos)
            var ultimosDesafiosPeriodo = await queryDesafio
                .OrderByDescending(du => du.DataConclusao)
                .Take(3)
                .Join(_context.Desafios,
                    du => du.DesafioId,
                    d => d.Id,
                    (du, d) => d.Titulo)
                .ToListAsync();

            var dashboardPeriodo = new DashboardPeriodoDto
            {
                SaldoPeriodo = saldoPeriodo,
                CursosConcluidosPeriodo = cursosConcluidosPeriodo,
                DesafiosConcluidosPeriodo = desafiosConcluidosPeriodo,
                PontuacaoPeriodo = pontuacaoPeriodo,
                BadgesPeriodo = badgesPeriodo,
                UltimosCursosPeriodo = ultimosCursosPeriodo,
                UltimosDesafiosPeriodo = ultimosDesafiosPeriodo,
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas
            };

            return Ok(dashboardPeriodo);
        }
    }
}
