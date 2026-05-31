using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    /// <summary>
    /// DTO para mostrar equipos en la vista.
    /// </summary>
    public class EquipoDto
    {
        public int IdEquipo { get; set; }
        public int IdProyecto { get; set; }
        public string NombreProyecto { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Supervisor { get; set; } = string.Empty;
        public int Trabajadores { get; set; }
        public string FechaCreacion { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para mostrar miembros de un equipo en la vista.
    /// </summary>
    public class MiembroDto
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }

    /// <summary>
    /// Service para operaciones de negocio relacionadas con Equipo.
    /// </summary>
    public class EquipoService
    {
        private readonly EquipoRepository _repo;
        private readonly UsuarioRepository _usuarioRepo;

        public EquipoService()
        {
            _repo = new EquipoRepository();
            _usuarioRepo = new UsuarioRepository();
        }

        /// <summary>
        /// Retorna todos los equipos activos de un proyecto como DTOs.
        /// </summary>
        public List<EquipoDto> ObtenerEquiposPorProyecto(int idProyecto)
        {
            var equipos = _repo.GetByProyecto(idProyecto);

            var idsSupervisores = equipos
                .Select(e => e.IdSupervisor)
                .Distinct()
                .ToList();

            var supervisores = _usuarioRepo
                .GetByIds(idsSupervisores)
                .ToDictionary(
                    u => u.IdUsuario,
                    u => $"{u.Nombre} {u.Apellido}".Trim()
                );

            return equipos.Select(e => new EquipoDto
            {
                IdEquipo = e.IdEquipo,
                IdProyecto = idProyecto,
                Nombre = e.Nombre,
                Descripcion = string.IsNullOrWhiteSpace(e.Descripcion) ? "-" : e.Descripcion,
                Supervisor = supervisores.ContainsKey(e.IdSupervisor)
                    ? supervisores[e.IdSupervisor]
                    : "Sin supervisor",
                Trabajadores = _repo.ContarMiembros(e.IdEquipo),
                FechaCreacion = e.FechaCreacion.HasValue
                    ? e.FechaCreacion.Value.ToString("dd/MM/yyyy")
                    : "-"
            }).ToList();
        }

        /// <summary>
        /// Retorna todos los equipos activos de una empresa (de todos sus proyectos).
        /// </summary>
        public List<EquipoDto> ObtenerEquiposPorEmpresa(int idEmpresa)
        {
            var resultados = _repo.GetByEmpresa(idEmpresa);

            var idsSupervisores = resultados
                .Select(r => r.Equipo.IdSupervisor)
                .Distinct()
                .ToList();

            var supervisores = _usuarioRepo
                .GetByIds(idsSupervisores)
                .ToDictionary(
                    u => u.IdUsuario,
                    u => $"{u.Nombre} {u.Apellido}".Trim()
                );

            return resultados.Select(r => new EquipoDto
            {
                IdEquipo = r.Equipo.IdEquipo,
                IdProyecto = r.IdProyecto,
                NombreProyecto = r.NombreProyecto,
                Nombre = r.Equipo.Nombre,
                Descripcion = string.IsNullOrWhiteSpace(r.Equipo.Descripcion) ? "-" : r.Equipo.Descripcion,
                Supervisor = supervisores.ContainsKey(r.Equipo.IdSupervisor)
                    ? supervisores[r.Equipo.IdSupervisor]
                    : "Sin supervisor",
                Trabajadores = _repo.ContarMiembros(r.Equipo.IdEquipo),
                FechaCreacion = r.Equipo.FechaCreacion.HasValue
                    ? r.Equipo.FechaCreacion.Value.ToString("dd/MM/yyyy")
                    : "-"
            }).ToList();
        }

        /// <summary>
        /// Retorna un equipo por su ID, o null si no existe / está inactivo.
        /// </summary>
        public Equipo ObtenerPorId(int idEquipo)
        {
            return _repo.GetById(idEquipo);
        }

        /// <summary>
        /// Retorna los miembros activos de un equipo como DTOs.
        /// </summary>
        public List<MiembroDto> ObtenerMiembros(int idEquipo)
        {
            var miembros = _repo.GetMiembros(idEquipo);
            var ids = miembros.Select(m => m.IdUsuario).ToList();
            var usuarios = _usuarioRepo.GetByIds(ids).ToDictionary(u => u.IdUsuario);

            return miembros
                .Where(m => usuarios.ContainsKey(m.IdUsuario))
                .Select(m => new MiembroDto
                {
                    IdUsuario = m.IdUsuario,
                    NombreCompleto = $"{usuarios[m.IdUsuario].Nombre} {usuarios[m.IdUsuario].Apellido}".Trim(),
                    Correo = usuarios[m.IdUsuario].Email
                })
                .OrderBy(m => m.NombreCompleto)
                .ToList();
        }

        /// <summary>
        /// Retorna todos los Jefes y Empleados activos para mostrarlos en el buscador.
        /// </summary>
        public List<MiembroDto> ObtenerCandidatosEquipo()
        {
            return _usuarioRepo.GetAll()
                .Where(u => u.Activo == 1 && u.Rol != ENTITY.ENUMS.RolUsuario.Admin)
                .Select(u => new MiembroDto
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}".Trim(),
                    Correo = u.Email
                })
                .OrderBy(m => m.NombreCompleto)
                .ToList();
        }

        /// <summary>
        /// Retorna los supervisores válidos (Admin y Jefes activos).
        /// </summary>
        public List<MiembroDto> ObtenerSupervisoresEquipo()
        {
            return _usuarioRepo.GetSupervisoresDisponibles()
                .Select(u => new MiembroDto
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}".Trim(),
                    Correo = u.Email
                })
                .ToList();
        }

        /// <summary>
        /// Retorna el nombre completo del supervisor.
        /// </summary>
        public string ObtenerNombreSupervisor(int idSupervisor)
        {
            var supervisor = _usuarioRepo.GetById(idSupervisor);
            if (supervisor == null) return "Sin supervisor";
            return $"{supervisor.Nombre} {supervisor.Apellido}".Trim();
        }

        /// <summary>
        /// Crea o edita un equipo con sus miembros iniciales y supervisor.
        /// </summary>
        public void GuardarEquipo(int idProyecto, int? idEquipo, string nombre, string descripcion, List<int> idsUsuarios, int idSupervisor)
        {
            Equipo equipo;
            if (idEquipo.HasValue && idEquipo.Value > 0)
            {
                equipo = _repo.GetById(idEquipo.Value) ?? throw new Exception("Equipo no encontrado.");
                equipo.Nombre = nombre;
                equipo.Descripcion = descripcion;
                equipo.IdSupervisor = idSupervisor;

                Validar(equipo, equipo.IdEquipo);

                if (_repo.ExisteNombre(equipo.IdProyecto, equipo.Nombre, equipo.IdEquipo))
                    throw new Exception("Ya existe un equipo con ese nombre en este proyecto.");

                _repo.Update(equipo);
                _repo.ReemplazarMiembros(equipo.IdEquipo, idsUsuarios ?? new List<int>());
            }
            else
            {
                equipo = new Equipo
                {
                    IdProyecto = idProyecto,
                    IdSupervisor = idSupervisor,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Activo = 1,
                    FechaCreacion = DateTime.Now
                };

                Validar(equipo, null);

                if (_repo.ExisteNombre(equipo.IdProyecto, equipo.Nombre))
                    throw new Exception("Ya existe un equipo con ese nombre en este proyecto.");

                _repo.Add(equipo);
                _repo.Save();

                if (idsUsuarios != null && idsUsuarios.Count > 0)
                {
                    _repo.AgregarMiembros(equipo.IdEquipo, idsUsuarios);
                }
            }

            _repo.Save();
        }

        /// <summary>
        /// Marca un equipo como inactivo (soft delete).
        /// </summary>
        public void EliminarEquipo(int idEquipo)
        {
            _repo.SoftDelete(idEquipo);
            _repo.Save();
        }

        // ── Validaciones centralizadas ──────────────────────────────────────
        private static void Validar(Equipo e, int? idEquipoEdicion)
        {
            if (string.IsNullOrWhiteSpace(e.Nombre))
                throw new Exception("El nombre del equipo es obligatorio.");

            if (e.Nombre.Length > 80)
                throw new Exception("El nombre no puede superar 80 caracteres.");

            if (!string.IsNullOrWhiteSpace(e.Descripcion) && e.Descripcion.Length > 250)
                throw new Exception("La descripción no puede superar 250 caracteres.");
        }
    }
}
