using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Disponibilidad
    {
        public int IdDisponibilidad { get; set; }

        public int IdUsuario { get; set; }

        public int? DiaSemana { get; set; }   // 0 = Domingo, 6 = Sábado

        public decimal? CapacidadPorDia { get; set; }
    }
}
