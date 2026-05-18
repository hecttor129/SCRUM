using ENTITY;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using VISTA.UserControls;

namespace VISTA
{
    public partial class MainWindow : Window
    {
        private bool _sidebarVisible = true;
        
        private DashboardEmpresaControl _empresaControl;
        private DashboardProyectoControl _proyectoControl;
        private DashboardEquipoControl _equipoControl;
        private ReportesControl _reportesControl;

        private int? _idProyectoSeleccionado;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InicializarSidebar();
            MostrarPantallaEmpresa();
        }

        private void InicializarSidebar()
        {
            txtSidebarNombre.Text = SesionActual.NombreCompleto;
            txtSidebarRol.Text = SesionActual.Rol.ToString();

            if (SesionActual.Rol == ENTITY.ENUMS.RolUsuario.Admin)
            {
                btnUsuarios.Visibility = Visibility.Visible;
            }
        }

        private void MostrarPantallaEmpresa()
        {
            _idProyectoSeleccionado = null;
            if (_empresaControl == null)
            {
                _empresaControl = new DashboardEmpresaControl();
                _empresaControl.GestionarProyectoRequested += (s, proyecto) => MostrarPantallaProyecto(proyecto.IdProyecto, _empresaControl.IdEmpresaActual);
            }
            else 
            {
                _empresaControl.CargarPantallaEmpresa();
            }
            
            MainContent.Content = _empresaControl;
        }

        private void MostrarPantallaProyecto(int idProyecto, int idEmpresa)
        {
            _idProyectoSeleccionado = idProyecto;
            _proyectoControl = new DashboardProyectoControl(idProyecto, idEmpresa);
            _proyectoControl.VolverEmpresaRequested += (s, e) => MostrarPantallaEmpresa();
            _proyectoControl.GestionarEquipoRequested += (s, equipo) => MostrarPantallaEquipo(equipo);
            
            MainContent.Content = _proyectoControl;
        }

        private void MostrarPantallaEquipo(EquipoDto equipo)
        {
            _equipoControl = new DashboardEquipoControl(equipo);
            _equipoControl.VolverAlProyectoRequested += (s, e) => {
                MainContent.Content = _proyectoControl;
            };
            
            MainContent.Content = _equipoControl;
        }

        private void BtnSidebarEmpresa_Click(object sender, RoutedEventArgs e)
        {
            MostrarPantallaEmpresa();
        }

        private void BtnEquipoSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (!_idProyectoSeleccionado.HasValue)
            {
                MessageBox.Show("Primero selecciona o gestiona un proyecto desde la lista de la Empresa.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_proyectoControl != null)
            {
                MainContent.Content = _proyectoControl;
            }
        }

        private void BtnSidebarReportes_Click(object sender, RoutedEventArgs e)
        {
            if (_reportesControl == null)
                _reportesControl = new ReportesControl();
            
            MainContent.Content = _reportesControl;
        }

        // ── Top Bar Actions ───────────────────────────────────────────────────
        
        private void txtBuscarProyecto_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (MainContent.Content == _empresaControl)
            {
                _empresaControl.FiltrarProyectos(txtBuscarProyecto.Text);
            }
        }

        private void BtnNuevoProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content == _empresaControl)
                _empresaControl.AbrirFormularioProyecto(null);
        }

        private void BtnEditarProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content == _empresaControl)
            {
                var p = _empresaControl.ObtenerProyectoSeleccionado();
                if (p != null) _empresaControl.AbrirFormularioProyecto(p.IdProyecto);
                else MessageBox.Show("Selecciona un proyecto.");
            }
        }

        private void BtnEliminarProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content == _empresaControl)
                _empresaControl.EliminarProyectoConfirmado();
        }
        
        private void BtnEditarEmpresa_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new EmpresaFormWindow() { Owner = this };
            if (ventana.ShowDialog() == true)
            {
                _empresaControl?.CargarPantallaEmpresa();
            }
        }

        // ── Animaciones y Navegación ───────────────────────────────────────────

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

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new UsuariosWindow() { Owner = this };
            ventana.ShowDialog();
        }
    }
}
