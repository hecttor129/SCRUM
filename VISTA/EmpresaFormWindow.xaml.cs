using BLL;
using ENTITY;
using System;
using System.Windows;
using System.Windows.Input;

namespace VISTA
{
    public partial class EmpresaFormWindow : Window
    {
        private readonly EmpresaService _empresaService;
        private Empresa _empresaActual;

        public EmpresaFormWindow()
        {
            InitializeComponent();
            _empresaService = new EmpresaService();
            Loaded += EmpresaFormWindow_Loaded;
        }

        private void EmpresaFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                _empresaActual = _empresaService.ObtenerEmpresa();

                if (_empresaActual != null)
                {
                    txtTituloVentana.Text = "Editar empresa";
                    txtNombre.Text = _empresaActual.Nombre;
                    txtDescripcion.Text = _empresaActual.Descripcion;
                    txtNit.Text = _empresaActual.Nit;
                    txtTelefono.Text = _empresaActual.Telefono;
                    txtCorreo.Text = _empresaActual.Correo;
                }
                else
                {
                    txtTituloVentana.Text = "Nueva empresa";
                    _empresaActual = new Empresa(); // IdEmpresa será 0 por defecto
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la configuración de la empresa:\n" + ex.Message);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = _empresaActual?.IdEmpresa ?? 0;
                
                _empresaService.GuardarEmpresa(
                    id,
                    txtNombre.Text.Trim(),
                    txtDescripcion.Text.Trim(),
                    txtNit.Text.Trim(),
                    txtTelefono.Text.Trim(),
                    txtCorreo.Text.Trim()
                );

                MessageBox.Show("Configuración guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnCerrarVentana_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
