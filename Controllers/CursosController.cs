using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finansmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CursosController(DatabaseContext context)
        {
            _context = context;
        }

        // Listar cursos com paginação
        [HttpGet]
        public async Task<IActionResult> GetCursos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Cursos.AsQueryable();

            var total = await query.CountAsync();
            var cursos = await query
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Nome,
                    c.Descricao,
                    c.AvaliacaoMedia
                })
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Cursos = cursos
            });
        }

        // Progresso do usuário no curso + recomendações
        [HttpGet("{cursoId}/progresso/{usuarioId}")]
        public async Task<IActionResult> GetProgresso(int cursoId, int usuarioId)
        {
            var progressoEntity = await _context.CursoProgresso
                .FirstOrDefaultAsync(p => p.CursoId == cursoId && p.UsuarioId == usuarioId);

            if (progressoEntity == null)
                return NotFound();

            var modulosCompletos = (progressoEntity.ModulosCompletos ?? "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var badges = (progressoEntity.Badges ?? "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var recomendacoes = await _context.Cursos
                .Where(c => c.Id != cursoId)
                .OrderByDescending(c => c.AvaliacaoMedia)
                .Take(3)
                .Select(c => c.Nome)
                .ToListAsync();

            var progresso = new
            {
                progressoEntity.CursoId,
                progressoEntity.UsuarioId,
                progressoEntity.PercentualConcluido,
                ModulosCompletos = modulosCompletos,
                Badges = badges,
                Recomendacoes = recomendacoes
            };

            return Ok(progresso);
        }

        // Avaliação de curso
        [HttpPost("{cursoId}/avaliar")]
        public async Task<IActionResult> AvaliarCurso(int cursoId, [FromBody] AvaliacaoCursoDto dto)
        {
            // Verifica se o curso existe antes de tudo
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
                return NotFound("Curso não encontrado.");

            // Verifica se o usuário já avaliou este curso
            var avaliacaoExistente = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.CursoId == cursoId && a.UsuarioId == dto.UsuarioId);

            if (avaliacaoExistente != null)
                return BadRequest("Usuário já avaliou este curso.");

            // Cria e salva avaliação
            var avaliacao = new Avaliacao
            {
                CursoId = cursoId,
                UsuarioId = dto.UsuarioId,
                Nota = dto.Nota,
                Comentario = dto.Comentario ?? "",
                DataAvaliacao = DateTime.Now
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            // Recalcula a média REAL depois de salvar a avaliação
            var avaliacoes = await _context.Avaliacoes
                .Where(a => a.CursoId == cursoId)
                .ToListAsync();

            curso.AvaliacaoMedia = (decimal)avaliacoes.Average(a => a.Nota);

            await _context.SaveChangesAsync();

            return Ok("Avaliação registrada com sucesso!");
        }
    }
}
