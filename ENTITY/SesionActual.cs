using static ENTITY.ENUMS;

namespace ENTITY
{
    /// <summary>
    /// Almacena los datos del usuario autenticado durante la sesión activa.
    /// Definido en ENTITY para que DAL, BLL y VISTA puedan acceder sin dependencias circulares.
    /// Los valores los llena UsuarioService.Login() al autenticar.
    /// </summary>
    public static class SesionActual
    {
        public static int    IdUsuario       { get; set; }
        public static string NombreCompleto  { get; set; } = string.Empty;
        public static RolUsuario Rol         { get; set; }
        public static int    NivelJerarquico { get; set; }
    }
}
