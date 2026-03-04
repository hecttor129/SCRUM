using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Evento
    {
        public int IdEvento { get; set; }
 
        public int IdUsuario { get; set; }

        public int? IdTarea { get; set; }

        public string TipoEvento { get; set; }

        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }
    }
}
