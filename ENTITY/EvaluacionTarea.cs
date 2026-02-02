using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class EvaluacionTarea
    {
        public int IdEvaluacion { get; set; }

        public int IdTarea { get; set; }

        public int IdUsuario { get; set; }

        public int? Puntuacion { get; set; }

        public string Comentario { get; set; }

        public DateTime Fecha { get; set; }

    }
}
