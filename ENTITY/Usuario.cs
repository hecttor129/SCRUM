using System;
using System.Collections.Generic;
using static ENTITY.ENUMS;

namespace ENTITY
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string password { get; set; }   // hash SHA-256 (64 chars)

        public RolUsuario Rol { get; set; }
        //enum

        public int NivelJerarquico { get; set; }

        public decimal? Salario { get; set; }

        public int Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Colección de especialidades del usuario (almacenada como text[] en PostgreSQL).
        /// </summary>
        public List<string> Especializaciones { get; set; } = new List<string>();
    }
}
