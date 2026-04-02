using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    /// <summary>
    /// DTO para mostrar proyectos en la vista sin exponer la entidad directamente.
    /// </summary>
    public class ProyectoDto
    {
        public int IdProyecto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Supervisor { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaFin { get; set; } = string.Empty;
        public string Progreso { get; set; } = string.Empty;
    }

    /// <summary>
    /// Service para operaciones de negocio relacionadas con Proyecto.
    /// </summary>
    public class ProyectoService
    {
        private readonly ProyectoRepository _repo;
        private readonly UsuarioRepository _usuarioRepo;

        public ProyectoService()
        {
            _repo = new ProyectoRepository();
            _usuarioRepo = new UsuarioRepository();
        }

        /// <summary>
        /// Retorna todos los proyectos de una empresa como DTOs listos para la vista.
        /// </summary>
        public List<ProyectoDto> ObtenerProyectosPorEmpresa(int idEmpresa)
        {
            var proyectos = _repo.GetByEmpresa(idEmpresa);

            var idsSupervisores = proyectos
                .Where(p => p.IdSupervisor.HasValue)
                .Select(p => p.IdSupervisor.Value)
                .Distinct()
                .ToList();

            var supervisores = _usuarioRepo
                .GetByIds(idsSupervisores)
                .ToDictionary(
                    u => u.IdUsuario,
                    u => string.IsNullOrWhiteSpace($"{u.Nombre} {u.Apellido}".Trim())
                        ? "Sin nombre"
                        : $"{u.Nombre} {u.Apellido}".Trim()
                );

            return proyectos.Select(p => new ProyectoDto
            {
                IdProyecto = p.IdProyecto,
                Nombre = string.IsNullOrWhiteSpace(p.Nombre) ? "Sin nombre" : p.Nombre.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(p.Descripcion) ? "-" : p.Descripcion.Trim(),
                Supervisor = p.IdSupervisor.HasValue && supervisores.ContainsKey(p.IdSupervisor.Value)
                    ? supervisores[p.IdSupervisor.Value]
                    : "Sin asignar",
                Estado = string.IsNullOrWhiteSpace(p.Estado) ? "Sin estado" : p.Estado.Trim(),
                FechaInicio = p.FechaInicio.HasValue ? p.FechaInicio.Value.ToString("dd/MM/yyyy") : "-",
                FechaFin = p.FechaFin.HasValue ? p.FechaFin.Value.ToString("dd/MM/yyyy") : "-",
                Progreso = $"{(p.Progreso ?? 0):0.##}%"
            }).ToList();
        }

        /// <summary>
        /// Retorna un proyecto por su ID y empresa, o null si no existe.
        /// </summary>
        public Proyecto ObtenerPorId(int idProyecto, int idEmpresa)
        {
            var proyecto = _repo.GetById(idProyecto);
            if (proyecto == null || proyecto.IdEmpresa != idEmpresa)
                return null;
            return proyecto;
        }

        /// <summary>
        /// Lista todos los usuarios activos como items para ComboBox.
        /// </summary>
        public List<UsuarioComboItem> ObtenerSupervisoresDisponibles()
        {
            var usuarios = _usuarioRepo.GetAll()
                .Select(u => new UsuarioComboItem
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}".Trim()
                })
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            usuarios.Insert(0, new UsuarioComboItem { IdUsuario = 0, NombreCompleto = "Sin asignar" });
            return usuarios;
        }

        /// <summary>
        /// Crea un nuevo proyecto aplicando validaciones de negocio.
        /// </summary>
        public void CrearProyecto(Proyecto proyecto)
        {
            Validar(proyecto);
            _repo.Add(proyecto);
            _repo.Save();
        }

        /// <summary>
        /// Edita un proyecto existente aplicando validaciones de negocio.
        /// </summary>
        public void EditarProyecto(Proyecto proyecto)
        {
            Validar(proyecto);
            _repo.Update(proyecto);
            _repo.Save();
        }

        /// <summary>
        /// Elimina físicamente un proyecto.
        /// </summary>
        public void EliminarProyecto(int idProyecto)
        {
            _repo.Delete(idProyecto);
            _repo.Save();
        }

        // ── Validaciones centralizadas ──────────────────────────────────────
        private static void Validar(Proyecto p)
        {
            if (string.IsNullOrWhiteSpace(p.Nombre))
                throw new Exception("El nombre del proyecto es obligatorio.");

            if (p.Nombre.Length > 80)
                throw new Exception("El nombre no puede superar 80 caracteres.");

            if (!string.IsNullOrWhiteSpace(p.Descripcion) && p.Descripcion.Length > 250)
                throw new Exception("La descripción no puede superar 250 caracteres.");

            if (string.IsNullOrWhiteSpace(p.Estado))
                throw new Exception("El estado del proyecto es obligatorio.");

            if (p.FechaInicio.HasValue && p.FechaFin.HasValue && p.FechaFin.Value.Date < p.FechaInicio.Value.Date)
                throw new Exception("La fecha fin no puede ser menor que la fecha inicio.");
        }
    }

    /// <summary>
    /// Item para poblar ComboBox de supervisores en la vista.
    /// </summary>
    public class UsuarioComboItem
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
    }
}
