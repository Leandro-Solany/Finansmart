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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chave composta para CursoProgresso
            modelBuilder.Entity<CursoProgresso>()
                .HasKey(cp => new { cp.CursoId, cp.UsuarioId });

            // Precisão para AvaliacaoMedia do Curso
            modelBuilder.Entity<Curso>()
                .Property(c => c.AvaliacaoMedia)
                .HasPrecision(10, 2);

            // Precisão para Valor em MovimentacaoFinanceira
            modelBuilder.Entity<MovimentacaoFinanceira>()
                .Property(mf => mf.Valor)
                .HasPrecision(12, 2);

            // Campos booleanos modelados como int (0/1) - garante o tipo no banco
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
