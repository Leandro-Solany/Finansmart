using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly DatabaseContext _context;
    public UsuariosController(DatabaseContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetUsuarios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var total = await _context.Usuarios.CountAsync();
        var usuarios = await _context.Usuarios
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return Ok(new { page, pageSize, total, items = usuarios });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null) return NotFound(new { error = "Usuário não encontrado." });
        return Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUsuario([FromBody] Usuario model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        model.DataCadastro = DateTime.Now;
        _context.Usuarios.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsuario), new { id = model.Id }, model);
    }
}
