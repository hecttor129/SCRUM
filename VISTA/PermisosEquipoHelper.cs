using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ENTITY.ENUMS;

namespace VISTA
{
    public static class PermisosEquipoHelper
    {
        public static bool PuedeGestionarEquipos(DB_Context context, int idProyecto)
        {
            if (SesionActual.Rol == RolUsuario.Admin)
                return true;

            if (SesionActual.Rol == RolUsuario.Jefe)
            {
                return context.Proyectos.Any(p =>
                    p.IdProyecto == idProyecto &&
                    p.IdSupervisor == SesionActual.IdUsuario &&
                    p.Activo == 1);
            }

            return false;
        }
    }
}
