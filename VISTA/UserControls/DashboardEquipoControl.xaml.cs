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
        private readonly ArchivoService _archivoService = new();

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

        private void TabControlPrincipal_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Solo reaccionar cuando el cambio de selección fue un TabItem (no una fila de DataGrid)
            if (e.AddedItems.Count == 0 || e.AddedItems[0] is not System.Windows.Controls.TabItem)
            {
                e.Handled = true;
                return;
            }

            e.Handled = true;

            if (tabItemArchivos != null && tabItemArchivos.IsSelected)
            {
                CargarArchivos();
            }
        }

        private void CargarArchivos()
        {
            try
            {
                var archivos = _archivoService.ObtenerArchivosPorEquipo(_idEquipo);
                dgArchivos.ItemsSource = archivos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar archivos:\n" + ex.Message);
            }
        }

        private void BtnSubirArchivo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Seleccionar archivo para subir al equipo",
                    Filter = "Todos los archivos (*.*)|*.*|Documentos PDF (*.pdf)|*.pdf|Documentos Word (*.docx;*.doc)|*.docx;*.doc|Hojas de cálculo Excel (*.xlsx;*.xls)|*.xlsx;*.xls"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    int idUsuario = SesionActual.IdUsuario;
                    _archivoService.SubirArchivo(openFileDialog.FileName, _idEquipo, idUsuario);
                    CargarArchivos();
                    MessageBox.Show("Archivo subido con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al subir el archivo:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDescargarArchivo_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivos.SelectedItem is not ArchivoDto seleccionada)
            {
                MessageBox.Show("Selecciona un archivo de la lista para descargar o abrir.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Guardar archivo como...",
                    FileName = seleccionada.NombreOriginal,
                    Filter = $"Archivo original (*{seleccionada.Extension})|*{seleccionada.Extension}|Todos los archivos (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _archivoService.DescargarArchivo(seleccionada.IdArchivo, saveFileDialog.FileName);
                    MessageBox.Show("Archivo descargado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar el archivo:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminarArchivo_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivos.SelectedItem is not ArchivoDto seleccionada)
            {
                MessageBox.Show("Selecciona un archivo de la lista para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = MessageBox.Show($"¿Estás seguro de que deseas eliminar el archivo '{seleccionada.NombreOriginal}'?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    int idUsuario = SesionActual.IdUsuario;
                    string rol = SesionActual.Rol.ToString();

                    _archivoService.EliminarArchivo(seleccionada.IdArchivo, idUsuario, rol);
                    CargarArchivos();
                    MessageBox.Show("Archivo eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo eliminar el archivo:\n" + ex.Message, "Permiso Denegado", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void DgArchivos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgArchivos.SelectedItem is not ArchivoDto seleccionada)
                return;

            try
            {
                string tempPath = _archivoService.ObtenerRutaTemporalParaAbrir(seleccionada.IdArchivo);
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar abrir el archivo de forma automática:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
