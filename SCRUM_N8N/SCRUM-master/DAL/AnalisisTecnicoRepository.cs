using ENTITY;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class AnalisisTecnicoRepository
    {
        private readonly DB_Context _context;

        public AnalisisTecnicoRepository()
        {
            _context = new DB_Context();
        }

        public void Add(AnalisisTecnico entity)
        {
            _context.AnalisisTecnicos.Add(entity);
        }

        public void Update(AnalisisTecnico entity)
        {
            var tracked = _context.AnalisisTecnicos.Local
                .FirstOrDefault(a => a.IdAnalisis == entity.IdAnalisis);
            if (tracked != null)
                _context.Entry(tracked).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            _context.AnalisisTecnicos.Update(entity);
        }

        public AnalisisTecnico GetById(int id)
        {
            return _context.AnalisisTecnicos.FirstOrDefault(a => a.IdAnalisis == id);
        }

        public List<AnalisisTecnico> GetByProyecto(int idProyecto)
        {
            return _context.AnalisisTecnicos
                .Where(a => a.IdProyecto == idProyecto)
                .OrderByDescending(a => a.FechaAnalisis)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
