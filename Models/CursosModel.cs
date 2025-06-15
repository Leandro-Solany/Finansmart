using System.Collections.Generic;

namespace Finansmart.Models
{
    public class CursosListViewModel
    {
        public List<Curso> Cursos { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
