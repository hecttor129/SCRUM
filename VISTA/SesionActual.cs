using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ENTITY.ENUMS;

namespace VISTA
{
    public static class SesionActual
    {
        public static int IdUsuario { get; set; } = 1; // temporal
        public static string NombreCompleto { get; set; } = "Luis Paredes";
        public static RolUsuario Rol { get; set; } = RolUsuario.Admin; // temporal
    }
}
