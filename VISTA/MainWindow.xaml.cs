using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace VISTA
{
    public partial class MainWindow : Window
    {
        private bool _sidebarVisible = true;

        // ── Servicios BLL ────────────────────────────────────────────────────
        private readonly EmpresaService _empresaService = new();
        private readonly ProyectoService _proyectoService = new();
        private readonly EquipoService _equipoService = new();
        private readonly PermisosService _permisosService = new();

        // ── Estado local ─────────────────────────────────────────────────────
        private List<ProyectoDto> _proyectos = new();
        private int _idEmpresaActual;
        private int? _idProyectoSeleccionado;
        private List<EquipoDto> _equipos = new();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargarPantallaEmpresa();
        }

        // ── Empresa ──────────────────────────────────────────────────────────

        private void CargarPantallaEmpresa()
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

                _idEmpresaActual = empresa.IdEmpresa;

                txtNombreEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Nombre) ? "Sin nombre" : empresa.Nombre;
                txtDescripcionEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Descripcion) ? "Sin descripción." : empresa.Descripcion;
                txtNitEmpresa.Text = $"NIT: {(string.IsNullOrWhiteSpace(empresa.Nit) ? "-" : empresa.Nit)}";
                txtCorreoEmpresa.Text = $"Correo: {(string.IsNullOrWhiteSpace(empresa.Correo) ? "-" : empresa.Correo)}";
                txtTelefonoEmpresa.Text = $"Teléfono: {(string.IsNullOrWhiteSpace(empresa.Telefono) ? "-" : empresa.Telefono)}";

                _proyectos = _proyectoService.ObtenerProyectosPorEmpresa(_idEmpresaActual);
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

        private void BtnEditarEmpresa_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí luego abrimos la ventana para editar la empresa.");
        }

        // ── Proyectos ────────────────────────────────────────────────────────

        private void txtBuscarProyecto_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscarProyecto.Text.Trim().ToLower();

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

        private void BtnNuevoProyecto_Click(object sender, RoutedEventArgs e)
        {
            AbrirFormularioProyecto(null);
        }

        private void BtnEditarProyecto_Click(object sender, RoutedEventArgs e)
        {
            var proyecto = ObtenerProyectoSeleccionado();
            if (proyecto == null)
            {
                MessageBox.Show("Selecciona un proyecto para editar.");
                return;
            }
            AbrirFormularioProyecto(proyecto.IdProyecto);
        }

        private void BtnEliminarProyecto_Click(object sender, RoutedEventArgs e)
        {
            var proyecto = ObtenerProyectoSeleccionado();
            if (proyecto == null)
            {
                MessageBox.Show("Selecciona un proyecto para eliminar.");
                return;
            }
            EliminarProyecto(proyecto.IdProyecto, proyecto.Nombre);
        }

        private void BtnEditarFilaProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProyectoDto proyecto)
                AbrirFormularioProyecto(proyecto.IdProyecto);
        }

        private void BtnEliminarFilaProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProyectoDto proyecto)
                EliminarProyecto(proyecto.IdProyecto, proyecto.Nombre);
        }

        private void dgProyectos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var proyecto = ObtenerProyectoSeleccionado();
            if (proyecto != null)
                AbrirFormularioProyecto(proyecto.IdProyecto);
        }

        private void dgProyectos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProyectos.SelectedItem is ProyectoDto proyecto)
                _idProyectoSeleccionado = proyecto.IdProyecto;
        }

        private void AbrirFormularioProyecto(int? idProyecto)
        {
            try
            {
                if (_idEmpresaActual <= 0)
                {
                    MessageBox.Show("No se encontró una empresa válida.");
                    return;
                }

                var ventana = new ProyectoFormWindow(_idEmpresaActual, idProyecto) { Owner = this };

                if (ventana.ShowDialog() == true)
                    CargarPantallaEmpresa();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error abriendo la ventana:\n" + ex.Message);
            }
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

        private ProyectoDto? ObtenerProyectoSeleccionado()
        {
            return dgProyectos.SelectedItem as ProyectoDto;
        }

        // ── Equipos ──────────────────────────────────────────────────────────

        private void BtnEquipoSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (!_idProyectoSeleccionado.HasValue)
            {
                MessageBox.Show("Primero selecciona un proyecto.");
                return;
            }

            CargarEquiposProyecto(_idProyectoSeleccionado.Value);
            panelEmpresa.Visibility = Visibility.Collapsed;
            panelEquipos.Visibility = Visibility.Visible;
        }

        private void BtnVolverEmpresa_Click(object sender, RoutedEventArgs e)
        {
            panelEquipos.Visibility = Visibility.Collapsed;
            panelEmpresa.Visibility = Visibility.Visible;
        }

        private void CargarEquiposProyecto(int idProyecto)
        {
            try
            {
                var proyecto = _proyectoService.ObtenerPorId(idProyecto, _idEmpresaActual);

                if (proyecto == null)
                {
                    MessageBox.Show("No se encontró el proyecto.");
                    return;
                }

                txtTituloEquipos.Text = $"Equipos de {proyecto.Nombre}";
                _equipos = _equipoService.ObtenerEquiposPorProyecto(idProyecto);
                dgEquipos.ItemsSource = _equipos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando equipos:\n" + ex.Message);
            }
        }

        private void BtnNuevoEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (!_idProyectoSeleccionado.HasValue)
            {
                MessageBox.Show("Selecciona un proyecto.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(_idProyectoSeleccionado.Value))
            {
                MessageBox.Show("No tienes permisos para crear equipos en este proyecto.");
                return;
            }

            var ventana = new EquipoFormWindow(_idProyectoSeleccionado.Value) { Owner = this };
            if (ventana.ShowDialog() == true)
                CargarEquiposProyecto(_idProyectoSeleccionado.Value);
        }

        private void BtnEditarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (!_idProyectoSeleccionado.HasValue)
            {
                MessageBox.Show("Selecciona un proyecto.");
                return;
            }

            if (dgEquipos.SelectedItem is not EquipoDto equipo)
            {
                MessageBox.Show("Selecciona un equipo.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(_idProyectoSeleccionado.Value))
            {
                MessageBox.Show("No tienes permisos para editar este equipo.");
                return;
            }

            var ventana = new EquipoFormWindow(_idProyectoSeleccionado.Value, equipo.IdEquipo) { Owner = this };
            if (ventana.ShowDialog() == true)
                CargarEquiposProyecto(_idProyectoSeleccionado.Value);
        }

        private void BtnEliminarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (!_idProyectoSeleccionado.HasValue)
            {
                MessageBox.Show("Selecciona un proyecto.");
                return;
            }

            if (dgEquipos.SelectedItem is not EquipoDto equipoDto)
            {
                MessageBox.Show("Selecciona un equipo.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(_idProyectoSeleccionado.Value))
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
                CargarEquiposProyecto(_idProyectoSeleccionado.Value);
                MessageBox.Show("Equipo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando equipo:\n" + ex.Message);
            }
        }

        private void dgEquipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí después puedes cargar tareas, archivos, estadísticas del equipo seleccionado
        }

        // ── Sidebar ──────────────────────────────────────────────────────────

        private void BtnHamburguesa_Click(object sender, RoutedEventArgs e)
        {
            const double abierto = 230;
            const double cerrado = 0;

            double destinoAncho = _sidebarVisible ? cerrado : abierto;
            double destinoOpacidad = _sidebarVisible ? 0 : 1;

            var animWidth = new DoubleAnimation
            {
                To = destinoAncho,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            var animOpacity = new DoubleAnimation
            {
                To = destinoOpacidad,
                Duration = TimeSpan.FromMilliseconds(180)
            };

            SidebarContainer.BeginAnimation(WidthProperty, animWidth);
            SidebarContainer.BeginAnimation(OpacityProperty, animOpacity);

            _sidebarVisible = !_sidebarVisible;
        }
    }
}
