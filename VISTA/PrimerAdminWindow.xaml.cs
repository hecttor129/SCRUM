using BLL;
using System;
using System.Windows;

namespace VISTA
{
    public partial class PrimerAdminWindow : Window
    {
        private readonly UsuarioService _service;

        public PrimerAdminWindow()
        {
            InitializeComponent();
            _service = new UsuarioService();
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;

            if (pwdPassword.Password != pwdConfirmar.Password)
            {
                MostrarError("Las contraseñas no coinciden.");
                return;
            }

            try
            {
                _service.CrearPrimerAdmin(
                    txtNombre.Text, 
                    txtApellido.Text, 
                    txtEmail.Text, 
                    pwdPassword.Password);

                MessageBox.Show("Administrador creado exitosamente. Ahora puedes iniciar sesión.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                
                new LoginWindow().Show();
                this.Close();
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
    }
}
