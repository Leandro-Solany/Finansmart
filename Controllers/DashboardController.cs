using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Finansmart.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DatabaseContext _context;

        public DashboardController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            // Monte aqui sua ViewModel com os dados agregados necessários
            var viewModel = new DashboardViewModel
            {
                UsuarioId = 1, // Simule/pegue usuário logado
                Saldo = await _context.MovimentacaoFinanceira.SumAsync(m => m.Receita ? m.Valor : -m.Valor),
                Receitas = await _context.MovimentacaoFinanceira.Where(m => m.Receita).SumAsync(m => m.Valor),
                Despesas = await _context.MovimentacaoFinanceira.Where(m => !m.Receita).SumAsync(m => m.Valor),
                CursosConcluidos = await _context.CursoProgresso.CountAsync(cp => cp.PercentualConcluido == 100),
                DesafiosConcluidos = await _context.DesafioUsuarios.CountAsync(d => d.Concluido)
            };
            return View(viewModel);
        }
    }
}
