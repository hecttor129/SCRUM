using ENTITY;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class EmpresaRepository : IRepository<Empresa>
    {
        private readonly DB_Context _context;

        public EmpresaRepository()
        {
            _context = new DB_Context();
        }

        public Empresa GetFirst()
        {
            return _context.Empresas
                .AsNoTracking()
                .FirstOrDefault();
        }

        public IEnumerable<Empresa> GetAll()
        {
            return _context.Empresas.AsNoTracking().ToList();
        }

        public Empresa GetById(int id)
        {
            return _context.Empresas
                .AsNoTracking()
                .FirstOrDefault(e => e.IdEmpresa == id);
        }

        public void Add(Empresa entity)
        {
            _context.Empresas.Add(entity);
        }

        public void Update(Empresa entity)
        {
            var existing = _context.Empresas.Find(entity.IdEmpresa);
            if (existing == null) return;

            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
            existing.Nit = entity.Nit;
            existing.Direccion = entity.Direccion;
            existing.Correo = entity.Correo;
            existing.Telefono = entity.Telefono;
            existing.Activo = entity.Activo;
        }

        public void Delete(int id)
        {
            var entity = _context.Empresas.Find(id);
            if (entity != null)
                _context.Empresas.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
