// SesionActual fue movida a la capa ENTITY para ser accesible desde BLL y DAL.
// Este archivo es un alias para mantener compatibilidad con código de VISTA que aún use el namespace VISTA.
using static ENTITY.ENUMS;

namespace VISTA
{
    /// <summary>
    /// Alias de ENTITY.SesionActual para compatibilidad en la capa VISTA.
    /// </summary>
    public static class SesionActual
    {
        public static int IdUsuario
        {
            get => ENTITY.SesionActual.IdUsuario;
            set => ENTITY.SesionActual.IdUsuario = value;
        }

        public static string NombreCompleto
        {
            get => ENTITY.SesionActual.NombreCompleto;
            set => ENTITY.SesionActual.NombreCompleto = value;
        }

        public static RolUsuario Rol
        {
            get => ENTITY.SesionActual.Rol;
            set => ENTITY.SesionActual.Rol = value;
        }
    }
}
