using System.Windows;
using BLL;

namespace VISTA
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var service = new UsuarioService();

            if (!service.ExisteAdmin())
            {
                new PrimerAdminWindow().Show();
            }
            else
            {
                new LoginWindow().Show();
            }
        }
    }
}
