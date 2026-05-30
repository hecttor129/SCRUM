using System;

namespace ENTITY
{
    public class Archivo
    {
        public int IdArchivo { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreFisico { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public double TamanoKB { get; set; }
        public DateTime FechaSubida { get; set; }
        public int? IdEquipo { get; set; }
        public int? IdProyecto { get; set; }
        public int IdUsuarioSubidoPor { get; set; }

        // Propiedades de navegación
        public Equipo? Equipo { get; set; }
        public Proyecto? Proyecto { get; set; }
        public Usuario? UsuarioSubidoPor { get; set; }
    }
}
