namespace Finansmart.Models
{
    public class DashboardPeriodoDto
    {
        public decimal SaldoPeriodo { get; set; } = new();
        public int CursosConcluidosPeriodo { get; set; } = new();
        public int DesafiosConcluidosPeriodo { get; set; } = new();
        public int PontuacaoPeriodo { get; set; } = new();
        public List<string> BadgesPeriodo { get; set; } 
        public List<string> UltimosCursosPeriodo { get; set; } = new();
        public List<string> UltimosDesafiosPeriodo { get; set; } = new();
        public decimal TotalReceitas { get; set; } = new();
        public decimal TotalDespesas { get; set; } = new();
    }
}
