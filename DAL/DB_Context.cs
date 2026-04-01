using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;

namespace DAL
{
    public class DB_Context : DbContext
    {
            protected override void OnConfiguring(DbContextOptionsBuilder options)
            {
                options.UseOracle(
                    "User Id=luis_scrum;Password=luis123;Data Source=127.0.0.1:1521/XEPDB1;");
                    
            }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RelacionJerarquica> RelacionesJerarquicas { get; set; }
        public DbSet<Especializacion> Especializaciones { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<ReglaAsignacion> ReglasAsignacion { get; set; }
        public DbSet<AsignacionTarea> AsignacionesTarea { get; set; }
        public DbSet<EvaluacionTarea> EvaluacionesTarea { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Disponibilidad> Disponibilidades { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<MetricaUsuario> MetricasUsuario { get; set; }
        public DbSet<UsuarioEspecializacion> UsuarioEspecializaciones { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<EquipoUsuario> EquipoUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =========================
            // USUARIOS
            // =========================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS");

                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.Apellido)
                      .HasColumnName("APELLIDO")
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasColumnName("EMAIL")
                      .HasMaxLength(30)
                      .IsRequired();

                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.password)
                      .HasColumnName("CONTRASENA")
                      .HasMaxLength(64)
                      .IsRequired();

                entity.Property(e => e.Rol)
                      .HasColumnName("ROL")
                      .HasConversion<string>()
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(e => e.NivelJerarquico)
                      .HasColumnName("NIVEL_JERARQUICO");

                entity.Property(e => e.Salario)
                      .HasColumnName("SALARIO")
                      .HasColumnType("NUMBER(12,2)");

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO")
                      .HasConversion<int>();

                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("FECHA_CREACION");
            });

            // =========================
            // ESPECIALIZACIONES
            // =========================
            modelBuilder.Entity<Especializacion>(entity =>
            {
                entity.ToTable("ESPECIALIZACIONES");

                entity.HasKey(e => e.IdEspecializacion);

                entity.Property(e => e.IdEspecializacion)
                      .HasColumnName("ID_ESPECIALIZACION");

                entity.Property(e => e.NombreEspecializacion)
                      .HasColumnName("NOMBRE_ESPECIALIZACION")
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION")
                      .HasColumnType("CLOB");
            });

            // =========================
            // TAREAS
            // =========================
            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.ToTable("TAREAS");

                entity.HasKey(e => e.IdTarea);

                entity.Property(e => e.IdTarea)
                      .HasColumnName("ID_TAREA");

                entity.Property(e => e.IdEspecializacion)
                      .HasColumnName("ID_ESPECIALIZACION");

                entity.Property(e => e.Titulo)
                      .HasColumnName("TITULO")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION")
                      .HasColumnType("CLOB");

                entity.Property(e => e.Prioridad)
                      .HasColumnName("PRIORIDAD");

                entity.Property(e => e.estadoTarea)
                      .HasColumnName("ESTADO")
                      .HasConversion<string>()
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("FECHA_INICIO");

                entity.Property(e => e.FechaLimite)
                      .HasColumnName("FECHA_LIMITE");

                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("FECHA_CREACION");

                entity.HasOne(e => e.Especializacion)
                      .WithMany()
                      .HasForeignKey(e => e.IdEspecializacion);
            });

            // =========================
            // ASIGNACION_TAREAS
            // =========================
            modelBuilder.Entity<AsignacionTarea>(entity =>
            {
                entity.ToTable("ASIGNACION_TAREAS");

                entity.HasKey(e => e.IdAsignacion);

                entity.Property(e => e.IdAsignacion)
                      .HasColumnName("ID_ASIGNACION");

                entity.Property(e => e.IdTarea)
                      .HasColumnName("ID_TAREA");

                entity.Property(e => e.IdAsignadoPor)
                      .HasColumnName("ID_ASIGNADO_POR");

                entity.Property(e => e.IdAsignadoA)
                      .HasColumnName("ID_ASIGNADO_A");

                entity.Property(e => e.IdRegla)
                      .HasColumnName("ID_REGLA");

                entity.Property(e => e.IdPeriodo)
                      .HasColumnName("ID_PERIODO");

                entity.Property(e => e.tipoAsignacion)
                      .HasColumnName("TIPO_ASIGNACION")
                      .HasConversion<string>()
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.EstadoAsignacion)
                      .HasColumnName("ESTADO")
                      .HasConversion<string>()
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.FechaAsignacion)
                      .HasColumnName("FECHA_ASIGNACION");

                // Relaciones SIN navegación
                entity.HasOne<Tarea>()
                      .WithMany()
                      .HasForeignKey(e => e.IdTarea);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdAsignadoPor)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdAsignadoA)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ReglaAsignacion>()
                      .WithMany()
                      .HasForeignKey(e => e.IdRegla);

                entity.HasOne<Periodo>()
                      .WithMany()
                      .HasForeignKey(e => e.IdPeriodo);
            });

            // =========================
            // USUARIO_ESPECIALIZACION
            // =========================
            modelBuilder.Entity<UsuarioEspecializacion>(entity =>
            {
                entity.ToTable("USUARIO_ESPECIALIZACION");

                entity.HasKey(e => new { e.IdUsuario, e.IdEspecializacion });

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO");

                entity.Property(e => e.IdEspecializacion)
                      .HasColumnName("ID_ESPECIALIZACION");
            });


            // =========================
            // PERIODOS
            // =========================
            modelBuilder.Entity<Periodo>(entity =>
            {
                entity.ToTable("PERIODOS");

                entity.HasKey(e => e.IdPeriodo);

                entity.Property(e => e.IdPeriodo)
                      .HasColumnName("ID_PERIODO");

                entity.Property(e => e.Tipo)
                      .HasColumnName("TIPO")
                      .HasMaxLength(10);

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("FECHA_INICIO");

                entity.Property(e => e.FechaFin)
                      .HasColumnName("FECHA_FIN");

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO")
                      .HasConversion<int>();
            });

            // =========================
            // REGLA_ASIGNACION
            // =========================
            modelBuilder.Entity<ReglaAsignacion>(entity =>
            {
                entity.ToTable("REGLA_ASIGNACION");

                entity.HasKey(e => e.IdRegla);

                entity.Property(e => e.IdRegla)
                      .HasColumnName("ID_REGLA");

                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .HasMaxLength(15);

                entity.Property(e => e.PesoDisponibilidad)
                      .HasColumnName("PESO_DISPONIBILIDAD")
                      .HasColumnType("NUMBER(5,2)");

                entity.Property(e => e.PesoCalificacion)
                      .HasColumnName("PESO_CALIFICACION")
                      .HasColumnType("NUMBER(5,2)");

                entity.Property(e => e.PesoCarga)
                      .HasColumnName("PESO_CARGA")
                      .HasColumnType("NUMBER(5,2)");

                entity.Property(e => e.PesoProductividad)
                      .HasColumnName("PESO_PRODUCTIVIDAD")
                      .HasColumnType("NUMBER(5,2)");

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO")
                      .HasConversion<int>();
            });

            // =========================
            // EVALUACION_TAREAS
            // =========================
            modelBuilder.Entity<EvaluacionTarea>(entity =>
            {
                entity.ToTable("EVALUACION_TAREAS");

                entity.HasKey(e => e.IdEvaluacion);

                entity.Property(e => e.IdEvaluacion)
                      .HasColumnName("ID_EVALUACION");

                entity.Property(e => e.IdTarea)
                      .HasColumnName("ID_TAREA");

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO");

                entity.Property(e => e.Puntuacion)
                      .HasColumnName("PUNTUACION");

                entity.Property(e => e.Comentario)
                      .HasColumnName("COMENTARIO")
                      .HasColumnType("CLOB");

                entity.Property(e => e.Fecha)
                      .HasColumnName("FECHA");

                entity.HasOne<Tarea>()
                      .WithMany()
                      .HasForeignKey(e => e.IdTarea);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario);
            });

            // =========================
            // EVENTOS
            // =========================

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.ToTable("EVENTOS");

                entity.HasKey(e => e.IdEvento);

                entity.Property(e => e.IdEvento)
                      .HasColumnName("ID_EVENTO");

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO");

                entity.Property(e => e.IdTarea)
                      .HasColumnName("ID_TAREA");

                entity.Property(e => e.TipoEvento)
                      .HasColumnName("TIPO_EVENTO")
                      .HasMaxLength(20);

                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION")
                      .HasColumnType("CLOB");

                entity.Property(e => e.Fecha)
                      .HasColumnName("FECHA");

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario);

                entity.HasOne<Tarea>()
                      .WithMany()
                      .HasForeignKey(e => e.IdTarea);
            });

            // =========================
            // DISPONIBILIDAD
            // =========================
            modelBuilder.Entity<Disponibilidad>(entity =>
            {
                entity.ToTable("DISPONIBILIDAD");

                entity.HasKey(e => e.IdDisponibilidad);

                entity.Property(e => e.IdDisponibilidad)
                      .HasColumnName("ID_DISPONIBILIDAD");

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO");

                entity.Property(e => e.DiaSemana)
                      .HasColumnName("DIA_SEMANA");

                entity.Property(e => e.CapacidadPorDia)
                      .HasColumnName("CAPACIDAD_POR_DIA")
                      .HasColumnType("NUMBER(5,2)");

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario);
            });

            // =========================
            // NOTIFICACIONES
            // =========================

            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.ToTable("NOTIFICACIONES");

                entity.HasKey(e => e.IdNotificacion);

                entity.Property(e => e.IdNotificacion)
                      .HasColumnName("ID_NOTIFICACION");

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO");

                entity.Property(e => e.Mensaje)
                      .HasColumnName("MENSAJE")
                      .HasColumnType("CLOB");

                entity.Property(e => e.Tipo)
                      .HasColumnName("TIPO")
                      .HasMaxLength(15);

                entity.Property(e => e.Fecha)
                      .HasColumnName("FECHA");

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario);
            });

            // =========================
            // METRICA_USUARIOS
            // =========================

            modelBuilder.Entity<MetricaUsuario>(entity =>
            {
                entity.ToTable("METRICA_USUARIOS");

                entity.HasKey(e => e.IdMetrica);

                entity.Property(e => e.IdMetrica)
                      .HasColumnName("ID_METRICA");

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO");

                entity.Property(e => e.IdPeriodo)
                      .HasColumnName("ID_PERIODO");

                entity.Property(e => e.TareasAsignadas)
                      .HasColumnName("TAREAS_ASIGNADAS");

                entity.Property(e => e.TareasCompletadas)
                      .HasColumnName("TAREAS_COMPLETADAS");

                entity.Property(e => e.CumplimientoPlazoPct)
                      .HasColumnName("CUMPLIMIENTO_PLAZO_PCT")
                      .HasColumnType("NUMBER(5,4)");

                entity.Property(e => e.CargaPromedio)
                      .HasColumnName("CARGA_PROMEDIO")
                      .HasColumnType("NUMBER(5,4)");

                entity.Property(e => e.Confiabilidad)
                      .HasColumnName("CONFIABILIDAD")
                      .HasColumnType("NUMBER(5,4)");

                entity.Property(e => e.CalificacionActual)
                      .HasColumnName("CALIFICACION_ACTUAL")
                      .HasColumnType("NUMBER(5,4)");

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario);

                entity.HasOne<Periodo>()
                      .WithMany()
                      .HasForeignKey(e => e.IdPeriodo);
            });


            // =========================
            // RELACION_JERARQUICA
            // =========================

            modelBuilder.Entity<RelacionJerarquica>(entity =>
            {
                entity.ToTable("RELACION_JERARQUICA");

                entity.HasKey(e => e.IdRelacion);

                entity.Property(e => e.IdRelacion)
                      .HasColumnName("ID_RELACION");

                entity.Property(e => e.IdJefe)
                      .HasColumnName("ID_JEFE");

                entity.Property(e => e.IdEmpleado)
                      .HasColumnName("ID_EMPLEADO");

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("FECHA_INICIO");

                entity.Property(e => e.FechaFin)
                      .HasColumnName("FECHA_FIN");

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdJefe)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdEmpleado)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("EMPRESAS");

                entity.HasKey(e => e.IdEmpresa);

                entity.Property(e => e.IdEmpresa)
                      .HasColumnName("ID_EMPRESA")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION")
                      .HasMaxLength(200);

                entity.Property(e => e.Nit)
                      .HasColumnName("NIT")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Direccion)
                      .HasColumnName("DIRECCION")
                      .HasMaxLength(120);

                entity.Property(e => e.Correo)
                      .HasColumnName("CORREO")
                      .HasMaxLength(80);

                entity.Property(e => e.Telefono)
                      .HasColumnName("TELEFONO")
                      .HasMaxLength(20);

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO");


                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("FECHA_CREACION");
            });

            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.ToTable("PROYECTOS");

                entity.HasKey(e => e.IdProyecto);

                entity.Property(e => e.IdProyecto)
                      .HasColumnName("ID_PROYECTO")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.IdEmpresa)
                      .HasColumnName("ID_EMPRESA")
                      .IsRequired();

                entity.Property(e => e.IdSupervisor)
                      .HasColumnName("ID_SUPERVISOR");

                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION")
                      .HasMaxLength(250);

                entity.Property(e => e.Estado)
                      .HasColumnName("ESTADO")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("FECHA_INICIO");

                entity.Property(e => e.FechaFin)
                      .HasColumnName("FECHA_FIN");

                entity.Property(e => e.Progreso)
                      .HasColumnName("PROGRESO")
                      .HasPrecision(5, 2);

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO");


                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("FECHA_CREACION");

                entity.HasOne<Empresa>()
                      .WithMany()
                      .HasForeignKey(e => e.IdEmpresa);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdSupervisor)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Equipo>(entity =>
            {
                entity.ToTable("EQUIPOS");

                entity.HasKey(e => e.IdEquipo);

                entity.Property(e => e.IdEquipo)
                      .HasColumnName("ID_EQUIPO")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.IdProyecto)
                      .HasColumnName("ID_PROYECTO")
                      .IsRequired();

                entity.Property(e => e.IdSupervisor)
                      .HasColumnName("ID_SUPERVISOR")
                      .IsRequired();

                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION")
                      .HasMaxLength(250);

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO");

                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("FECHA_CREACION");

                entity.HasOne<Proyecto>()
                      .WithMany()
                      .HasForeignKey(e => e.IdProyecto);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdSupervisor)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EquipoUsuario>(entity =>
            {
                entity.ToTable("EQUIPO_USUARIOS");

                entity.HasKey(e => e.IdEquipoUsuario);

                entity.Property(e => e.IdEquipoUsuario)
                      .HasColumnName("ID_EQUIPO_USUARIO")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.IdEquipo)
                      .HasColumnName("ID_EQUIPO")
                      .IsRequired();

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("ID_USUARIO")
                      .IsRequired();

                entity.Property(e => e.FechaAsignacion)
                      .HasColumnName("FECHA_ASIGNACION");

                entity.Property(e => e.Activo)
                      .HasColumnName("ACTIVO");

                entity.HasOne<Equipo>()
                      .WithMany()
                      .HasForeignKey(e => e.IdEquipo);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
