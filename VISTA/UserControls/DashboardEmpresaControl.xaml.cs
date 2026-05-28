using BLL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace VISTA.UserControls
{
    public partial class DashboardEmpresaControl : UserControl
    {
        private readonly EmpresaService _empresaService = new();

        public int IdEmpresaActual { get; private set; }

        // Mantenemos estos eventos por si MainWindow los necesita
        public event EventHandler<ProyectoDto> GestionarProyectoRequested;
        public event EventHandler ProyectoSeleccionadoModificado;

        public DashboardEmpresaControl()
        {
            InitializeComponent();
        }

        public void CargarPantallaEmpresa()
        {
            try
            {
                var empresa = _empresaService.ObtenerEmpresa();
                if (empresa == null)
                {
                    txtNombreEmpresa.Text    = "Sin empresa configurada";
                    txtDescripcionEmpresa.Text = "Configure la empresa desde el botón Editar.";
                    return;
                }

                IdEmpresaActual = empresa.IdEmpresa;

                txtNombreEmpresa.Text     = string.IsNullOrWhiteSpace(empresa.Nombre) ? "Sin nombre" : empresa.Nombre;
                txtDescripcionEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Descripcion) ? "Sin descripción." : empresa.Descripcion;
                txtNitEmpresa.Text        = string.IsNullOrWhiteSpace(empresa.Nit) ? "—" : empresa.Nit;
                txtCorreoEmpresa.Text     = string.IsNullOrWhiteSpace(empresa.Correo) ? "—" : empresa.Correo;
                txtTelefonoEmpresa.Text   = string.IsNullOrWhiteSpace(empresa.Telefono) ? "—" : empresa.Telefono;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando empresa:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditarEmpresa_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new EmpresaFormWindow { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarPantallaEmpresa();
        }
    }
}
