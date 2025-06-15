
namespace Finansmart.Models
{
    public class CursoProgressoModel;

        public class CursoProgresso
    {
        public int CursoId { get; set; }
        public int UsuarioId { get; set; }
        public double PercentualConcluido { get; set; }
        public string ModulosCompletos { get; set; }
        public string Badges { get; set; }
        public DateTime? DataConclusao { get; internal set; }
    }
}