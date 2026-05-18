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
        private List<UsuarioService.UsuarioVistaDto> _todos = new();

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
                _todos = _service.ObtenerUsuariosVista();
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
            if (sender is Button btn && btn.DataContext is UsuarioService.UsuarioVistaDto u)
            {
                var form = new UsuarioFormWindow(u.IdUsuario) { Owner = this };
                if (form.ShowDialog() == true)
                    CargarUsuarios();
            }
        }

        private void BtnDesactivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioService.UsuarioVistaDto u)
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
            if (sender is Button btn && btn.DataContext is UsuarioService.UsuarioVistaDto u)
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
