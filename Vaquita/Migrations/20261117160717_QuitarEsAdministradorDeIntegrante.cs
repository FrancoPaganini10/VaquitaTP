using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaquita.Migrations
{
    /// <inheritdoc />
    public partial class QuitarEsAdministradorDeIntegrante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsAdministrador",
                table: "Integrantes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsAdministrador",
                table: "Integrantes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
