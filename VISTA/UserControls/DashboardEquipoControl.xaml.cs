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
        private TareaService _tareaService = new();

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
                _tareaService = new TareaService();
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

        private void BtnNuevaTarea_Click(object sender, RoutedEventArgs e)
        {
            var win = new TareaFormWindow(null, null, _idEquipo);
            win.Owner = Window.GetWindow(this);
            if (win.ShowDialog() == true)
            {
                CargarTareas();
            }
        }

        private void BtnEditarTarea_Click(object sender, RoutedEventArgs e)
        {
            if (dgTareas.SelectedItem is not TareaDto seleccionada)
            {
                MessageBox.Show("Selecciona una tarea de la lista para editar.");
                return;
            }

            var win = new TareaFormWindow(null, null, _idEquipo, seleccionada.IdTarea);
            win.Owner = Window.GetWindow(this);
            if (win.ShowDialog() == true)
            {
                CargarTareas();
            }
        }

        private void BtnEliminarTarea_Click(object sender, RoutedEventArgs e)
        {
            if (dgTareas.SelectedItem is not TareaDto seleccionada)
            {
                MessageBox.Show("Selecciona una tarea de la lista para eliminar.");
                return;
            }

            var res = MessageBox.Show($"¿Estás seguro de que deseas eliminar la tarea '{seleccionada.Titulo}'?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    _tareaService.EliminarTarea(seleccionada.IdTarea);
                    // Reevaluar las dependencias
                    _tareaService.ReevaluarDisponibilidadTareas(null, null, _idEquipo);
                    CargarTareas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar la tarea:\n" + ex.Message);
                }
            }
        }
    }
}
