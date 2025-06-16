using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DesafiosController : ControllerBase
{
    private readonly DatabaseContext _context;
    public DesafiosController(DatabaseContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetDesafios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var total = await _context.Desafios.CountAsync();
        var desafios = await _context.Desafios
            .OrderBy(d => d.Titulo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return Ok(new { page, pageSize, total, items = desafios });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDesafio(int id)
    {
        var desafio = await _context.Desafios.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (desafio == null) return NotFound(new { error = "Desafio não encontrado." });
        return Ok(desafio);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDesafio([FromBody] Desafio model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.Desafios.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDesafio), new { id = model.Id }, model);
    }
}
