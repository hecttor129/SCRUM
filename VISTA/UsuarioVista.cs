using System;

namespace VISTA
{
    /// <summary>
    /// Modelo para mapear la vista del Grid de UsuariosWindow
    /// </summary>
    public class UsuarioVista
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RolDisplay { get; set; } = string.Empty;
        public int NivelJerarquico { get; set; }
        public string NombreSuperior { get; set; } = string.Empty;
        public string EstadoDisplay { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
        public int Activo { get; set; } 

        public bool EsAdmin => RolDisplay == "Admin";
        public bool PuedeDesactivar => Activo == 1 && !EsAdmin;
        public bool PuedeReactivar => Activo == 0 && !EsAdmin;
    }
}
