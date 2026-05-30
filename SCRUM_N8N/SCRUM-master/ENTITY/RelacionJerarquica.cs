using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class RelacionJerarquica
    {
        public int IdRelacion { get; set; }

        public int IdJefe { get; set; }

        public int IdEmpleado { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
    }
}
