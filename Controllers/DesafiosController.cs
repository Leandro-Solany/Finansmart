using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finansmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesafiosController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DesafiosController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/desafios?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetDesafios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Desafios.Where(d => d.Ativo);

            var total = await query.CountAsync();
            var desafios = await query
                .OrderBy(d => d.Titulo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.Id,
                    d.Titulo,
                    d.Descricao,
                    d.Pontuacao
                })
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Desafios = desafios
            });
        }

        // POST: api/desafios/concluir
        [HttpPost("concluir")]
        public async Task<IActionResult> ConcluirDesafio([FromBody] ConcluirDesafioDto dto)
        {
            var desafio = await _context.Desafios.FindAsync(dto.DesafioId);
            if (desafio == null || !desafio.Ativo)
                return NotFound("Desafio não encontrado ou inativo.");

            // Já concluído?
            var registroExistente = await _context.DesafioUsuarios
                .FirstOrDefaultAsync(du => du.UsuarioId == dto.UsuarioId && du.DesafioId == dto.DesafioId);

            if (registroExistente != null && registroExistente.Concluido)
                return BadRequest("Desafio já concluído por este usuário.");

            var badge = desafio.Pontuacao >= 100 ? "Master" : null; // Exemplo simples de badge

            // Cria ou atualiza o registro
            if (registroExistente == null)
            {
                registroExistente = new DesafioUsuario
                {
                    UsuarioId = dto.UsuarioId,
                    DesafioId = dto.DesafioId,
                    Concluido = true,
                    DataConclusao = DateTime.Now,
                    PontuacaoRecebida = desafio.Pontuacao,
                    BadgeRecebido = badge
                };
                _context.DesafioUsuarios.Add(registroExistente);
            }
            else
            {
                registroExistente.Concluido = true;
                registroExistente.DataConclusao = DateTime.Now;
                registroExistente.PontuacaoRecebida = desafio.Pontuacao;
                registroExistente.BadgeRecebido = badge;
                _context.DesafioUsuarios.Update(registroExistente);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = "Desafio concluído com sucesso!",
                BadgeRecebido = badge,
                Pontuacao = desafio.Pontuacao
            });
        }
    }
}
