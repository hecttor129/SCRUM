using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Bonos
    {
        public int IdBono { get; set; }

        public int IdUsuario { get; set; }

        public int IdRegla { get; set; }

        public decimal? Monto { get; set; }

        public int IdPeriodo { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
