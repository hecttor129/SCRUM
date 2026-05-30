using System;
using System.Collections.Generic;
using static ENTITY.ENUMS;

namespace ENTITY
{
    public class Tarea
    {
        public int IdTarea { get; set; }

        /// <summary>
        /// Especialización requerida para esta tarea (texto libre, ej. "Frontend", "C#").
        /// </summary>
        public string EspecializacionRequerida { get; set; } = string.Empty;

        // Llaves foráneas opcionales para la jerarquía de tareas
        public int? IdEmpresa { get; set; }
        public int? IdProyecto { get; set; }
        public int? IdEquipo { get; set; }

        public string Titulo { get; set; }

        /// <summary>
        /// Origen de la tarea: "Manual" (creada por el usuario) o "IA" (generada por el flujo n8n).
        /// </summary>
        public string Origen { get; set; } = "Manual";

        /// <summary>
        /// Id del AnalisisTecnico que originó esta tarea. Nulo si fue creada manualmente.
        /// </summary>
        public int? IdAnalisis { get; set; }

        public string Descripcion { get; set; }

        public int? Prioridad { get; set; }

        public EstadoTarea estadoTarea { get; set; }
        //enum

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaLimite { get; set; }

        public DateTime FechaCreacion { get; set; }

        public Empresa Empresa { get; set; }
        public Proyecto Proyecto { get; set; }
        public Equipo Equipo { get; set; }
        // Nueva propiedad: indica si la tarea está disponible para trabajar
        public bool Disponible { get; set; } = false;

        // Nueva propiedad: lista de IDs de tareas de las que depende esta tarea
        public List<int> Dependencias { get; set; } = new List<int>();

    }
}
