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

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        // Remova o construtor protegido se não estiver usando (boas práticas)
        // protected DatabaseContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CursoProgresso>()
                .HasKey(cp => new { cp.CursoId, cp.UsuarioId });

            modelBuilder.Entity<Curso>()
                .Property(c => c.AvaliacaoMedia)
                .HasPrecision(10, 2);

            modelBuilder.Entity<MovimentacaoFinanceira>()
                .Property(mf => mf.Valor)
                .HasPrecision(12, 2);

            // REMOVA HasConversion<int>() nesses casos:
            // Apenas defina o tipo da coluna, se quiser garantir no banco:
            modelBuilder.Entity<Desafio>()
                .Property(d => d.Ativo)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DesafioUsuario>()
                .Property(du => du.Concluido)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<MovimentacaoFinanceira>()
                .Property(mf => mf.Receita)
                .HasColumnType("NUMBER(1)");

            base.OnModelCreating(modelBuilder);
        }

    }
}
