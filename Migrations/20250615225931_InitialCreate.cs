using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finansmart.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CursoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Nota = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Comentario = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    DataAvaliacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CursoProgresso",
                columns: table => new
                {
                    CursoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PercentualConcluido = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ModulosCompletos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Badges = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursoProgresso", x => new { x.CursoId, x.UsuarioId });
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AvaliacaoMedia = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Desafios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Titulo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ativo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Pontuacao = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desafios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesafioUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DesafioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Concluido = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PontuacaoRecebida = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    BadgeRecebido = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesafioUsuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacaoFinanceira",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Valor = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    Receita = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Data = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacaoFinanceira", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Nivel = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "CursoProgresso");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Desafios");

            migrationBuilder.DropTable(
                name: "DesafioUsuarios");

            migrationBuilder.DropTable(
                name: "MovimentacaoFinanceira");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
