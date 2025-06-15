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
        public DbSet<Desafio> Desafios { get; set; }
        public DbSet<DesafioUsuario> DesafioUsuarios { get; set; }
        public DbSet<MovimentacaoFinanceira> MovimentacaoFinanceira { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

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
