using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class ReglaAsignacion
    {
        public int IdRegla { get; set; }

        public string Nombre { get; set; }

        public decimal? PesoDisponibilidad { get; set; }

        public decimal? PesoCalificacion { get; set; }

        public decimal? PesoCarga { get; set; }

        public decimal? PesoProductividad { get; set; }

        public bool? Activo { get; set; }
    }
}
