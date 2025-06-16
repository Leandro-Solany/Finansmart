using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class DashboardController : Controller
{
    private readonly DatabaseContext _context;

    public DashboardController(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            UsuarioId = 1, // Simule/pegue usuário logado
            Saldo = await _context.MovimentacaoFinanceira.SumAsync(m => m.Receita == 1 ? m.Valor : -m.Valor),
            Receitas = await _context.MovimentacaoFinanceira.Where(m => m.Receita == 1).SumAsync(m => m.Valor),
            Despesas = await _context.MovimentacaoFinanceira.Where(m => m.Receita == 0).SumAsync(m => m.Valor),
            CursosConcluidos = await _context.CursoProgresso.CountAsync(cp => cp.PercentualConcluido == 100),
            DesafiosConcluidos = await _context.Desafios.CountAsync(d => d.Ativo == 1) // Só se seu Desafio.Ativo for int!
        };
        return View(viewModel);
    }
}
