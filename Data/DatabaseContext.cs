using Finansmart.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Finansmart.Models;

namespace Finansmart.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<CursoProgresso> CursoProgresso { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected DatabaseContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
