using static ENTITY.ENUMS;

namespace ENTITY
{
    /// <summary>
    /// Almacena los datos del usuario autenticado durante la sesión activa.
    /// Definido en ENTITY para que DAL, BLL y VISTA puedan acceder sin dependencias circulares.
    /// </summary>
    public static class SesionActual
    {
        public static int IdUsuario { get; set; } = 1;              // temporal
        public static string NombreCompleto { get; set; } = "Luis Paredes";
        public static RolUsuario Rol { get; set; } = RolUsuario.Admin; // temporal
    }
}
