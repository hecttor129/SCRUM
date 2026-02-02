using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Notificacion
    {
        public int IdNotificacion { get; set; }

        public int IdUsuario { get; set; }

        public string Mensaje { get; set; }

        public string Tipo { get; set; }

        public DateTime Fecha { get; set; }
    }
}
