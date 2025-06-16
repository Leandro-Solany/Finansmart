using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CursosController : ControllerBase
{
    private readonly DatabaseContext _context;
    public CursosController(DatabaseContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetCursos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var total = await _context.Cursos.CountAsync();
        var cursos = await _context.Cursos
            .OrderBy(c => c.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return Ok(new { page, pageSize, total, items = cursos });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCurso(int id)
    {
        var curso = await _context.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound(new { error = "Curso não encontrado." });
        return Ok(curso);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCurso([FromBody] Curso model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.Cursos.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCurso), new { id = model.Id }, model);
    }
}
