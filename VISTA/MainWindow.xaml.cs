using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ENTITY;

namespace VISTA
{
    public partial class MainWindow : Window
    {
        private bool _sidebarVisible = true;
        private readonly DB_Context _context = new DB_Context();
        private List<ProyectoVista> _proyectos = new();
        private int _idEmpresaActual;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargarPantallaEmpresa();
        }

        private void CargarPantallaEmpresa()
        {
            try
            {
                var empresa = _context.Empresas
                    .AsNoTracking()
                    .Select(e => new
                    {
                        e.IdEmpresa,
                        e.Nombre,
                        e.Descripcion,
                        e.Nit,
                        e.Correo,
                        e.Telefono
                    })
                    .FirstOrDefault();

                if (empresa == null)
                {
                    MessageBox.Show("No hay empresa registrada.");
                    dgProyectos.ItemsSource = null;
                    _proyectos = new List<ProyectoVista>();
                    return;
                }

                _idEmpresaActual = empresa.IdEmpresa;

                txtNombreEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Nombre)
                    ? "Sin nombre"
                    : empresa.Nombre;

                txtDescripcionEmpresa.Text = string.IsNullOrWhiteSpace(empresa.Descripcion)
                    ? "Sin descripción."
                    : empresa.Descripcion;

                txtNitEmpresa.Text = $"NIT: {(string.IsNullOrWhiteSpace(empresa.Nit) ? "-" : empresa.Nit)}";
                txtCorreoEmpresa.Text = $"Correo: {(string.IsNullOrWhiteSpace(empresa.Correo) ? "-" : empresa.Correo)}";
                txtTelefonoEmpresa.Text = $"Teléfono: {(string.IsNullOrWhiteSpace(empresa.Telefono) ? "-" : empresa.Telefono)}";

                var proyectosDb = _context.Proyectos
                    .AsNoTracking()
                    .Where(p => p.IdEmpresa == empresa.IdEmpresa)
                    .Select(p => new
                    {
                        p.IdProyecto,
                        p.Nombre,
                        p.Descripcion,
                        p.IdSupervisor,
                        p.Estado,
                        p.FechaInicio,
                        p.FechaFin,
                        p.Progreso
                    })
                    .OrderBy(p => p.Nombre)
                    .ToList();

                var idsSupervisores = proyectosDb
                    .Where(p => p.IdSupervisor.HasValue)
                    .Select(p => p.IdSupervisor.Value)
                    .Distinct()
                    .ToList();

                var supervisores = _context.Usuarios
                    .AsNoTracking()
                    .Where(u => idsSupervisores.Contains(u.IdUsuario))
                    .Select(u => new
                    {
                        u.IdUsuario,
                        NombreCompleto = ((u.Nombre ?? "") + " " + (u.Apellido ?? "")).Trim()
                    })
                    .ToDictionary(
                        u => u.IdUsuario,
                        u => string.IsNullOrWhiteSpace(u.NombreCompleto) ? "Sin nombre" : u.NombreCompleto
                    );

                _proyectos = proyectosDb.Select(p => new ProyectoVista
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = string.IsNullOrWhiteSpace(p.Nombre) ? "Sin nombre" : p.Nombre.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(p.Descripcion) ? "-" : p.Descripcion.Trim(),
                    Supervisor = p.IdSupervisor.HasValue && supervisores.ContainsKey(p.IdSupervisor.Value)
                        ? supervisores[p.IdSupervisor.Value]
                        : "Sin asignar",
                    Estado = string.IsNullOrWhiteSpace(p.Estado) ? "Sin estado" : p.Estado.Trim(),
                    FechaInicio = p.FechaInicio.HasValue ? p.FechaInicio.Value.ToString("dd/MM/yyyy") : "-",
                    FechaFin = p.FechaFin.HasValue ? p.FechaFin.Value.ToString("dd/MM/yyyy") : "-",
                    Progreso = $"{(p.Progreso ?? 0):0.##}%"
                }).ToList();

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

        private void BtnEditarEmpresa_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí luego abrimos la ventana para editar la empresa.");
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
            if (sender is Button button && button.DataContext is ProyectoVista proyecto)
            {
                AbrirFormularioProyecto(proyecto.IdProyecto);
            }
        }

        private void BtnEliminarFilaProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProyectoVista proyecto)
            {
                EliminarProyecto(proyecto.IdProyecto, proyecto.Nombre);
            }
        }

        private void dgProyectos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var proyecto = ObtenerProyectoSeleccionado();
            if (proyecto != null)
            {
                AbrirFormularioProyecto(proyecto.IdProyecto);
            }
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

                var ventana = new ProyectoFormWindow(_idEmpresaActual, idProyecto)
                {
                    Owner = this
                };

                bool? resultado = ventana.ShowDialog();

                if (resultado == true)
                {
                    CargarPantallaEmpresa();
                }
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

                if (confirmacion != MessageBoxResult.Yes)
                    return;

                var proyecto = _context.Proyectos.FirstOrDefault(p => p.IdProyecto == idProyecto);

                if (proyecto == null)
                {
                    MessageBox.Show("El proyecto ya no existe o no se pudo encontrar.");
                    CargarPantallaEmpresa();
                    return;
                }

                _context.Proyectos.Remove(proyecto);
                _context.SaveChanges();

                MessageBox.Show("Proyecto eliminado correctamente.");
                CargarPantallaEmpresa();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("No se puede eliminar el proyecto porque tiene registros relacionados en la base de datos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando proyecto:\n" + ex.Message);
            }
        }

        private ProyectoVista? ObtenerProyectoSeleccionado()
        {
            return dgProyectos.SelectedItem as ProyectoVista;
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private class ProyectoVista
        {
            public int IdProyecto { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public string Supervisor { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public string FechaInicio { get; set; } = string.Empty;
            public string FechaFin { get; set; } = string.Empty;
            public string Progreso { get; set; } = string.Empty;
        }
    }
}
