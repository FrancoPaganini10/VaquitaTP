using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vaquita.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rubros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apellido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Apodo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailVerificado = table.Column<bool>(type: "bit", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministradorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grupos_Usuarios_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokensVerificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usado = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensVerificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokensVerificacion_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    PagadorId = table.Column<int>(type: "int", nullable: false),
                    RegistradoPorId = table.Column<int>(type: "int", nullable: false),
                    RubroId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                    table.CheckConstraint("CK_Gasto_Importe", "[Importe] > 0");
                    table.ForeignKey(
                        name: "FK_Gastos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gastos_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Gastos_Usuarios_PagadorId",
                        column: x => x.PagadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gastos_Usuarios_RegistradoPorId",
                        column: x => x.RegistradoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Integrantes",
                columns: table => new
                {
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EsAdministrador = table.Column<bool>(type: "bit", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrantes", x => new { x.GrupoId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Integrantes_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Integrantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    InvitadoPorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Usuarios_InvitadoPorId",
                        column: x => x.InvitadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    DeudorId = table.Column<int>(type: "int", nullable: false),
                    AcreedorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.CheckConstraint("CK_Pago_Importe", "[Importe] > 0");
                    table.CheckConstraint("CK_Pago_Personas", "[DeudorId] <> [AcreedorId]");
                    table.ForeignKey(
                        name: "FK_Pagos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagos_Usuarios_AcreedorId",
                        column: x => x.AcreedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_Usuarios_DeudorId",
                        column: x => x.DeudorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Rubros",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Combustible" },
                    { 2, "Alojamiento" },
                    { 3, "Transporte" },
                    { 4, "Materiales" },
                    { 5, "Mano de obra" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_GrupoId",
                table: "Gastos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_PagadorId",
                table: "Gastos",
                column: "PagadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_RegistradoPorId",
                table: "Gastos",
                column: "RegistradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_RubroId",
                table: "Gastos",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_AdministradorId",
                table: "Grupos",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Integrantes_UsuarioId",
                table: "Integrantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_GrupoId",
                table: "Invitaciones",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_InvitadoPorId",
                table: "Invitaciones",
                column: "InvitadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_Token",
                table: "Invitaciones",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_AcreedorId",
                table: "Pagos",
                column: "AcreedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_DeudorId",
                table: "Pagos",
                column: "DeudorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_GrupoId",
                table: "Pagos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Rubros_Nombre",
                table: "Rubros",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokensVerificacion_Token",
                table: "TokensVerificacion",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokensVerificacion_UsuarioId",
                table: "TokensVerificacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "Integrantes");

            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "TokensVerificacion");

            migrationBuilder.DropTable(
                name: "Rubros");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
