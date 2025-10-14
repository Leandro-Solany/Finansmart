using System;
using System.ComponentModel.DataAnnotations;

namespace Finansmart.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o curso.")]
        public int CursoId { get; set; }

        [Required(ErrorMessage = "Informe o usuário.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Informe a nota.")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        public int Nota { get; set; }

        [StringLength(500, ErrorMessage = "Comentário muito longo (máximo 500 caracteres).")]
        public string? Comentario { get; set; }

        [Display(Name = "Data da Avaliação")]
        public DateTime? DataAvaliacao { get; set; }
    }
}
