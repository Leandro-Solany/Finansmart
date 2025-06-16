using Finansmart.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly DatabaseContext _context;
    public DashboardController(DatabaseContext context) => _context = context;

    [HttpGet("relatorio-esg")]
    public async Task<IActionResult> GetRelatorioESG()
    {
        var totalCursos = await _context.Cursos.CountAsync();
        var totalUsuarios = await _context.Usuarios.CountAsync();
        var totalDesafios = await _context.Desafios.CountAsync();
        return Ok(new
        {
            totalCursos,
            totalUsuarios,
            totalDesafios,
            message = "Relatório ESG gerado com sucesso.",
            timestamp = DateTime.UtcNow
        });
    }
}

