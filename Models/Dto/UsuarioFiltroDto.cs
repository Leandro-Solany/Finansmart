namespace Finansmart.Models.Dto
{
    public class UsuarioFiltroDto
    {
        public string? Nome { get; set; }
        public string? Status { get; set; }
        public string? Nivel { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
