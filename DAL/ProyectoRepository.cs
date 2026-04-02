using ENTITY;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ProyectoRepository : IRepository<Proyecto>
    {
        private readonly DB_Context _context;

        public ProyectoRepository()
        {
            _context = new DB_Context();
        }

        public IEnumerable<Proyecto> GetAll()
        {
            return _context.Proyectos.AsNoTracking().ToList();
        }

        public Proyecto GetById(int id)
        {
            return _context.Proyectos
                .AsNoTracking()
                .FirstOrDefault(p => p.IdProyecto == id);
        }

        public List<Proyecto> GetByEmpresa(int idEmpresa)
        {
            return _context.Proyectos
                .AsNoTracking()
                .Where(p => p.IdEmpresa == idEmpresa)
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public bool ExisteEnEmpresa(int idProyecto, int idEmpresa)
        {
            return _context.Proyectos
                .AsNoTracking()
                .Any(p => p.IdProyecto == idProyecto && p.IdEmpresa == idEmpresa);
        }

        public bool EsSupervisor(int idProyecto, int idUsuario)
        {
            return _context.Proyectos
                .AsNoTracking()
                .Any(p => p.IdProyecto == idProyecto
                          && p.IdSupervisor == idUsuario
                          && p.Activo == 1);
        }

        public void Add(Proyecto entity)
        {
            entity.FechaCreacion = DateTime.Now;
            entity.Activo = 1;
            _context.Proyectos.Add(entity);
        }

        public void Update(Proyecto entity)
        {
            var existing = _context.Proyectos.Find(entity.IdProyecto);
            if (existing == null) return;

            existing.IdSupervisor = entity.IdSupervisor;
            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
            existing.Estado = entity.Estado;
            existing.FechaInicio = entity.FechaInicio;
            existing.FechaFin = entity.FechaFin;
            existing.Progreso = entity.Progreso;
        }

        public void Delete(int id)
        {
            var entity = _context.Proyectos.Find(id);
            if (entity != null)
                _context.Proyectos.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
