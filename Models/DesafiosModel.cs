namespace Finansmart.Models
{
    public class Desafio
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int Ativo { get; set; }
        public int Pontuacao { get; set; }
        public virtual ICollection<DesafioUsuario> DesafioUsuarios { get; set; }
    }

    public class DesafioUsuario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int DesafioId { get; set; }
        public int Concluido { get; set; }
        public DateTime DataConclusao { get; set; }
        public int PontuacaoRecebida { get; set; }
        public string? BadgeRecebido { get; set; }
        public virtual Desafio Desafio { get; set; }
        public virtual Usuario Usuario { get; set; }
    }

    public class ConcluirDesafioDto
    {
        public int UsuarioId { get; set; }
        public int DesafioId { get; set; }
    }
}
