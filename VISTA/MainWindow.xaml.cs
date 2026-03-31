using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ENTITY;
using System.Windows.Media.Animation;

namespace VISTA
{
    public partial class MainWindow : Window
    {
        private bool _sidebarVisible = true;
        private readonly DB_Context _context = new DB_Context();
        private List<ProyectoVista> _proyectos = new();

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
                        Nombre = e.Nombre,
                        Descripcion = e.Descripcion,
                        Nit = e.Nit,
                        Correo = e.Correo,
                        Telefono = e.Telefono
                    })
                    .FirstOrDefault();

                if (empresa == null)
                {
                    MessageBox.Show("No hay empresa registrada.");
                    return;
                }

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
                        NombreCompleto = (u.Nombre ?? "") + " " + (u.Apellido ?? "")
                    })
                    .ToDictionary(
                        u => u.IdUsuario,
                        u => string.IsNullOrWhiteSpace(u.NombreCompleto) ? "Sin nombre" : u.NombreCompleto.Trim()
                    );

                _proyectos = proyectosDb.Select(p => new ProyectoVista
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = string.IsNullOrWhiteSpace(p.Nombre) ? "Sin nombre" : p.Nombre,
                    Descripcion = string.IsNullOrWhiteSpace(p.Descripcion) ? "-" : p.Descripcion,
                    Supervisor = p.IdSupervisor.HasValue && supervisores.ContainsKey(p.IdSupervisor.Value)
                        ? supervisores[p.IdSupervisor.Value]
                        : "Sin asignar",
                    Estado = string.IsNullOrWhiteSpace(p.Estado) ? "Sin estado" : p.Estado,
                    FechaInicio = p.FechaInicio.HasValue ? p.FechaInicio.Value.ToString("dd/MM/yyyy") : "-",
                    FechaFin = p.FechaFin.HasValue ? p.FechaFin.Value.ToString("dd/MM/yyyy") : "-",
                    Progreso = $"{(p.Progreso ?? 0):0.##}%"
                }).ToList();

                dgProyectos.ItemsSource = _proyectos;

                txtProyectosActivos.Text = _proyectos.Count(x => x.Estado == "Activo").ToString();
                txtProyectosFinalizados.Text = _proyectos.Count(x => x.Estado == "Finalizado").ToString();
                txtProyectosPausados.Text = _proyectos.Count(x => x.Estado == "Pausado").ToString();
                txtResponsablesAsignados.Text = _proyectos.Count(x => x.Supervisor != "Sin asignar").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando empresa:\n{ex}");
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
            try
            {
                var empresa = _context.Empresas
                    .AsNoTracking()
                    .FirstOrDefault();

                if (empresa == null)
                {
                    MessageBox.Show("No hay empresa registrada.");
                    return;
                }

                var ventana = new ProyectoFormWindow(empresa.IdEmpresa)
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

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
        //principal
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