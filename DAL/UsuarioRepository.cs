using ENTITY;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class UsuarioRepository : IRepository<Usuario>
    {
        private readonly DB_Context _context;

        public UsuarioRepository()
        {
            _context = new DB_Context();
        }

        public IEnumerable<Usuario> GetAll()
        {
            return _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Activo == 1)
                .OrderBy(u => u.Nombre)
                .ToList();
        }

        public Usuario GetById(int id)
        {
            return _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.IdUsuario == id);
        }

        public List<Usuario> GetByIds(List<int> ids)
        {
            return _context.Usuarios
                .AsNoTracking()
                .Where(u => ids.Contains(u.IdUsuario))
                .ToList();
        }

        public void Add(Usuario entity)
        {
            _context.Usuarios.Add(entity);
        }

        public void Update(Usuario entity)
        {
            var existing = _context.Usuarios.Find(entity.IdUsuario);
            if (existing == null) return;

            existing.Nombre = entity.Nombre;
            existing.Apellido = entity.Apellido;
            existing.Email = entity.Email;
            existing.Rol = entity.Rol;
            existing.NivelJerarquico = entity.NivelJerarquico;
            existing.Salario = entity.Salario;
            existing.Activo = entity.Activo;
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
    }
}
