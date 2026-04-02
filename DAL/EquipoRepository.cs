using ENTITY;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class EquipoRepository : IRepository<Equipo>
    {
        private readonly DB_Context _context;

        public EquipoRepository()
        {
            _context = new DB_Context();
        }

        public IEnumerable<Equipo> GetAll()
        {
            return _context.Equipos.AsNoTracking().ToList();
        }

        public Equipo GetById(int id)
        {
            return _context.Equipos
                .AsNoTracking()
                .FirstOrDefault(e => e.IdEquipo == id && e.Activo == 1);
        }

        public List<Equipo> GetByProyecto(int idProyecto)
        {
            return _context.Equipos
                .AsNoTracking()
                .Where(e => e.IdProyecto == idProyecto && e.Activo == 1)
                .OrderBy(e => e.Nombre)
                .ToList();
        }

        public bool ExisteNombre(int idProyecto, string nombre, int? exceptoIdEquipo = null)
        {
            return _context.Equipos.Any(e =>
                e.IdProyecto == idProyecto &&
                e.Activo == 1 &&
                e.Nombre.ToLower() == nombre.ToLower() &&
                e.IdEquipo != (exceptoIdEquipo ?? 0));
        }

        public int ContarMiembros(int idEquipo)
        {
            return _context.EquipoUsuarios
                .Count(eu => eu.IdEquipo == idEquipo && eu.Activo == 1);
        }

        public List<EquipoUsuario> GetMiembros(int idEquipo)
        {
            return _context.EquipoUsuarios
                .AsNoTracking()
                .Where(eu => eu.IdEquipo == idEquipo && eu.Activo == 1)
                .ToList();
        }

        public void Add(Equipo entity)
        {
            entity.FechaCreacion = DateTime.Now;
            entity.Activo = 1;
            _context.Equipos.Add(entity);
        }

        public void Update(Equipo entity)
        {
            var existing = _context.Equipos.Find(entity.IdEquipo);
            if (existing == null) return;

            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
        }

        public void SoftDelete(int id)
        {
            var entity = _context.Equipos.Find(id);
            if (entity != null)
                entity.Activo = 0;
        }

        // Eliminar físico (requerido por IRepository)
        public void Delete(int id)
        {
            SoftDelete(id);
        }

        public void ReemplazarMiembros(int idEquipo, List<int> nuevosIdsUsuario)
        {
            // Quitar todos los miembros actuales
            var actuales = _context.EquipoUsuarios
                .Where(eu => eu.IdEquipo == idEquipo)
                .ToList();

            _context.EquipoUsuarios.RemoveRange(actuales);

            // Agregar los nuevos
            foreach (var idUsuario in nuevosIdsUsuario)
            {
                _context.EquipoUsuarios.Add(new EquipoUsuario
                {
                    IdEquipo = idEquipo,
                    IdUsuario = idUsuario,
                    FechaAsignacion = DateTime.Now,
                    Activo = 1
                });
            }
        }

        public void AgregarMiembros(int idEquipo, List<int> idsUsuario)
        {
            foreach (var idUsuario in idsUsuario)
            {
                _context.EquipoUsuarios.Add(new EquipoUsuario
                {
                    IdEquipo = idEquipo,
                    IdUsuario = idUsuario,
                    FechaAsignacion = DateTime.Now,
                    Activo = 1
                });
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
