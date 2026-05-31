using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class MetricaUsuario
    {
        public int IdMetrica { get; set; }

        public int IdUsuario { get; set; }

        public int IdPeriodo { get; set; }

        public int? TareasAsignadas { get; set; }

        public int? TareasCompletadas { get; set; }

        public decimal? CumplimientoPlazoPct { get; set; }

        public decimal? CargaPromedio { get; set; }

        public decimal? Confiabilidad { get; set; }

        public decimal? CalificacionActual { get; set; }
    }
}
