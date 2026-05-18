using BLL;
using ENTITY;
using System;
using System.Windows;
using System.Windows.Controls;

namespace VISTA.UserControls
{
    public partial class DashboardEquipoControl : UserControl
    {
        private readonly int _idEquipo;
        private readonly TareaService _tareaService = new();

        public event EventHandler VolverAlProyectoRequested;

        public DashboardEquipoControl(EquipoDto equipo)
        {
            InitializeComponent();
            _idEquipo = equipo.IdEquipo;
            txtTituloEquipoSeleccionado.Text = $"Entorno: {equipo.Nombre}";
            Loaded += DashboardEquipoControl_Loaded;
        }

        private void DashboardEquipoControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarTareas();
        }

        private void CargarTareas()
        {
            try
            {
                var tareas = _tareaService.ObtenerTareasPorEquipo(_idEquipo);
                dgTareas.ItemsSource = tareas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando tareas:\n" + ex.Message);
            }
        }

        private void BtnVolverProyecto_Click(object sender, RoutedEventArgs e)
        {
            VolverAlProyectoRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
