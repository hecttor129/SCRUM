using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace BLL
{
    // ── DTOs de presentación ────────────────────────────────────────────────

    /// <summary>DTO para mostrar en la tabla del Historial IA.</summary>
    public class AnalisisHistorialDto
    {
        public int IdAnalisis { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string TipoOperacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool TieneTareasGeneradas { get; set; }
    }

    /// <summary>DTO con detalle completo de un análisis para el panel lateral del historial.</summary>
    public class AnalisisDetalleDto
    {
        public int IdAnalisis { get; set; }
        public string NombreDocumento { get; set; } = string.Empty;
        public string FechaAnalisis { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string ResultadoJson { get; set; } = string.Empty;

        // Campos parseados del ResultadoJson para facilitar la visualización
        public string HuProcesadas { get; set; } = "—";
        public string RfProcesados { get; set; } = "—";
        public string ResumenAnalisis { get; set; } = string.Empty;

        // Campos de generación de tareas
        public bool TieneTareasGeneradas { get; set; }
        public string FechaGeneracionTareas { get; set; } = "—";
        public string TareasGeneradasJson { get; set; } = string.Empty;
        public string TareasGeneradasResumen { get; set; } = "—";
        public string EquiposAsignados { get; set; } = "—";
    }

    // ── Servicio ────────────────────────────────────────────────────────────

    public class AnalisisTecnicoService
    {
        private AnalisisTecnicoRepository _repo;

        public AnalisisTecnicoService()
        {
            _repo = new AnalisisTecnicoRepository();
        }

        /// <summary>
        /// Persiste el resultado de un análisis IA y retorna el IdAnalisis generado.
        /// </summary>
        public int GuardarAnalisis(int idProyecto, string nombreDocumento, string resultadoJson)
        {
            _repo = new AnalisisTecnicoRepository();

            var analisis = new AnalisisTecnico
            {
                IdProyecto      = idProyecto,
                NombreDocumento = nombreDocumento,
                ResultadoJson   = resultadoJson,
                FechaAnalisis   = DateTime.Now,
                Estado          = "Completado"
            };

            _repo.Add(analisis);
            _repo.Save();
            return analisis.IdAnalisis;
        }

        /// <summary>
        /// Persiste el JSON de tareas generadas sobre un análisis existente
        /// y actualiza su estado a "TareasGeneradas".
        /// </summary>
        public void GuardarResultadoTareas(int idAnalisis, string tareasJson)
        {
            _repo = new AnalisisTecnicoRepository();
            var analisis = _repo.GetById(idAnalisis)
                ?? throw new Exception($"No se encontró el análisis con Id {idAnalisis}.");

            analisis.TareasGeneradas         = tareasJson;
            analisis.FechaGeneracionTareas   = DateTime.Now;
            analisis.Estado                  = "TareasGeneradas";

            _repo.Update(analisis);
            _repo.Save();
        }

        /// <summary>
        /// Retorna la lista resumida para la tabla del Historial IA de un proyecto.
        /// Genera dos filas por análisis: una para el análisis y otra (si existe) para tareas.
        /// </summary>
        public List<AnalisisHistorialDto> ObtenerHistorialPorProyecto(int idProyecto)
        {
            _repo = new AnalisisTecnicoRepository();
            var lista = _repo.GetByProyecto(idProyecto);
            var resultado = new List<AnalisisHistorialDto>();

            foreach (var a in lista)
            {
                // Fila de análisis
                resultado.Add(new AnalisisHistorialDto
                {
                    IdAnalisis          = a.IdAnalisis,
                    Fecha               = a.FechaAnalisis.ToString("dd/MM/yyyy HH:mm"),
                    Documento           = a.NombreDocumento,
                    TipoOperacion       = "Análisis",
                    Estado              = a.Estado,
                    TieneTareasGeneradas = !string.IsNullOrWhiteSpace(a.TareasGeneradas)
                });

                // Fila de generación de tareas (si existe)
                if (!string.IsNullOrWhiteSpace(a.TareasGeneradas))
                {
                    resultado.Add(new AnalisisHistorialDto
                    {
                        IdAnalisis          = a.IdAnalisis,
                        Fecha               = a.FechaGeneracionTareas?.ToString("dd/MM/yyyy HH:mm") ?? "—",
                        Documento           = a.NombreDocumento,
                        TipoOperacion       = "Generación de tareas",
                        Estado              = "TareasGeneradas",
                        TieneTareasGeneradas = true
                    });
                }
            }

            return resultado;
        }

        /// <summary>
        /// Retorna el detalle completo de un análisis para el panel del historial.
        /// </summary>
        public AnalisisDetalleDto ObtenerDetalle(int idAnalisis)
        {
            _repo = new AnalisisTecnicoRepository();
            var a = _repo.GetById(idAnalisis)
                ?? throw new Exception($"No se encontró el análisis con Id {idAnalisis}.");

            var dto = new AnalisisDetalleDto
            {
                IdAnalisis           = a.IdAnalisis,
                NombreDocumento      = a.NombreDocumento,
                FechaAnalisis        = a.FechaAnalisis.ToString("dd/MM/yyyy HH:mm"),
                Estado               = a.Estado,
                ResultadoJson        = a.ResultadoJson,
                TieneTareasGeneradas = !string.IsNullOrWhiteSpace(a.TareasGeneradas),
                FechaGeneracionTareas = a.FechaGeneracionTareas?.ToString("dd/MM/yyyy HH:mm") ?? "—",
                TareasGeneradasJson  = a.TareasGeneradas ?? string.Empty
            };

            // Intentar parsear campos conocidos del JSON de análisis
            dto.HuProcesadas    = ExtraerCampoJson(a.ResultadoJson, "historias_usuario", "hu_procesadas", "HU", "historias");
            dto.RfProcesados    = ExtraerCampoJson(a.ResultadoJson, "requisitos_funcionales", "rf_procesados", "RF", "requisitos");
            dto.ResumenAnalisis = ExtraerResumen(a.ResultadoJson);

            // Campos de tareas generadas
            if (dto.TieneTareasGeneradas)
            {
                dto.TareasGeneradasResumen = ExtraerResumenTareas(a.TareasGeneradas!);
                dto.EquiposAsignados       = ExtraerCampoJson(a.TareasGeneradas!, "equipos", "equipos_asignados", "equipo");
            }

            return dto;
        }

        // ── Helpers de parseo JSON (resilientes ante estructuras desconocidas) ──

        private string ExtraerCampoJson(string json, params string[] posiblesCampos)
        {
            if (string.IsNullOrWhiteSpace(json)) return "—";
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Si es un array, buscar en el primer elemento
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    root = root[0];

                foreach (var campo in posiblesCampos)
                {
                    if (root.TryGetProperty(campo, out var prop))
                    {
                        if (prop.ValueKind == JsonValueKind.Array)
                            return $"{prop.GetArrayLength()} elemento(s)";
                        return prop.ToString();
                    }
                }
            }
            catch { /* JSON malformado → silencio */ }
            return "—";
        }

        private string ExtraerResumen(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return string.Empty;
            // Devolver el JSON formateado o las primeras 400 chars
            try
            {
                var obj = JsonDocument.Parse(json);
                var pretty = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                return pretty.Length > 1200 ? pretty[..1200] + "\n..." : pretty;
            }
            catch
            {
                return json.Length > 1200 ? json[..1200] + "..." : json;
            }
        }

        private string ExtraerResumenTareas(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return "—";
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Contar tareas si es un array
                if (root.ValueKind == JsonValueKind.Array)
                    return $"{root.GetArrayLength()} tarea(s) generada(s)";

                // Buscar campo "tareas"
                if (root.TryGetProperty("tareas", out var tareas) && tareas.ValueKind == JsonValueKind.Array)
                    return $"{tareas.GetArrayLength()} tarea(s) generada(s)";
            }
            catch { }

            return "Ver JSON de tareas para más detalle";
        }
    }
}
