using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class ReglaBono
    {
        public int IdRegla { get; set; }

        public string TipoMetrica { get; set; }

        public string Operador { get; set; }   // >, <, >=, <=, =

        public decimal? ValorObjetivo { get; set; }

        public decimal? Monto { get; set; }

        public bool? Activo { get; set; }
    }
}
