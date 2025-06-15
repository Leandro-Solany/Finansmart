using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finansmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UsuariosController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios([FromQuery] string? nome = null, [FromQuery] string? status = null, [FromQuery] string? nivel = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(u => u.Nome.Contains(nome));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(u => u.Status == status);
            if (!string.IsNullOrWhiteSpace(nivel))
                query = query.Where(u => u.Nivel == nivel);

            var total = await query.CountAsync();
            var usuarios = await query
                .OrderBy(u => u.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Status,
                    u.Nivel,
                    u.DataCadastro
                })
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Usuarios = usuarios
            });
        }

        // GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] Usuario usuario)
        {
            usuario.DataCadastro = DateTime.Now;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] Usuario usuarioAtualizado)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Nome = usuarioAtualizado.Nome;
            usuario.Email = usuarioAtualizado.Email;
            usuario.Status = usuarioAtualizado.Status;
            usuario.Nivel = usuarioAtualizado.Nivel;
            // Não atualize DataCadastro aqui

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/usuarios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario); // Ou, para soft delete: usuario.Status = "Inativo";
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
