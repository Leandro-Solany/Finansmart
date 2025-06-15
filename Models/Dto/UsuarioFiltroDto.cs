namespace Finansmart.Models.Dto
{
    public class UsuarioFiltroDto
    {
        public string? Nome { get; set; } = null;
        public string? Status { get; set; } = null;
        public string? Nivel { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
