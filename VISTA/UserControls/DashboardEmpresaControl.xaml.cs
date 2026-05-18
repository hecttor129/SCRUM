using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VISTA.UserControls
{
    public partial class DashboardEmpresaControl : UserControl
    {
        private readonly EmpresaService _empresaService = new();
        private readonly ProyectoService _proyectoService = new();
        
        public int IdEmpresaActual { get; private set; }
        private List<ProyectoDto> _proyectos = new();

        public event EventHandler<ProyectoDto> GestionarProyectoRequested;
        public event EventHandler ProyectoSeleccionadoModificado;

        public DashboardEmpresaControl()
        {
            InitializeComponent();
            Loaded += DashboardEmpresaControl_Loaded;
        }

        private void DashboardEmpresaControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarPantallaEmpresa();
        }

        public void CargarPantallaEmpresa()
        {
            try
            {
                var empresa = _empresaService.ObtenerEmpresa();

                if (empresa == null)
                {
                    MessageBox.Show("No hay empresa registrada.");
                    dgProyectos.ItemsSource = null;
                    _proyectos = new List<ProyectoDto>();
                    return;
                }

                IdEmpresaActual = empresa.IdEmpresa;

                txtNombreEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Nombre) ? "Sin nombre" : empresa.Nombre;
                txtDescripcionEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Descripcion) ? "Sin descripción." : empresa.Descripcion;
                txtNitEmpresa.Text = $"NIT: {(string.IsNullOrWhiteSpace(empresa.Nit) ? "-" : empresa.Nit)}";
                txtCorreoEmpresa.Text = $"Correo: {(string.IsNullOrWhiteSpace(empresa.Correo) ? "-" : empresa.Correo)}";
                txtTelefonoEmpresa.Text = $"Teléfono: {(string.IsNullOrWhiteSpace(empresa.Telefono) ? "-" : empresa.Telefono)}";

                _proyectos = _proyectoService.ObtenerProyectosPorEmpresa(IdEmpresaActual);
                dgProyectos.ItemsSource = _proyectos;

                txtProyectosActivos.Text = _proyectos.Count(x => x.Estado.Equals("Activo", StringComparison.OrdinalIgnoreCase)).ToString();
                txtProyectosFinalizados.Text = _proyectos.Count(x => x.Estado.Equals("Finalizado", StringComparison.OrdinalIgnoreCase)).ToString();
                txtProyectosPausados.Text = _proyectos.Count(x => x.Estado.Equals("Pausado", StringComparison.OrdinalIgnoreCase)).ToString();
                txtResponsablesAsignados.Text = _proyectos.Count(x => x.Supervisor != "Sin asignar").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando empresa:\n{ex.Message}");
            }
        }

        public void FiltrarProyectos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                dgProyectos.ItemsSource = _proyectos;
                return;
            }

            var filtrados = _proyectos
                .Where(p =>
                    p.Nombre.ToLower().Contains(texto) ||
                    p.Descripcion.ToLower().Contains(texto) ||
                    p.Supervisor.ToLower().Contains(texto) ||
                    p.Estado.ToLower().Contains(texto))
                .ToList();

            dgProyectos.ItemsSource = filtrados;
        }

        private void BtnGestionarProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ProyectoDto proyecto)
            {
                GestionarProyectoRequested?.Invoke(this, proyecto);
            }
        }

        private void BtnEditarFilaProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProyectoDto proyecto)
            {
                AbrirFormularioProyecto(proyecto.IdProyecto);
            }
        }

        private void BtnEliminarFilaProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProyectoDto proyecto)
            {
                EliminarProyecto(proyecto.IdProyecto, proyecto.Nombre);
            }
        }

        public void AbrirFormularioProyecto(int? idProyecto)
        {
            try
            {
                if (IdEmpresaActual <= 0)
                {
                    MessageBox.Show("No se encontró una empresa válida.");
                    return;
                }

                var ventana = new ProyectoFormWindow(IdEmpresaActual, idProyecto) { Owner = Window.GetWindow(this) };

                if (ventana.ShowDialog() == true)
                {
                    CargarPantallaEmpresa();
                    ProyectoSeleccionadoModificado?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error abriendo la ventana:\n" + ex.Message);
            }
        }

        public void EliminarProyectoConfirmado()
        {
            var proyecto = dgProyectos.SelectedItem as ProyectoDto;
            if (proyecto == null)
            {
                MessageBox.Show("Selecciona un proyecto para eliminar.");
                return;
            }
            EliminarProyecto(proyecto.IdProyecto, proyecto.Nombre);
        }

        private void EliminarProyecto(int idProyecto, string nombreProyecto)
        {
            try
            {
                var confirmacion = MessageBox.Show(
                    $"¿Deseas eliminar el proyecto \"{nombreProyecto}\"?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmacion != MessageBoxResult.Yes) return;

                _proyectoService.EliminarProyecto(idProyecto);
                MessageBox.Show("Proyecto eliminado correctamente.");
                CargarPantallaEmpresa();
                ProyectoSeleccionadoModificado?.Invoke(this, EventArgs.Empty);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                MessageBox.Show("No se puede eliminar el proyecto porque tiene registros relacionados en la base de datos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando proyecto:\n" + ex.Message);
            }
        }

        public ProyectoDto ObtenerProyectoSeleccionado()
        {
            return dgProyectos.SelectedItem as ProyectoDto;
        }
    }
}
