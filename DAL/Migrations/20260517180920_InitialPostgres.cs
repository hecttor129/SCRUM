using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMPRESAS",
                columns: table => new
                {
                    ID_EMPRESA = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOMBRE = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NIT = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CORREO = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TELEFONO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DIRECCION = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPRESAS", x => x.ID_EMPRESA);
                });

            migrationBuilder.CreateTable(
                name: "ESPECIALIZACIONES",
                columns: table => new
                {
                    ID_ESPECIALIZACION = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOMBRE_ESPECIALIZACION = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESPECIALIZACIONES", x => x.ID_ESPECIALIZACION);
                });

            migrationBuilder.CreateTable(
                name: "PERIODOS",
                columns: table => new
                {
                    ID_PERIODO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TIPO = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FECHA_INICIO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FECHA_FIN = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERIODOS", x => x.ID_PERIODO);
                });

            migrationBuilder.CreateTable(
                name: "REGLA_ASIGNACION",
                columns: table => new
                {
                    ID_REGLA = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOMBRE = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    PESO_DISPONIBILIDAD = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    PESO_CALIFICACION = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    PESO_CARGA = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    PESO_PRODUCTIVIDAD = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGLA_ASIGNACION", x => x.ID_REGLA);
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

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOMBRE = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    APELLIDO = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    EMAIL = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CONTRASENA = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ROL = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NIVEL_JERARQUICO = table.Column<int>(type: "integer", nullable: false),
                    SALARIO = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: false),
                    FECHA_CREACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "TAREAS",
                columns: table => new
                {
                    ID_TAREA = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_ESPECIALIZACION = table.Column<int>(type: "integer", nullable: false),
                    TITULO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "text", nullable: false),
                    PRIORIDAD = table.Column<int>(type: "integer", nullable: true),
                    ESTADO = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    FECHA_INICIO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FECHA_LIMITE = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAREAS", x => x.ID_TAREA);
                    table.ForeignKey(
                        name: "FK_TAREAS_ESPECIALIZACIONES_ID_ESPECIALIZACION",
                        column: x => x.ID_ESPECIALIZACION,
                        principalTable: "ESPECIALIZACIONES",
                        principalColumn: "ID_ESPECIALIZACION",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DISPONIBILIDAD",
                columns: table => new
                {
                    ID_DISPONIBILIDAD = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    DIA_SEMANA = table.Column<int>(type: "integer", nullable: true),
                    CAPACIDAD_POR_DIA = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISPONIBILIDAD", x => x.ID_DISPONIBILIDAD);
                    table.ForeignKey(
                        name: "FK_DISPONIBILIDAD_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "METRICA_USUARIOS",
                columns: table => new
                {
                    ID_METRICA = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    ID_PERIODO = table.Column<int>(type: "integer", nullable: false),
                    TAREAS_ASIGNADAS = table.Column<int>(type: "integer", nullable: true),
                    TAREAS_COMPLETADAS = table.Column<int>(type: "integer", nullable: true),
                    CUMPLIMIENTO_PLAZO_PCT = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    CARGA_PROMEDIO = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    CONFIABILIDAD = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    CALIFICACION_ACTUAL = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_METRICA_USUARIOS", x => x.ID_METRICA);
                    table.ForeignKey(
                        name: "FK_METRICA_USUARIOS_PERIODOS_ID_PERIODO",
                        column: x => x.ID_PERIODO,
                        principalTable: "PERIODOS",
                        principalColumn: "ID_PERIODO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_METRICA_USUARIOS_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NOTIFICACIONES",
                columns: table => new
                {
                    ID_NOTIFICACION = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    MENSAJE = table.Column<string>(type: "text", nullable: false),
                    TIPO = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    FECHA = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICACIONES", x => x.ID_NOTIFICACION);
                    table.ForeignKey(
                        name: "FK_NOTIFICACIONES_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PROYECTOS",
                columns: table => new
                {
                    ID_PROYECTO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_EMPRESA = table.Column<int>(type: "integer", nullable: false),
                    ID_SUPERVISOR = table.Column<int>(type: "integer", nullable: true),
                    NOMBRE = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ESTADO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FECHA_INICIO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FECHA_FIN = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PROGRESO = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROYECTOS", x => x.ID_PROYECTO);
                    table.ForeignKey(
                        name: "FK_PROYECTOS_EMPRESAS_ID_EMPRESA",
                        column: x => x.ID_EMPRESA,
                        principalTable: "EMPRESAS",
                        principalColumn: "ID_EMPRESA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PROYECTOS_USUARIOS_ID_SUPERVISOR",
                        column: x => x.ID_SUPERVISOR,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RELACION_JERARQUICA",
                columns: table => new
                {
                    ID_RELACION = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_JEFE = table.Column<int>(type: "integer", nullable: false),
                    ID_EMPLEADO = table.Column<int>(type: "integer", nullable: false),
                    FECHA_INICIO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FECHA_FIN = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELACION_JERARQUICA", x => x.ID_RELACION);
                    table.ForeignKey(
                        name: "FK_RELACION_JERARQUICA_USUARIOS_ID_EMPLEADO",
                        column: x => x.ID_EMPLEADO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RELACION_JERARQUICA_USUARIOS_ID_JEFE",
                        column: x => x.ID_JEFE,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ASIGNACION_TAREAS",
                columns: table => new
                {
                    ID_ASIGNACION = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_TAREA = table.Column<int>(type: "integer", nullable: false),
                    ID_ASIGNADO_POR = table.Column<int>(type: "integer", nullable: false),
                    ID_ASIGNADO_A = table.Column<int>(type: "integer", nullable: false),
                    ID_REGLA = table.Column<int>(type: "integer", nullable: true),
                    ID_PERIODO = table.Column<int>(type: "integer", nullable: false),
                    TIPO_ASIGNACION = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    FECHA_ASIGNACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ESTADO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASIGNACION_TAREAS", x => x.ID_ASIGNACION);
                    table.ForeignKey(
                        name: "FK_ASIGNACION_TAREAS_PERIODOS_ID_PERIODO",
                        column: x => x.ID_PERIODO,
                        principalTable: "PERIODOS",
                        principalColumn: "ID_PERIODO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ASIGNACION_TAREAS_REGLA_ASIGNACION_ID_REGLA",
                        column: x => x.ID_REGLA,
                        principalTable: "REGLA_ASIGNACION",
                        principalColumn: "ID_REGLA");
                    table.ForeignKey(
                        name: "FK_ASIGNACION_TAREAS_TAREAS_ID_TAREA",
                        column: x => x.ID_TAREA,
                        principalTable: "TAREAS",
                        principalColumn: "ID_TAREA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ASIGNACION_TAREAS_USUARIOS_ID_ASIGNADO_A",
                        column: x => x.ID_ASIGNADO_A,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ASIGNACION_TAREAS_USUARIOS_ID_ASIGNADO_POR",
                        column: x => x.ID_ASIGNADO_POR,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVALUACION_TAREAS",
                columns: table => new
                {
                    ID_EVALUACION = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_TAREA = table.Column<int>(type: "integer", nullable: false),
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    PUNTUACION = table.Column<int>(type: "integer", nullable: true),
                    COMENTARIO = table.Column<string>(type: "text", nullable: false),
                    FECHA = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVALUACION_TAREAS", x => x.ID_EVALUACION);
                    table.ForeignKey(
                        name: "FK_EVALUACION_TAREAS_TAREAS_ID_TAREA",
                        column: x => x.ID_TAREA,
                        principalTable: "TAREAS",
                        principalColumn: "ID_TAREA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EVALUACION_TAREAS_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EVENTOS",
                columns: table => new
                {
                    ID_EVENTO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    ID_TAREA = table.Column<int>(type: "integer", nullable: true),
                    TIPO_EVENTO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "text", nullable: false),
                    FECHA = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTOS", x => x.ID_EVENTO);
                    table.ForeignKey(
                        name: "FK_EVENTOS_TAREAS_ID_TAREA",
                        column: x => x.ID_TAREA,
                        principalTable: "TAREAS",
                        principalColumn: "ID_TAREA");
                    table.ForeignKey(
                        name: "FK_EVENTOS_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPOS",
                columns: table => new
                {
                    ID_EQUIPO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_PROYECTO = table.Column<int>(type: "integer", nullable: false),
                    ID_SUPERVISOR = table.Column<int>(type: "integer", nullable: false),
                    NOMBRE = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: true),
                    FECHA_CREACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPOS", x => x.ID_EQUIPO);
                    table.ForeignKey(
                        name: "FK_EQUIPOS_PROYECTOS_ID_PROYECTO",
                        column: x => x.ID_PROYECTO,
                        principalTable: "PROYECTOS",
                        principalColumn: "ID_PROYECTO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EQUIPOS_USUARIOS_ID_SUPERVISOR",
                        column: x => x.ID_SUPERVISOR,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPO_USUARIOS",
                columns: table => new
                {
                    ID_EQUIPO_USUARIO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_EQUIPO = table.Column<int>(type: "integer", nullable: false),
                    ID_USUARIO = table.Column<int>(type: "integer", nullable: false),
                    FECHA_ASIGNACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ACTIVO = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPO_USUARIOS", x => x.ID_EQUIPO_USUARIO);
                    table.ForeignKey(
                        name: "FK_EQUIPO_USUARIOS_EQUIPOS_ID_EQUIPO",
                        column: x => x.ID_EQUIPO,
                        principalTable: "EQUIPOS",
                        principalColumn: "ID_EQUIPO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EQUIPO_USUARIOS_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_TAREAS_ID_ASIGNADO_A",
                table: "ASIGNACION_TAREAS",
                column: "ID_ASIGNADO_A");

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_TAREAS_ID_ASIGNADO_POR",
                table: "ASIGNACION_TAREAS",
                column: "ID_ASIGNADO_POR");

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_TAREAS_ID_PERIODO",
                table: "ASIGNACION_TAREAS",
                column: "ID_PERIODO");

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_TAREAS_ID_REGLA",
                table: "ASIGNACION_TAREAS",
                column: "ID_REGLA");

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_TAREAS_ID_TAREA",
                table: "ASIGNACION_TAREAS",
                column: "ID_TAREA");

            migrationBuilder.CreateIndex(
                name: "IX_DISPONIBILIDAD_ID_USUARIO",
                table: "DISPONIBILIDAD",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_USUARIOS_ID_EQUIPO",
                table: "EQUIPO_USUARIOS",
                column: "ID_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_USUARIOS_ID_USUARIO",
                table: "EQUIPO_USUARIOS",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPOS_ID_PROYECTO",
                table: "EQUIPOS",
                column: "ID_PROYECTO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPOS_ID_SUPERVISOR",
                table: "EQUIPOS",
                column: "ID_SUPERVISOR");

            migrationBuilder.CreateIndex(
                name: "IX_EVALUACION_TAREAS_ID_TAREA",
                table: "EVALUACION_TAREAS",
                column: "ID_TAREA");

            migrationBuilder.CreateIndex(
                name: "IX_EVALUACION_TAREAS_ID_USUARIO",
                table: "EVALUACION_TAREAS",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTOS_ID_TAREA",
                table: "EVENTOS",
                column: "ID_TAREA");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTOS_ID_USUARIO",
                table: "EVENTOS",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_METRICA_USUARIOS_ID_PERIODO",
                table: "METRICA_USUARIOS",
                column: "ID_PERIODO");

            migrationBuilder.CreateIndex(
                name: "IX_METRICA_USUARIOS_ID_USUARIO",
                table: "METRICA_USUARIOS",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICACIONES_ID_USUARIO",
                table: "NOTIFICACIONES",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_PROYECTOS_ID_EMPRESA",
                table: "PROYECTOS",
                column: "ID_EMPRESA");

            migrationBuilder.CreateIndex(
                name: "IX_PROYECTOS_ID_SUPERVISOR",
                table: "PROYECTOS",
                column: "ID_SUPERVISOR");

            migrationBuilder.CreateIndex(
                name: "IX_RELACION_JERARQUICA_ID_EMPLEADO",
                table: "RELACION_JERARQUICA",
                column: "ID_EMPLEADO");

            migrationBuilder.CreateIndex(
                name: "IX_RELACION_JERARQUICA_ID_JEFE",
                table: "RELACION_JERARQUICA",
                column: "ID_JEFE");

            migrationBuilder.CreateIndex(
                name: "IX_TAREAS_ID_ESPECIALIZACION",
                table: "TAREAS",
                column: "ID_ESPECIALIZACION");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_EMAIL",
                table: "USUARIOS",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ASIGNACION_TAREAS");

            migrationBuilder.DropTable(
                name: "DISPONIBILIDAD");

            migrationBuilder.DropTable(
                name: "EQUIPO_USUARIOS");

            migrationBuilder.DropTable(
                name: "EVALUACION_TAREAS");

            migrationBuilder.DropTable(
                name: "EVENTOS");

            migrationBuilder.DropTable(
                name: "METRICA_USUARIOS");

            migrationBuilder.DropTable(
                name: "NOTIFICACIONES");

            migrationBuilder.DropTable(
                name: "RELACION_JERARQUICA");

            migrationBuilder.DropTable(
                name: "USUARIO_ESPECIALIZACION");

            migrationBuilder.DropTable(
                name: "REGLA_ASIGNACION");

            migrationBuilder.DropTable(
                name: "EQUIPOS");

            migrationBuilder.DropTable(
                name: "TAREAS");

            migrationBuilder.DropTable(
                name: "PERIODOS");

            migrationBuilder.DropTable(
                name: "PROYECTOS");

            migrationBuilder.DropTable(
                name: "ESPECIALIZACIONES");

            migrationBuilder.DropTable(
                name: "EMPRESAS");

            migrationBuilder.DropTable(
                name: "USUARIOS");
        }
    }
}
