using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class TareaDto
    {
        public int IdTarea { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string FechaLimite { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public string DisponibleDisplay { get; set; } = string.Empty;
        public List<int> Dependencias { get; set; } = new();
        public string DependenciasDisplay { get; set; } = string.Empty;
    }

    public class TareaService
    {
        private readonly TareaRepository _repo;

        public TareaService()
        {
            _repo = new TareaRepository();
        }

        public List<TareaDto> ObtenerTareasPorEmpresa(int idEmpresa)
        {
            var tareas = _repo.GetByEmpresa(idEmpresa);
            return ConvertirADtos(tareas);
        }

        public List<TareaDto> ObtenerTareasPorProyecto(int idProyecto)
        {
            var tareas = _repo.GetByProyecto(idProyecto);
            return ConvertirADtos(tareas);
        }

        public List<TareaDto> ObtenerTareasPorEquipo(int idEquipo)
        {
            var tareas = _repo.GetByEquipo(idEquipo);
            return ConvertirADtos(tareas);
        }

        public void CrearTarea(Tarea tarea)
        {
            Validar(tarea);
            tarea.FechaCreacion = DateTime.Now;
            // Por defecto, calculamos la disponibilidad inicial antes de agregar
            tarea.Disponible = true;
            if (tarea.Dependencias != null && tarea.Dependencias.Count > 0)
            {
                tarea.Disponible = false; // Se reevaluará abajo
            }
            _repo.Add(tarea);
            _repo.Save();

            ReevaluarDisponibilidadTareas(tarea.IdEmpresa, tarea.IdProyecto, tarea.IdEquipo);
        }

        public void EditarTarea(Tarea tarea)
        {
            Validar(tarea);
            _repo.Update(tarea);
            _repo.Save();

            ReevaluarDisponibilidadTareas(tarea.IdEmpresa, tarea.IdProyecto, tarea.IdEquipo);
        }

        public void EliminarTarea(int idTarea)
        {
            _repo.Delete(idTarea);
            _repo.Save();
        }

        private List<TareaDto> ConvertirADtos(List<Tarea> tareas)
        {
            var titulosTareas = _repo.GetAll().ToDictionary(t => t.IdTarea, t => t.Titulo);

            return tareas.Select(t =>
            {
                var depsNombres = t.Dependencias != null
                    ? t.Dependencias.Select(id => titulosTareas.ContainsKey(id) ? titulosTareas[id] : $"#{id}").ToList()
                    : new List<string>();

                return new TareaDto
                {
                    IdTarea = t.IdTarea,
                    Titulo = t.Titulo,
                    Descripcion = string.IsNullOrWhiteSpace(t.Descripcion) ? "-" : t.Descripcion,
                    Estado = t.estadoTarea.ToString(),
                    Prioridad = t.Prioridad?.ToString() ?? "-",
                    FechaLimite = t.FechaLimite.HasValue ? t.FechaLimite.Value.ToString("dd/MM/yyyy") : "-",
                    FechaCreacion = t.FechaCreacion.ToString("dd/MM/yyyy"),
                    Disponible = t.Disponible,
                    DisponibleDisplay = t.Disponible ? "Sí" : "No",
                    Dependencias = t.Dependencias ?? new List<int>(),
                    DependenciasDisplay = depsNombres.Any() ? string.Join(", ", depsNombres) : "Ninguna"
                };
            }).ToList();
        }

        public void ReevaluarDisponibilidadTareas(int? idEmpresa, int? idProyecto, int? idEquipo)
        {
            List<Tarea> tareas;
            if (idEquipo.HasValue) tareas = _repo.GetByEquipo(idEquipo.Value);
            else if (idProyecto.HasValue) tareas = _repo.GetByProyecto(idProyecto.Value);
            else if (idEmpresa.HasValue) tareas = _repo.GetByEmpresa(idEmpresa.Value);
            else return;

            bool huboCambios = false;
            // Algoritmo de punto fijo simple para reevaluar disponibilidades
            for (int i = 0; i < tareas.Count; i++)
            {
                bool pasadaCambio = false;
                foreach (var t in tareas)
                {
                    bool nuevaDisp = true;
                    if (t.Dependencias != null && t.Dependencias.Count > 0)
                    {
                        foreach (int depId in t.Dependencias)
                        {
                            var depTarea = tareas.FirstOrDefault(x => x.IdTarea == depId);
                            if (depTarea == null || depTarea.estadoTarea != ENTITY.ENUMS.EstadoTarea.Completada)
                            {
                                nuevaDisp = false;
                                break;
                            }
                        }
                    }

                    if (t.Disponible != nuevaDisp)
                    {
                        t.Disponible = nuevaDisp;
                        _repo.Update(t);
                        pasadaCambio = true;
                        huboCambios = true;
                    }
                }
                if (!pasadaCambio) break;
            }

            if (huboCambios)
            {
                _repo.Save();
            }
        }

        /// <summary>
        /// Crea múltiples tareas generadas por IA asociadas a un análisis.
        /// Todas se marcan con Origen="IA" y el IdAnalisis proporcionado.
        /// </summary>
        public void CrearTareasDesdeIA(List<Tarea> tareas, int idAnalisis)
        {
            foreach (var tarea in tareas)
            {
                tarea.Origen     = "IA";
                tarea.IdAnalisis = idAnalisis;
                tarea.FechaCreacion = DateTime.Now;
                tarea.Disponible = tarea.Dependencias == null || tarea.Dependencias.Count == 0;
                _repo.Add(tarea);
            }
            _repo.Save();

            // Reevaluar disponibilidad con el contexto del primer elemento
            if (tareas.Count > 0)
            {
                var t = tareas[0];
                ReevaluarDisponibilidadTareas(t.IdEmpresa, t.IdProyecto, t.IdEquipo);
            }
        }

        /// <summary>
        /// Retorna la entidad Tarea completa por Id.
        /// </summary>
        public Tarea ObtenerPorId(int idTarea)
            => _repo.GetById(idTarea);

        private void Validar(Tarea t)
        {
            if (string.IsNullOrWhiteSpace(t.Titulo))
                throw new Exception("El título de la tarea es obligatorio.");

            if (t.Titulo.Length > 500)
                throw new Exception("El título no puede superar los 500 caracteres.");

            int nivelesAsignados = 0;
            if (t.IdEmpresa.HasValue) nivelesAsignados++;
            if (t.IdProyecto.HasValue) nivelesAsignados++;
            if (t.IdEquipo.HasValue) nivelesAsignados++;

            if (nivelesAsignados == 0)
                throw new Exception("La tarea debe estar asignada a una Empresa, Proyecto o Equipo.");
            
            if (nivelesAsignados > 1)
                throw new Exception("La tarea solo puede estar asignada a UN nivel a la vez (Empresa, Proyecto o Equipo).");

            if (t.FechaInicio.HasValue && t.FechaLimite.HasValue)
            {
                if (t.FechaLimite.Value < t.FechaInicio.Value)
                    throw new Exception("La fecha límite no puede ser anterior a la fecha de inicio.");
            }

            // 1. Validación de prioridad
            if (t.Prioridad.HasValue && (t.Prioridad.Value < 1 || t.Prioridad.Value > 5))
            {
                throw new Exception("La prioridad de la tarea debe estar entre 1 (Muy baja) y 5 (Muy alta).");
            }

            // 2. Validación de Autodependencia
            if (t.IdTarea > 0 && t.Dependencias != null && t.Dependencias.Contains(t.IdTarea))
            {
                throw new Exception("Una tarea no puede depender de sí misma.");
            }

            // 3. Validación de dependencias circulares directas
            if (t.Dependencias != null)
            {
                foreach (int depId in t.Dependencias)
                {
                    var depTarea = _repo.GetById(depId);
                    if (depTarea != null && depTarea.Dependencias != null && depTarea.Dependencias.Contains(t.IdTarea))
                    {
                        throw new Exception($"Se detectó una dependencia circular directa: la tarea '{depTarea.Titulo}' ya depende de esta tarea.");
                    }
                }
            }

            // 4. Validación de Disponibilidad: si no está disponible, solo puede estar en Pendiente o Cancelada
            bool realmenteDisponible = true;
            if (t.Dependencias != null && t.Dependencias.Count > 0)
            {
                foreach (int depId in t.Dependencias)
                {
                    var depTarea = _repo.GetById(depId);
                    if (depTarea == null || depTarea.estadoTarea != ENTITY.ENUMS.EstadoTarea.Completada)
                    {
                        realmenteDisponible = false;
                        break;
                    }
                }
            }

            if (!realmenteDisponible)
            {
                if (t.estadoTarea == ENTITY.ENUMS.EstadoTarea.EnProgreso || t.estadoTarea == ENTITY.ENUMS.EstadoTarea.Completada)
                {
                    throw new Exception("La tarea tiene dependencias pendientes de completar, por lo tanto solo puede estar en estado Pendiente o Cancelada.");
                }
            }
        }
    }
}
