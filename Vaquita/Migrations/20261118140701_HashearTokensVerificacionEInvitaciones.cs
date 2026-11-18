using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaquita.Migrations
{
    /// <inheritdoc />
    public partial class HashearTokensVerificacionEInvitaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [TokensVerificacion];");
            migrationBuilder.Sql(
            "UPDATE [Invitaciones] " +
            "SET [Token] = CONVERT(varchar(64), " +
            "HASHBYTES('SHA2_256', CONVERT(varchar(150), [Token])), 2);");
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "TokensVerificacion",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_TokensVerificacion_Token",
                table: "TokensVerificacion",
                newName: "IX_TokensVerificacion_TokenHash");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Invitaciones",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_Invitaciones_Token",
                table: "Invitaciones",
                newName: "IX_Invitaciones_TokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "TokensVerificacion",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_TokensVerificacion_TokenHash",
                table: "TokensVerificacion",
                newName: "IX_TokensVerificacion_Token");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "Invitaciones",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_Invitaciones_TokenHash",
                table: "Invitaciones",
                newName: "IX_Invitaciones_Token");
        }
    }
}
