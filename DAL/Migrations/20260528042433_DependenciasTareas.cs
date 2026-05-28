using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DependenciasTareas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "DEPENDENCIAS",
                table: "TAREAS",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<bool>(
                name: "DISPONIBLE",
                table: "TAREAS",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DEPENDENCIAS",
                table: "TAREAS");

            migrationBuilder.DropColumn(
                name: "DISPONIBLE",
                table: "TAREAS");
        }
    }
}
