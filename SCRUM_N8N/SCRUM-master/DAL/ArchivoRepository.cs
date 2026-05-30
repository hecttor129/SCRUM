using ENTITY;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ArchivoRepository : IRepository<Archivo>
    {
        private readonly DB_Context _context;

        public ArchivoRepository()
        {
            _context = new DB_Context();
        }

        public IEnumerable<Archivo> GetAll()
        {
            return _context.Archivos
                .Include(a => a.UsuarioSubidoPor)
                .ToList();
        }

        public Archivo GetById(int id)
        {
            return _context.Archivos
                .Include(a => a.UsuarioSubidoPor)
                .FirstOrDefault(a => a.IdArchivo == id);
        }

        public void Add(Archivo entity)
        {
            _context.Archivos.Add(entity);
        }

        public void Update(Archivo entity)
        {
            var tracked = _context.Archivos.Local.FirstOrDefault(a => a.IdArchivo == entity.IdArchivo);
            if (tracked != null)
            {
                _context.Entry(tracked).State = EntityState.Detached;
            }
            _context.Archivos.Update(entity);
        }

        public void Delete(int id)
        {
            var archivo = GetById(id);
            if (archivo != null)
            {
                _context.Archivos.Remove(archivo);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Archivo> GetByEquipo(int idEquipo)
        {
            return _context.Archivos
                .Include(a => a.UsuarioSubidoPor)
                .Where(a => a.IdEquipo == idEquipo)
                .OrderByDescending(a => a.FechaSubida)
                .ToList();
        }

        public List<Archivo> GetByProyectoAndAllTeams(int idProyecto)
        {
            var idsEquipos = _context.Equipos
                .Where(e => e.IdProyecto == idProyecto)
                .Select(e => e.IdEquipo)
                .ToList();

            return _context.Archivos
                .Include(a => a.UsuarioSubidoPor)
                .Include(a => a.Equipo)
                .Where(a => a.IdProyecto == idProyecto || (a.IdEquipo.HasValue && idsEquipos.Contains(a.IdEquipo.Value)))
                .OrderByDescending(a => a.FechaSubida)
                .ToList();
        }
    }
}
