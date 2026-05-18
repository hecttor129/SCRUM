using DAL;
using ENTITY;
using ENTITY.ENUMS;
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
            _repo.Add(tarea);
            _repo.Save();
        }

        public void EditarTarea(Tarea tarea)
        {
            Validar(tarea);
            _repo.Update(tarea);
            _repo.Save();
        }

        public void EliminarTarea(int idTarea)
        {
            _repo.Delete(idTarea);
            _repo.Save();
        }

        private List<TareaDto> ConvertirADtos(List<Tarea> tareas)
        {
            return tareas.Select(t => new TareaDto
            {
                IdTarea = t.IdTarea,
                Titulo = t.Titulo,
                Descripcion = string.IsNullOrWhiteSpace(t.Descripcion) ? "-" : t.Descripcion,
                Estado = t.estadoTarea.ToString(),
                Prioridad = t.Prioridad?.ToString() ?? "-",
                FechaLimite = t.FechaLimite.HasValue ? t.FechaLimite.Value.ToString("dd/MM/yyyy") : "-",
                FechaCreacion = t.FechaCreacion.ToString("dd/MM/yyyy")
            }).ToList();
        }

        private void Validar(Tarea t)
        {
            if (string.IsNullOrWhiteSpace(t.Titulo))
                throw new Exception("El título de la tarea es obligatorio.");

            if (t.Titulo.Length > 20)
                throw new Exception("El título no puede superar los 20 caracteres.");

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
        }
    }
}
