namespace Finansmart.Models
{
    public class DashboardPeriodoDto
    {
        public decimal SaldoPeriodo { get; set; }
        public int CursosConcluidosPeriodo { get; set; }
        public int DesafiosConcluidosPeriodo { get; set; }
        public int PontuacaoPeriodo { get; set; }
        public List<string> BadgesPeriodo { get; set; }
        public List<string> UltimosCursosPeriodo { get; set; }
        public List<string> UltimosDesafiosPeriodo { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
    }
}
