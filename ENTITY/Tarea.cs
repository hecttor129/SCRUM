using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ENTITY.ENUMS;

namespace ENTITY
{
    public class Tarea
    {
        public int IdTarea { get; set; }

        public int IdEspecializacion { get; set; }

        // Nuevas llaves foráneas opcionales para la jerarquía de tareas
        public int? IdEmpresa { get; set; }
        public int? IdProyecto { get; set; }
        public int? IdEquipo { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public int? Prioridad { get; set; }

        public EstadoTarea estadoTarea { get; set; }
        //enum

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaLimite { get; set; }

        public DateTime FechaCreacion { get; set; }

        public Especializacion Especializacion { get; set; }
        
        public Empresa Empresa { get; set; }
        public Proyecto Proyecto { get; set; }
        public Equipo Equipo { get; set; }
    }
}
