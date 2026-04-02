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
        /// Busca un usuario activo y con rol Empleado por su correo.
        /// Retorna null si no existe.
        /// </summary>
        public MiembroDto BuscarEmpleadoPorCorreo(string correo)
        {
            var usuario = _usuarioRepo.GetAll()
                .FirstOrDefault(u =>
                    u.Email.ToLower() == correo.ToLower() &&
                    u.Activo == 1 &&
                    u.Rol == ENTITY.ENUMS.RolUsuario.Empleado);

            if (usuario == null) return null;

            return new MiembroDto
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                Correo = usuario.Email
            };
        }

        /// <summary>
        /// Crea un nuevo equipo con sus miembros iniciales.
        /// </summary>
        public void CrearEquipo(Equipo equipo, List<int> idsUsuarios)
        {
            Validar(equipo, null);

            if (_repo.ExisteNombre(equipo.IdProyecto, equipo.Nombre))
                throw new Exception("Ya existe un equipo con ese nombre en este proyecto.");

            _repo.Add(equipo);
            _repo.Save();

            if (idsUsuarios != null && idsUsuarios.Count > 0)
            {
                _repo.AgregarMiembros(equipo.IdEquipo, idsUsuarios);
                _repo.Save();
            }
        }

        /// <summary>
        /// Edita un equipo existente y reemplaza sus miembros.
        /// </summary>
        public void EditarEquipo(Equipo equipo, List<int> idsUsuarios)
        {
            Validar(equipo, equipo.IdEquipo);

            if (_repo.ExisteNombre(equipo.IdProyecto, equipo.Nombre, equipo.IdEquipo))
                throw new Exception("Ya existe un equipo con ese nombre en este proyecto.");

            _repo.Update(equipo);
            _repo.ReemplazarMiembros(equipo.IdEquipo, idsUsuarios ?? new List<int>());
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
