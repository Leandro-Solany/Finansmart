using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finansmart.Migrations
{
    /// <inheritdoc />
    public partial class AjusteBoolOracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "MovimentacaoFinanceira",
                type: "DECIMAL(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Receita",
                table: "MovimentacaoFinanceira",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<int>(
                name: "Concluido",
                table: "DesafioUsuarios",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<int>(
                name: "Ativo",
                table: "Desafios",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AvaliacaoMedia",
                table: "Cursos",
                type: "DECIMAL(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "MovimentacaoFinanceira",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "Receita",
                table: "MovimentacaoFinanceira",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(1)");

            migrationBuilder.AlterColumn<int>(
                name: "Concluido",
                table: "DesafioUsuarios",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(1)");

            migrationBuilder.AlterColumn<int>(
                name: "Ativo",
                table: "Desafios",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AvaliacaoMedia",
                table: "Cursos",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldPrecision: 10,
                oldScale: 2);
        }
    }
}
