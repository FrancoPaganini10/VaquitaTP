using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaquita.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCreadorIdAPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreadorId",
                table: "Pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Los pagos existentes se asignan al deudor como creador
         migrationBuilder.Sql("UPDATE Pagos SET CreadorId = DeudorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreadorId",
                table: "Pagos");
        }
    }
}
