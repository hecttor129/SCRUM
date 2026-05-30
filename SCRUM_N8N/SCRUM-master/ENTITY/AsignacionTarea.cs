using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ENTITY.ENUMS;

namespace ENTITY
{
    public class AsignacionTarea
    {
        public int IdAsignacion { get; set; }

        public int IdTarea { get; set; }

        public int IdAsignadoPor { get; set; }

        public int IdAsignadoA { get; set; }

        public int? IdRegla { get; set; }

        public int IdPeriodo { get; set; }

        public TipoAsignacion tipoAsignacion { get; set; }
        //enum

        public DateTime FechaAsignacion { get; set; }

        public EstadoAsignacion EstadoAsignacion { get; set; }
        //enum
    }
}
