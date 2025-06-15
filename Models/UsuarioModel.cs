namespace Finansmart.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } // "Ativo", "Inativo", etc
        public string Nivel { get; set; } // "Aluno", "Mentor", "Administrador"
        public DateTime? DataCadastro { get; set; }
    }
}
