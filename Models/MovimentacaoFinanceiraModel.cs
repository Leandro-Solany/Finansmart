namespace Finansmart.Models

{

    public class MovimentacaoFinanceira
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public decimal Valor { get; set; }
        public int Receita { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
    }
}