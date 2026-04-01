using System;

namespace ENTITY
{
    public class Proyecto
    {
        public int IdProyecto { get; set; }
        public int IdEmpresa { get; set; }

        public int? IdSupervisor { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public decimal? Progreso { get; set; }

        public int? Activo { get; set; }

        public DateTime? FechaCreacion { get; set; }
       
    }
}