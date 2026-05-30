using BLL;

namespace VISTA
{
    /// <summary>
    /// Helper de compatibilidad en VISTA que delega al PermisosService en BLL.
    /// Mantenido para no romper llamadas existentes mientras se migra el resto del código.
    /// </summary>
    public static class PermisosEquipoHelper
    {
        private static readonly PermisosService _service = new();

        public static bool PuedeGestionarEquipos(int idProyecto)
        {
            return _service.PuedeGestionarEquipos(idProyecto);
        }
    }
}
