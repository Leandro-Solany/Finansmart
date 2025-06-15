namespace Finansmart.Api.Models
{
    public class AvaliacaoRequestDto;
        public class AvaliacaoRequest

    {
        public int CursoId { get; set; }
        public int UsuarioId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; }
    }
}
