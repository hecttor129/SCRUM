using BLL;
using ENTITY;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VISTA.UserControls;

namespace VISTA
{
    public partial class MainWindow : Window
    {
        private bool _sidebarVisible = true;
        private bool _inicializado    = false;  // evita que SelectionChanged actúe antes del Loaded

        private DashboardProyectoControl _proyectoControl;
        private DashboardEquipoControl   _equipoControl;

        private int? _idProyectoActivo;
        private int  _idEmpresaActiva;

        // ── Visibilidad barra de retroceso ────────────────────────────────
        private void MostrarVolver(bool visible)
            => barraVolver.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtSidebarNombre.Text = SesionActual.NombreCompleto;
            txtSidebarRol.Text    = SesionActual.Rol.ToString();

            if (SesionActual.Rol == ENTITY.ENUMS.RolUsuario.Admin)
            {
                tabUsuarios.Visibility     = Visibility.Visible;
                btnMenuUsuarios.Visibility = Visibility.Visible;
            }

            ctrlProyectos.GestionarProyectoRequested    += OnGestionarProyecto;
            ctrlEquiposGeneral.GestionarEquipoRequested += OnGestionarEquipoDesdeGeneral;

            // Primer arranque: solicitar creación de empresa si no existe
            var empSvc  = new EmpresaService();
            var empresa = empSvc.ObtenerEmpresa();
            if (empresa == null)
            {
                MessageBox.Show(
                    "No se encontró ninguna empresa registrada.\nPor favor configure los datos de la empresa.",
                    "Configuración Inicial", MessageBoxButton.OK, MessageBoxImage.Information);
                var ventana = new EmpresaFormWindow { Owner = this };
                if (ventana.ShowDialog() == true)
                    empresa = empSvc.ObtenerEmpresa();
            }

            if (empresa != null)
            {
                _idEmpresaActiva = empresa.IdEmpresa;
                ctrlEmpresa.CargarPantallaEmpresa();
                ctrlProyectos.CargarProyectos();
            }

            _inicializado = true;
        }

        // ── Selección de tabs ──────────────────────────────────────────────

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_inicializado) return;
            if (e.AddedItems.Count == 0 || e.AddedItems[0] is not TabItem) return;
            e.Handled = true;

            switch (MainTabControl.SelectedIndex)
            {
                case 0: btnMenuEmpresa.IsChecked   = true; break;
                case 1: btnMenuProyectos.IsChecked = true; break;
                case 2: btnMenuEquipo.IsChecked    = true; break;
                case 3: btnMenuUsuarios.IsChecked  = true; break;
                case 4: btnMenuReportes.IsChecked  = true; break;
                case 5: btnMenuPeriodo.IsChecked   = true; break;
            }

            if (MainTabControl.SelectedItem is TabItem tab)
            {
                switch (tab.Header?.ToString())
                {
                    case "Empresa":   ctrlEmpresa.CargarPantallaEmpresa();   break;
                    case "Usuarios":  ctrlUsuarios.CargarUsuarios();          break;
                    case "Equipo":    ctrlEquiposGeneral.CargarEquipos();     break;
                }
            }
        }

        private void NavegarATab(int index)
        {
            if (index == 1)
                RestaurarProyectos();
            else if (index == 2)
                RestaurarEquipos();

            MainTabControl.SelectedIndex = index;
        }

        private void BtnMenuEmpresa_Click(object sender, RoutedEventArgs e)   => NavegarATab(0);
        private void BtnMenuProyectos_Click(object sender, RoutedEventArgs e) => NavegarATab(1);
        private void BtnMenuEquipo_Click(object sender, RoutedEventArgs e)    => NavegarATab(2);
        private void BtnMenuUsuarios_Click(object sender, RoutedEventArgs e)  => NavegarATab(3);
        private void BtnMenuReportes_Click(object sender, RoutedEventArgs e)  => NavegarATab(4);
        private void BtnMenuPeriodo_Click(object sender, RoutedEventArgs e)   => NavegarATab(5);

        // ── Helpers de restauración ────────────────────────────────────────

        private void RestaurarProyectos()
        {
            gridProyectos.Children.Clear();
            gridProyectos.Children.Add(ctrlProyectos);
            ctrlProyectos.CargarProyectos();
            MostrarVolver(false);
            _idProyectoActivo = null;
        }

        private void RestaurarEquipos()
        {
            gridEquipo.Children.Clear();
            gridEquipo.Children.Add(ctrlEquiposGeneral);
            ctrlEquiposGeneral.CargarEquipos();
            MostrarVolver(false);
        }

        // ── Navegación a Proyecto ──────────────────────────────────────────

        private void OnGestionarProyecto(object sender, ProyectoDto proyecto)
        {
            _idProyectoActivo = proyecto.IdProyecto;

            _proyectoControl = new DashboardProyectoControl(proyecto.IdProyecto, _idEmpresaActiva);
            _proyectoControl.GestionarEquipoRequested += OnGestionarEquipo;

            gridProyectos.Children.Clear();
            gridProyectos.Children.Add(_proyectoControl);
            MostrarVolver(true);
        }

        private void OnVolverAProyectos(object sender, EventArgs e) => RestaurarProyectos();

        // ── Navegación a Equipo ────────────────────────────────────────────

        private void OnGestionarEquipo(object sender, EquipoDto equipo)
            => MostrarDashboardEquipo(equipo, desdeProjeto: true);

        private void OnGestionarEquipoDesdeGeneral(object sender, EquipoDto equipo)
        {
            _idProyectoActivo = equipo.IdProyecto;

            if (_proyectoControl == null || _proyectoControl.IdProyecto != equipo.IdProyecto)
            {
                _proyectoControl = new DashboardProyectoControl(equipo.IdProyecto, _idEmpresaActiva);
                _proyectoControl.GestionarEquipoRequested += OnGestionarEquipo;
            }

            MostrarDashboardEquipo(equipo, desdeProjeto: false);
        }

        private void MostrarDashboardEquipo(EquipoDto equipo, bool desdeProjeto)
        {
            _equipoControl = new DashboardEquipoControl(equipo);

            if (desdeProjeto)
            {
                gridProyectos.Children.Clear();
                gridProyectos.Children.Add(_equipoControl);
            }
            else
            {
                gridEquipo.Children.Clear();
                gridEquipo.Children.Add(_equipoControl);
            }

            MostrarVolver(true);
        }

        private void OnVolverAProyecto()
        {
            if (_proyectoControl != null)
            {
                gridProyectos.Children.Clear();
                gridProyectos.Children.Add(_proyectoControl);
            }
            MostrarVolver(true);
        }

        // ── Botón ← Volver ────────────────────────────────────────────────

        private void BtnVolverPrincipal_Click(object sender, RoutedEventArgs e)
        {
            int index = MainTabControl.SelectedIndex;

            if (index == 1) // Tab Proyectos
            {
                if (gridProyectos.Children.Count > 0)
                {
                    var current = gridProyectos.Children[0];
                    if (current is DashboardEquipoControl)
                        OnVolverAProyecto();
                    else if (current is DashboardProyectoControl)
                        RestaurarProyectos();
                }
            }
            else if (index == 2) // Tab Equipo
            {
                if (gridEquipo.Children.Count > 0 && gridEquipo.Children[0] is DashboardEquipoControl)
                    RestaurarEquipos();
            }
        }

        // ── Hamburguesa ───────────────────────────────────────────────────

        private void BtnHamburguesa_Click(object sender, RoutedEventArgs e)
        {
            double destino  = _sidebarVisible ? 0   : 220;
            double opacidad = _sidebarVisible ? 0.0 : 1.0;

            SidebarContainer.BeginAnimation(WidthProperty,
                new DoubleAnimation
                {
                    To             = destino,
                    Duration       = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                });
            SidebarContainer.BeginAnimation(OpacityProperty,
                new DoubleAnimation { To = opacidad, Duration = TimeSpan.FromMilliseconds(160) });

            _sidebarVisible = !_sidebarVisible;
        }
    }
}
