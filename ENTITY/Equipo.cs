using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Equipo
    {
        public int IdEquipo { get; set; }
        public int IdProyecto { get; set; }
        public int IdSupervisor { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
