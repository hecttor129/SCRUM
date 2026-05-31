using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VISTA.UserControls
{
    public partial class UsuariosControl : UserControl
    {
        private readonly UsuarioService _service = new();
        private List<UsuarioService.UsuarioVistaDto> _todos = new();

        public UsuariosControl()
        {
            InitializeComponent();
            Loaded += UsuariosControl_Loaded;
        }

        private void UsuariosControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarUsuarios();
        }

        public void CargarUsuarios()
        {
            try
            {
                // Guardar la selección actual
                var selected = dgUsuarios.SelectedItem as UsuarioService.UsuarioVistaDto;
                int? selectedId = selected?.IdUsuario;

                _todos = _service.ObtenerUsuariosVista();
                AplicarFiltro();

                // Restaurar la selección
                if (selectedId.HasValue)
                {
                    var newSelected = _todos.FirstOrDefault(u => u.IdUsuario == selectedId.Value);
                    if (newSelected != null)
                    {
                        dgUsuarios.SelectedItem = newSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltro()
        {
            if (dgUsuarios == null || txtBuscar == null) return;

            var b = txtBuscar.Text.Trim().ToLower();
            if (b == "" || b == "buscar por nombre o correo...")
            {
                dgUsuarios.ItemsSource = _todos;
                return;
            }

            dgUsuarios.ItemsSource = _todos
                .Where(u => u.Nombre.ToLower().Contains(b) ||
                            u.Apellido.ToLower().Contains(b) ||
                            u.Email.ToLower().Contains(b))
                .ToList();
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var form = new UsuarioFormWindow { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
                CargarUsuarios();
        }

        private void BtnEditarTop_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem is UsuarioService.UsuarioVistaDto u)
            {
                var form = new UsuarioFormWindow(u.IdUsuario) { Owner = Window.GetWindow(this) };
                if (form.ShowDialog() == true)
                    CargarUsuarios();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un usuario de la tabla para editar.", "Editar Usuario",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDesactivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioService.UsuarioVistaDto u)
            {
                if (MessageBox.Show($"¿Desactivar a {u.Nombre} {u.Apellido}?", "Confirmar",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try { _service.DesactivarUsuario(u.IdUsuario); CargarUsuarios(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
        }

        private void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioService.UsuarioVistaDto u)
            {
                try { _service.ReactivarUsuario(u.IdUsuario); CargarUsuarios(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
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
