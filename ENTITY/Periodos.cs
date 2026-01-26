using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Periodos
    {
        public int IdPeriodo { get; set; }

        public string Tipo { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool? Activo { get; set; }
    }
}
