using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddProyectoArchivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ARCHIVOS_EQUIPOS_ID_EQUIPO",
                table: "ARCHIVOS");

            migrationBuilder.AlterColumn<int>(
                name: "ID_EQUIPO",
                table: "ARCHIVOS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ID_PROYECTO",
                table: "ARCHIVOS",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ARCHIVOS_ID_PROYECTO",
                table: "ARCHIVOS",
                column: "ID_PROYECTO");

            migrationBuilder.AddForeignKey(
                name: "FK_ARCHIVOS_EQUIPOS_ID_EQUIPO",
                table: "ARCHIVOS",
                column: "ID_EQUIPO",
                principalTable: "EQUIPOS",
                principalColumn: "ID_EQUIPO");

            migrationBuilder.AddForeignKey(
                name: "FK_ARCHIVOS_PROYECTOS_ID_PROYECTO",
                table: "ARCHIVOS",
                column: "ID_PROYECTO",
                principalTable: "PROYECTOS",
                principalColumn: "ID_PROYECTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ARCHIVOS_EQUIPOS_ID_EQUIPO",
                table: "ARCHIVOS");

            migrationBuilder.DropForeignKey(
                name: "FK_ARCHIVOS_PROYECTOS_ID_PROYECTO",
                table: "ARCHIVOS");

            migrationBuilder.DropIndex(
                name: "IX_ARCHIVOS_ID_PROYECTO",
                table: "ARCHIVOS");

            migrationBuilder.DropColumn(
                name: "ID_PROYECTO",
                table: "ARCHIVOS");

            migrationBuilder.AlterColumn<int>(
                name: "ID_EQUIPO",
                table: "ARCHIVOS",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ARCHIVOS_EQUIPOS_ID_EQUIPO",
                table: "ARCHIVOS",
                column: "ID_EQUIPO",
                principalTable: "EQUIPOS",
                principalColumn: "ID_EQUIPO",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
