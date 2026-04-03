using BLL;
using ENTITY;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VISTA
{
    public partial class UsuarioFormWindow : Window
    {
        private readonly UsuarioService _service;
        private readonly int? _idUsuario;
        private int _adminId = 0;

        public UsuarioFormWindow(int? idUsuario = null)
        {
            InitializeComponent();
            _service = new UsuarioService();
            _idUsuario = idUsuario;
            Loaded += UsuarioFormWindow_Loaded;
        }

        private void UsuarioFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var supervisores = _service.ObtenerSupervisoresDisponibles();
            cbSuperior.ItemsSource = supervisores;
            
            var admin = supervisores.FirstOrDefault(s => s.NivelJerarquico == 1);
            if (admin != null) _adminId = admin.IdUsuario;

            if (_idUsuario == null)
            {
                txtTitulo.Text = "Nuevo usuario";
                btnGuardar.Content = "Crear usuario";
                txtHintPwd.Visibility = Visibility.Collapsed;
                cbRol.SelectedIndex = 1; // Empleado
                if (cbSuperior.Items.Count > 0) cbSuperior.SelectedIndex = 0;
            }
            else
            {
                txtTitulo.Text = "Editar usuario";
                btnGuardar.Content = "Guardar cambios";
                txtHintPwd.Visibility = Visibility.Visible;
                CargarDatos(_idUsuario.Value);
            }
        }

        private void CargarDatos(int id)
        {
            try
            {
                var usuario = _service.ObtenerTodos().FirstOrDefault(u => u.IdUsuario == id);
                if (usuario == null) { Close(); return; }

                txtNombre.Text = usuario.Nombre;
                txtApellido.Text = usuario.Apellido;
                txtEmail.Text = usuario.Email;
                txtSalario.Text = usuario.Salario?.ToString();

                foreach (ComboBoxItem item in cbRol.Items)
                {
                    if (item.Content.ToString() == usuario.Rol.ToString())
                    {
                        cbRol.SelectedItem = item;
                        break;
                    }
                }

                var rel = _service.ObtenerSuperiorActual(id);
                if (rel != null)
                {
                    var supItem = cbSuperior.Items.Cast<UsuarioSuperiorItem>().FirstOrDefault(s => s.IdUsuario == rel.IdJefe);
                    if (supItem != null) cbSuperior.SelectedItem = supItem;
                }
                else if (cbSuperior.Items.Count > 0)
                {
                    cbSuperior.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private void CbSuperior_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSuperior.SelectedItem is UsuarioSuperiorItem s)
            {
                txtNivelCalculado.Text = $"Nivel jerárquico asignado: {s.NivelJerarquico + 1}";
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;

            try
            {
                string rolStr = ((ComboBoxItem)cbRol.SelectedItem).Content.ToString();
                ENUMS.RolUsuario rol = rolStr == "Jefe" ? ENUMS.RolUsuario.Jefe : ENUMS.RolUsuario.Empleado;
                
                int idSup = _adminId;
                if (cbSuperior.SelectedItem is UsuarioSuperiorItem s) idSup = s.IdUsuario;

                decimal? sal = null;
                if (decimal.TryParse(txtSalario.Text, out decimal parsedSal)) sal = parsedSal;

                if (_idUsuario == null)
                {
                    _service.CrearUsuario(txtNombre.Text, txtApellido.Text, txtEmail.Text, pwdPassword.Password, rol, idSup);
                }
                else
                {
                    _service.EditarUsuario(_idUsuario.Value, txtNombre.Text, txtApellido.Text, txtEmail.Text, rol, sal, idSup);
                    if (!string.IsNullOrWhiteSpace(pwdPassword.Password))
                    {
                        _service.CambiarPassword(_idUsuario.Value, pwdPassword.Password);
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private void MostrarError(string msg)
        {
            txtError.Text = msg;
            txtError.Visibility = Visibility.Visible;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();
    }
}
