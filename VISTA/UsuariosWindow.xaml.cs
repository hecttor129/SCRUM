using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VISTA
{
    public partial class UsuariosWindow : Window
    {
        private readonly UsuarioService _service;
        private List<UsuarioVista> _todos = new();

        public UsuariosWindow()
        {
            InitializeComponent();
            _service = new UsuarioService();
            Loaded += UsuariosWindow_Loaded;
            
            // Add a simple bool-to-visibility converter implicitly
            if (!Resources.Contains("BooleanToVisibilityConverter"))
                Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
        }

        private void UsuariosWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            try
            {
                var usuarios = _service.ObtenerTodos();
                
                _todos = usuarios.Select(u => {
                    string sup = "-";
                    var rel = _service.ObtenerSuperiorActual(u.IdUsuario);
                    if (rel != null)
                    {
                        var j = usuarios.FirstOrDefault(x => x.IdUsuario == rel.IdJefe);
                        if (j != null) sup = $"{j.Nombre} {j.Apellido}".Trim();
                    }

                    return new UsuarioVista
                    {
                        IdUsuario = u.IdUsuario,
                        Nombre = u.Nombre,
                        Apellido = u.Apellido,
                        Email = u.Email,
                        RolDisplay = u.Rol.ToString(),
                        NivelJerarquico = u.NivelJerarquico,
                        EstadoDisplay = u.Activo == 1 ? "Activo" : "Inactivo",
                        NombreSuperior = sup,
                        FechaCreacion = u.FechaCreacion.ToString("dd/MM/yyyy"),
                        Activo = u.Activo
                    };
                }).ToList();

                AplicarFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltro()
        {
            string b = txtBuscar.Text.Trim().ToLower();
            if (b == "" || b == "buscar por nombre o correo...")
            {
                dgUsuarios.ItemsSource = _todos;
                return;
            }

            dgUsuarios.ItemsSource = _todos.Where(u => 
                u.Nombre.ToLower().Contains(b) || 
                u.Apellido.ToLower().Contains(b) || 
                u.Email.ToLower().Contains(b)).ToList();
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var form = new UsuarioFormWindow() { Owner = this };
            if (form.ShowDialog() == true)
                CargarUsuarios();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioVista u)
            {
                var form = new UsuarioFormWindow(u.IdUsuario) { Owner = this };
                if (form.ShowDialog() == true)
                    CargarUsuarios();
            }
        }

        private void BtnDesactivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioVista u)
            {
                if (MessageBox.Show($"¿Seguro que deseas desactivar a {u.Nombre} {u.Apellido}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try { _service.DesactivarUsuario(u.IdUsuario); CargarUsuarios(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioVista u)
            {
                try { _service.ReactivarUsuario(u.IdUsuario); CargarUsuarios(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void TxtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar por nombre o correo...")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TxtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar por nombre o correo...";
                txtBuscar.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e) => AplicarFiltro();
    }
}
