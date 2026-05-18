using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VISTA.UserControls
{
    public partial class DashboardProyectoControl : UserControl
    {
        private readonly EquipoService _equipoService = new();
        private readonly ProyectoService _proyectoService = new();
        private readonly PermisosService _permisosService = new();
        
        public int IdProyecto { get; private set; }
        private int _idEmpresaActual;
        private List<EquipoDto> _equipos = new();

        public event EventHandler VolverEmpresaRequested;
        public event EventHandler<EquipoDto> GestionarEquipoRequested;

        public DashboardProyectoControl(int idProyecto, int idEmpresaActual)
        {
            InitializeComponent();
            IdProyecto = idProyecto;
            _idEmpresaActual = idEmpresaActual;
            Loaded += DashboardProyectoControl_Loaded;
        }

        private void DashboardProyectoControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarEquipos();
        }

        private void CargarEquipos()
        {
            try
            {
                var proyecto = _proyectoService.ObtenerPorId(IdProyecto, _idEmpresaActual);
                if (proyecto == null)
                {
                    MessageBox.Show("No se encontró el proyecto.");
                    return;
                }

                txtTituloEquipos.Text = $"Equipos de {proyecto.Nombre}";
                _equipos = _equipoService.ObtenerEquiposPorProyecto(IdProyecto);
                dgEquipos.ItemsSource = _equipos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando equipos:\n" + ex.Message);
            }
        }

        private void BtnVolverEmpresa_Click(object sender, RoutedEventArgs e)
        {
            VolverEmpresaRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnNuevoEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para crear equipos en este proyecto.");
                return;
            }

            var ventana = new EquipoFormWindow(IdProyecto) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarEquipos();
        }

        private void BtnEditarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipos.SelectedItem is not EquipoDto equipo)
            {
                MessageBox.Show("Selecciona un equipo.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para editar este equipo.");
                return;
            }

            var ventana = new EquipoFormWindow(IdProyecto, equipo.IdEquipo) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarEquipos();
        }

        private void BtnEliminarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipos.SelectedItem is not EquipoDto equipoDto)
            {
                MessageBox.Show("Selecciona un equipo.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para eliminar este equipo.");
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Deseas eliminar el equipo '{equipoDto.Nombre}'?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            try
            {
                _equipoService.EliminarEquipo(equipoDto.IdEquipo);
                CargarEquipos();
                MessageBox.Show("Equipo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando equipo:\n" + ex.Message);
            }
        }

        private void BtnGestionarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is EquipoDto equipo)
            {
                GestionarEquipoRequested?.Invoke(this, equipo);
            }
        }
    }
}
