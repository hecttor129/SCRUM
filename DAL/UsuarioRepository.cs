using ENTITY;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static ENTITY.ENUMS;

namespace DAL
{
    public class UsuarioRepository : IRepository<Usuario>
    {
        private readonly DB_Context _context;

        public UsuarioRepository()
        {
            _context = new DB_Context();
        }

        // ── IRepository<T> ──────────────────────────────────────────────────

        public IEnumerable<Usuario> GetAll()
        {
            return _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Activo == 1)
                .OrderBy(u => u.NivelJerarquico)
                .ThenBy(u => u.Nombre)
                .ToList();
        }

        public Usuario GetById(int id)
        {
            return _context.Usuarios
                .FirstOrDefault(u => u.IdUsuario == id);
        }

        public void Add(Usuario entity)
        {
            _context.Usuarios.Add(entity);
        }

        public void Update(Usuario entity)
        {
            var existing = _context.Usuarios.Find(entity.IdUsuario);
            if (existing == null) return;

            existing.Nombre          = entity.Nombre;
            existing.Apellido        = entity.Apellido;
            existing.Email           = entity.Email;
            existing.password        = entity.password;
            existing.Rol             = entity.Rol;
            existing.NivelJerarquico = entity.NivelJerarquico;
            existing.Salario         = entity.Salario;
            existing.Activo          = entity.Activo;
        }

        public void Delete(int id)
        {
            var entity = _context.Usuarios.Find(id);
            if (entity != null)
                _context.Usuarios.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        // ── Métodos adicionales ──────────────────────────────────────────────

        /// <summary>
        /// Busca un usuario activo por email + hash de contraseña (para Login).
        /// </summary>
        public Usuario GetByEmailYPassword(string email, string hashPassword)
        {
            return _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u =>
                    u.Email == email &&
                    u.password == hashPassword &&
                    u.Activo == 1);
        }

        /// <summary>
        /// Verifica si un email ya está registrado.
        /// Se puede excluir un ID para no bloquear la edición del propio usuario.
        /// </summary>
        public bool EmailExiste(string email, int? exceptoIdUsuario = null)
        {
            return _context.Usuarios
                .AsNoTracking()
                .Any(u =>
                    u.Email.ToLower() == email.ToLower() &&
                    u.IdUsuario != (exceptoIdUsuario ?? 0));
        }

        /// <summary>
        /// Verifica si existe al menos un Admin activo en el sistema.
        /// </summary>
        public bool ExisteAdmin()
        {
            return _context.Usuarios
                .AsNoTracking()
                .Any(u => u.Rol == RolUsuario.Admin && u.Activo == 1);
        }

        /// <summary>
        /// Retorna todos los usuarios (activos e inactivos) ordenados por nivel y nombre.
        /// </summary>
        public List<Usuario> GetAllConInactivos()
        {
            return _context.Usuarios
                .AsNoTracking()
                .OrderByDescending(u => u.Activo)
                .ThenBy(u => u.NivelJerarquico)
                .ThenBy(u => u.Nombre)
                .ToList();
        }

        /// <summary>
        /// Retorna usuarios que pueden ser superiores: Admin + Jefes activos.
        /// </summary>
        public List<Usuario> GetSupervisoresDisponibles()
        {
            return _context.Usuarios
                .AsNoTracking()
                .Where(u =>
                    (u.Rol == RolUsuario.Admin || u.Rol == RolUsuario.Jefe) &&
                    u.Activo == 1)
                .OrderBy(u => u.NivelJerarquico)
                .ThenBy(u => u.Nombre)
                .ToList();
        }

        /// <summary>
        /// Retorna el Admin activo del sistema.
        /// </summary>
        public Usuario GetAdmin()
        {
            return _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Rol == RolUsuario.Admin && u.Activo == 1);
        }

        /// <summary>
        /// Carga múltiples usuarios por sus IDs (para cargar supervisores en bloque).
        /// </summary>
        public List<Usuario> GetByIds(List<int> ids)
        {
            return _context.Usuarios
                .AsNoTracking()
                .Where(u => ids.Contains(u.IdUsuario))
                .ToList();
        }
    }
}
