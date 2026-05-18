using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class TareaRepository : IRepository<Tarea>
    {
        private readonly DB_Context _context;

        public TareaRepository()
        {
            _context = new DB_Context();
        }

        public IEnumerable<Tarea> GetAll()
        {
            return _context.Tareas.ToList();
        }

        public Tarea GetById(int id)
        {
            return _context.Tareas.FirstOrDefault(t => t.IdTarea == id);
        }

        public void Add(Tarea entity)
        {
            _context.Tareas.Add(entity);
        }

        public void Update(Tarea entity)
        {
            _context.Tareas.Update(entity);
        }

        public void Delete(int id)
        {
            var tarea = GetById(id);
            if (tarea != null)
            {
                _context.Tareas.Remove(tarea);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        // ── Consultas Específicas ───────────────────────────────────────────

        public List<Tarea> GetByEmpresa(int idEmpresa)
        {
            return _context.Tareas
                .Where(t => t.IdEmpresa == idEmpresa)
                .OrderByDescending(t => t.FechaCreacion)
                .ToList();
        }

        public List<Tarea> GetByProyecto(int idProyecto)
        {
            return _context.Tareas
                .Where(t => t.IdProyecto == idProyecto)
                .OrderByDescending(t => t.FechaCreacion)
                .ToList();
        }

        public List<Tarea> GetByEquipo(int idEquipo)
        {
            return _context.Tareas
                .Where(t => t.IdEquipo == idEquipo)
                .OrderByDescending(t => t.FechaCreacion)
                .ToList();
        }
    }
}
