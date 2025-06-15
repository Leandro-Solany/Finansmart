
using Finansmart.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finansmart.Api.Controllers
{
    internal class AvaliacaoController : ControllerBase
    {
        [HttpPost("avaliacao")]
        public IActionResult AvaliarCurso([FromBody] AvaliacaoRequest request)
        {
            if (request == null || request.CursoId <= 0 || request.UsuarioId <= 0)
            {
                return BadRequest("Dados inválidos para avaliação.");
            }
            // Aqui você pode adicionar a lógica para salvar a avaliação no banco de dados.
            return Ok("Avaliação registrada com sucesso.");
        }
    }
}