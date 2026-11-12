using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaquita.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCodigoInvitacionGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoInvitacion",
                table: "Grupos",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

             migrationBuilder.Sql(
        "UPDATE Grupos SET CodigoInvitacion = LOWER(CONVERT(varchar(32), CRYPT_GEN_RANDOM(16), 2))");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_CodigoInvitacion",
                table: "Grupos",
                column: "CodigoInvitacion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Grupos_CodigoInvitacion",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "CodigoInvitacion",
                table: "Grupos");
        }
    }
}
