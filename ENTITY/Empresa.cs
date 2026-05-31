using System;

namespace ENTITY
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Nit { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }
        public int? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}