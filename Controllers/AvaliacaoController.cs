using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finansmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvaliacaoController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AvaliacaoController(DatabaseContext context)
        {
            _context = context;
        }

        // POST: api/avaliacao/{cursoId}
        [HttpPost("{cursoId}")]
        public async Task<IActionResult> AvaliarCurso(int cursoId, [FromBody] AvaliacaoCursoDto request)
        {
            if (request == null || cursoId <= 0 || request.UsuarioId <= 0)
                return BadRequest("Dados inválidos para avaliação.");

            // Verifica se o curso existe
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
                return NotFound("Curso não encontrado.");

            // Verifica se o usuário já avaliou o curso
            var avaliacaoExistente = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.CursoId == cursoId && a.UsuarioId == request.UsuarioId);

            if (avaliacaoExistente != null)
                return BadRequest("Usuário já avaliou este curso.");

            // Cria a avaliação
            var avaliacao = new Avaliacao
            {
                CursoId = cursoId,
                UsuarioId = request.UsuarioId,
                Nota = request.Nota,
                Comentario = request.Comentario,
                DataAvaliacao = DateTime.Now
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            return Ok("Avaliação registrada com sucesso!");
        }

        // GET: api/avaliacao/{cursoId}?page=1&pageSize=10
        [HttpGet("{cursoId}")]
        public async Task<IActionResult> GetAvaliacoes(int cursoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Avaliacoes.Where(a => a.CursoId == cursoId);

            var total = await query.CountAsync();
            var avaliacoes = await query
                .OrderByDescending(a => a.DataAvaliacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Avaliacoes = avaliacoes
            });
        }
    }
}
