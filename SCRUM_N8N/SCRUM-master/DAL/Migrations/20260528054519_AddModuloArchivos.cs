using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloArchivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ARCHIVOS",
                columns: table => new
                {
                    ID_ARCHIVO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOMBRE_ORIGINAL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NOMBRE_FISICO = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EXTENSION = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TAMANO_KB = table.Column<double>(type: "double precision", nullable: false),
                    FECHA_SUBIDA = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ID_EQUIPO = table.Column<int>(type: "integer", nullable: false),
                    ID_USUARIO_SUBIDO_POR = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARCHIVOS", x => x.ID_ARCHIVO);
                    table.ForeignKey(
                        name: "FK_ARCHIVOS_EQUIPOS_ID_EQUIPO",
                        column: x => x.ID_EQUIPO,
                        principalTable: "EQUIPOS",
                        principalColumn: "ID_EQUIPO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARCHIVOS_USUARIOS_ID_USUARIO_SUBIDO_POR",
                        column: x => x.ID_USUARIO_SUBIDO_POR,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARCHIVOS_ID_EQUIPO",
                table: "ARCHIVOS",
                column: "ID_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_ARCHIVOS_ID_USUARIO_SUBIDO_POR",
                table: "ARCHIVOS",
                column: "ID_USUARIO_SUBIDO_POR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARCHIVOS");
        }
    }
}
