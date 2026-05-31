using ENTITY;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DAL
{
    public class RelacionJerarquicaRepository
    {
        private readonly DB_Context _context;

        public RelacionJerarquicaRepository()
        {
            _context = new DB_Context();
        }

        /// <summary>
        /// Crea una nueva relación jerárquica entre un jefe y un empleado.
        /// </summary>
        public void Crear(int idJefe, int idEmpleado)
        {
            _context.RelacionesJerarquicas.Add(new RelacionJerarquica
            {
                IdJefe     = idJefe,
                IdEmpleado = idEmpleado,
                FechaInicio = DateTime.Now,
                FechaFin   = null
            });
            _context.SaveChanges();
        }

        /// <summary>
        /// Retorna la relación activa (FechaFin == null) de un empleado con su superior.
        /// </summary>
        public RelacionJerarquica GetSuperiorActual(int idEmpleado)
        {
            return _context.RelacionesJerarquicas
                .AsNoTracking()
                .FirstOrDefault(r =>
                    r.IdEmpleado == idEmpleado &&
                    r.FechaFin == null);
        }

        /// <summary>
        /// Cierra la relación activa de un empleado (le pone FechaFin = hoy).
        /// Se usa cuando se cambia el superior de un usuario.
        /// </summary>
        public void CerrarRelacion(int idEmpleado)
        {
            var relacion = _context.RelacionesJerarquicas
                .FirstOrDefault(r =>
                    r.IdEmpleado == idEmpleado &&
                    r.FechaFin == null);

            if (relacion != null)
            {
                relacion.FechaFin = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}
