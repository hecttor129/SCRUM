using BLL;
using System;
using System.Windows;

namespace VISTA
{
    public partial class LoginWindow : Window
    {
        private readonly UsuarioService _service;

        public LoginWindow()
        {
            InitializeComponent();
            _service = new UsuarioService();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;

            try
            {
                bool ok = _service.Login(txtEmail.Text, pwdPassword.Password);
                if (ok)
                {
                    new MainWindow().Show();
                    this.Close();
                }
                else
                {
                    MostrarError("Credenciales incorrectas o usuario inactivo.");
                }
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
