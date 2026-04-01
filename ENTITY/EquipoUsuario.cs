using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class EquipoUsuario
    {
        public int IdEquipoUsuario { get; set; }
        public int IdEquipo { get; set; }
        public int IdUsuario { get; set; }

        public DateTime? FechaAsignacion { get; set; }
        public int? Activo { get; set; }
    }
}
