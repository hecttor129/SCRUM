using System;

namespace ENTITY
{
    /// <summary>
    /// Representa un análisis técnico realizado por IA sobre un documento de requerimientos.
    /// Almacena tanto el resultado del análisis como el resultado de la generación de tareas.
    /// </summary>
    public class AnalisisTecnico
    {
        public int IdAnalisis { get; set; }

        /// <summary>Proyecto al que pertenece este análisis.</summary>
        public int IdProyecto { get; set; }

        /// <summary>Nombre del documento analizado (nombre original del archivo).</summary>
        public string NombreDocumento { get; set; } = string.Empty;

        /// <summary>JSON completo devuelto por el flujo de análisis de n8n.</summary>
        public string ResultadoJson { get; set; } = string.Empty;

        /// <summary>Fecha en que se ejecutó el análisis.</summary>
        public DateTime FechaAnalisis { get; set; }

        /// <summary>JSON completo devuelto por el flujo de generación de tareas de n8n. Nulo si aún no se generaron.</summary>
        public string? TareasGeneradas { get; set; }

        /// <summary>Fecha en que se ejecutó la generación de tareas. Nula si aún no se ejecutó.</summary>
        public DateTime? FechaGeneracionTareas { get; set; }

        /// <summary>Estado del análisis: "Completado", "ErrorAnalisis", "TareasGeneradas", etc.</summary>
        public string Estado { get; set; } = "Completado";

        // Propiedad de navegación
        public Proyecto? Proyecto { get; set; }
    }
}
