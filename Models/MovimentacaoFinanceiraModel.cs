namespace Finansmart.Models

{

    public class MovimentacaoFinanceira
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public decimal Valor { get; set; }
        public bool Receita { get; set; } // true = receita, false = despesa
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
    }
}