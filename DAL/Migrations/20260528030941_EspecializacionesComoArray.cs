using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class EspecializacionesComoArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TAREAS_ESPECIALIZACIONES_ID_ESPECIALIZACION",
                table: "TAREAS");

            migrationBuilder.DropTable(
                name: "ESPECIALIZACIONES");

            migrationBuilder.DropTable(
                name: "USUARIO_ESPECIALIZACION");

            migrationBuilder.DropIndex(
                name: "IX_TAREAS_ID_ESPECIALIZACION",
                table: "TAREAS");

            migrationBuilder.DropColumn(
                name: "ID_ESPECIALIZACION",
                table: "TAREAS");

            migrationBuilder.AlterColumn<string>(
                name: "ROL",
                table: "USUARIOS",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "USUARIOS",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<List<string>>(
                name: "ESPECIALIZACIONES",
                table: "USUARIOS",
                type: "text[]",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_LIMITE",
                table: "TAREAS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "TAREAS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "TAREAS",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "ESPECIALIZACION_REQUERIDA",
                table: "TAREAS",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ID_EMPRESA",
                table: "TAREAS",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_EQUIPO",
                table: "TAREAS",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_PROYECTO",
                table: "TAREAS",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "RELACION_JERARQUICA",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_FIN",
                table: "RELACION_JERARQUICA",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "PROYECTOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_FIN",
                table: "PROYECTOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "PROYECTOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "PERIODOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_FIN",
                table: "PERIODOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA",
                table: "NOTIFICACIONES",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA",
                table: "EVENTOS",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA",
                table: "EVALUACION_TAREAS",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "EQUIPOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_ASIGNACION",
                table: "EQUIPO_USUARIOS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "EMPRESAS",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_ASIGNACION",
                table: "ASIGNACION_TAREAS",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_TAREAS_ID_EMPRESA",
                table: "TAREAS",
                column: "ID_EMPRESA");

            migrationBuilder.CreateIndex(
                name: "IX_TAREAS_ID_EQUIPO",
                table: "TAREAS",
                column: "ID_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_TAREAS_ID_PROYECTO",
                table: "TAREAS",
                column: "ID_PROYECTO");

            migrationBuilder.AddForeignKey(
                name: "FK_TAREAS_EMPRESAS_ID_EMPRESA",
                table: "TAREAS",
                column: "ID_EMPRESA",
                principalTable: "EMPRESAS",
                principalColumn: "ID_EMPRESA");

            migrationBuilder.AddForeignKey(
                name: "FK_TAREAS_EQUIPOS_ID_EQUIPO",
                table: "TAREAS",
                column: "ID_EQUIPO",
                principalTable: "EQUIPOS",
                principalColumn: "ID_EQUIPO");

            migrationBuilder.AddForeignKey(
                name: "FK_TAREAS_PROYECTOS_ID_PROYECTO",
                table: "TAREAS",
                column: "ID_PROYECTO",
                principalTable: "PROYECTOS",
                principalColumn: "ID_PROYECTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TAREAS_EMPRESAS_ID_EMPRESA",
                table: "TAREAS");

            migrationBuilder.DropForeignKey(
                name: "FK_TAREAS_EQUIPOS_ID_EQUIPO",
                table: "TAREAS");

            migrationBuilder.DropForeignKey(
                name: "FK_TAREAS_PROYECTOS_ID_PROYECTO",
                table: "TAREAS");

            migrationBuilder.DropIndex(
                name: "IX_TAREAS_ID_EMPRESA",
                table: "TAREAS");

            migrationBuilder.DropIndex(
                name: "IX_TAREAS_ID_EQUIPO",
                table: "TAREAS");

            migrationBuilder.DropIndex(
                name: "IX_TAREAS_ID_PROYECTO",
                table: "TAREAS");

            migrationBuilder.DropColumn(
                name: "ESPECIALIZACIONES",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "ESPECIALIZACION_REQUERIDA",
                table: "TAREAS");

            migrationBuilder.DropColumn(
                name: "ID_EMPRESA",
                table: "TAREAS");

            migrationBuilder.DropColumn(
                name: "ID_EQUIPO",
                table: "TAREAS");

            migrationBuilder.DropColumn(
                name: "ID_PROYECTO",
                table: "TAREAS");

            migrationBuilder.AlterColumn<string>(
                name: "ROL",
                table: "USUARIOS",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "USUARIOS",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_LIMITE",
                table: "TAREAS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "TAREAS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "TAREAS",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "ID_ESPECIALIZACION",
                table: "TAREAS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "RELACION_JERARQUICA",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_FIN",
                table: "RELACION_JERARQUICA",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "PROYECTOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_FIN",
                table: "PROYECTOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "PROYECTOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_INICIO",
                table: "PERIODOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_FIN",
                table: "PERIODOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA",
                table: "NOTIFICACIONES",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA",
                table: "EVENTOS",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA",
                table: "EVALUACION_TAREAS",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "EQUIPOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_ASIGNACION",
                table: "EQUIPO_USUARIOS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_CREACION",
                table: "EMPRESAS",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FECHA_ASIGNACION",
                table: "ASIGNACION_TAREAS",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateTable(
                name: "ESPECIALIZACIONES",
                columns: table => new
                {
                    ID_ESPECIALIZACION = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DESCRIPCION = table.Column<string>(type: "text", nullable: false),
                    NOMBRE_ESPECIALIZACION = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESPECIALIZACIONES", x => x.ID_ESPECIALIZACION);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_ESPECIALIZACION",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    ID_ESPECIALIZACION = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_ESPECIALIZACION", x => new { x.ID_USUARIO, x.ID_ESPECIALIZACION });
                });

            migrationBuilder.CreateIndex(
                name: "IX_TAREAS_ID_ESPECIALIZACION",
                table: "TAREAS",
                column: "ID_ESPECIALIZACION");

            migrationBuilder.AddForeignKey(
                name: "FK_TAREAS_ESPECIALIZACIONES_ID_ESPECIALIZACION",
                table: "TAREAS",
                column: "ID_ESPECIALIZACION",
                principalTable: "ESPECIALIZACIONES",
                principalColumn: "ID_ESPECIALIZACION",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
