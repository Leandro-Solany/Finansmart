namespace Finansmart.Models
{
    public class DesafiosModel;

    public class Desafio
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int Pontuacao { get; set; }
    }

    public class DesafioUsuario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int DesafioId { get; set; }
        public bool Concluido { get; set; }
        public DateTime DataConclusao { get; set; }
        public int PontuacaoRecebida { get; set; }
        public string? BadgeRecebido { get; set; }
    }

    public class ConcluirDesafioDto
    {
        public int UsuarioId { get; set; }
        public int DesafioId { get; set; }
    }
}
