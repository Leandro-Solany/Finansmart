namespace Finansmart.Models
{
    public class DashboardViewModel
    {
        public int UsuarioId { get; set; }
        public decimal Saldo { get; set; }
        public decimal Receitas { get; set; }
        public decimal Despesas { get; set; }
        public int CursosConcluidos { get; set; }
        public int DesafiosConcluidos { get; set; }
    }
}
