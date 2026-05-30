using DAL;
using ENTITY;
using static ENTITY.ENUMS;

namespace BLL
{
    /// <summary>
    /// Service que centraliza la lógica de permisos sobre equipos y proyectos.
    /// </summary>
    public class PermisosService
    {
        private readonly ProyectoRepository _proyectoRepo;

        public PermisosService()
        {
            _proyectoRepo = new ProyectoRepository();
        }

        /// <summary>
        /// Determina si el usuario en sesión puede crear, editar o eliminar
        /// equipos dentro de un proyecto dado.
        /// - Admin: siempre puede.
        /// - Jefe: solo si es supervisor del proyecto.
        /// - Empleado: nunca puede.
        /// </summary>
        public bool PuedeGestionarEquipos(int idProyecto)
        {
            if (SesionActual.Rol == RolUsuario.Admin)
                return true;

            if (SesionActual.Rol == RolUsuario.Jefe)
                return _proyectoRepo.EsSupervisor(idProyecto, SesionActual.IdUsuario);

            return false;
        }
    }
}
