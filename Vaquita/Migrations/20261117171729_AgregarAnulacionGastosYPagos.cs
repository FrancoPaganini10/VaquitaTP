using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaquita.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAnulacionGastosYPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "Pagos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AnuladoPorId",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAnulacion",
                table: "Pagos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "Gastos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AnuladoPorId",
                table: "Gastos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAnulacion",
                table: "Gastos",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "AnuladoPorId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaAnulacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "AnuladoPorId",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "FechaAnulacion",
                table: "Gastos");
        }
    }
}
