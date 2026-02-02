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
        public DB_Context(DbContextOptions<DB_Context> options) : base(options)
        {
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
        public DbSet<ReglaBono> ReglasBono { get; set; }
        public DbSet<Bono> Bonos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<MetricaUsuario> MetricasUsuario { get; set; }
        public DbSet<UsuarioEspecializacion> UsuarioEspecializaciones { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          //mapeo de oracle a c#


        }



















    }
}
