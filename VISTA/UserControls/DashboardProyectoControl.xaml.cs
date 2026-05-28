using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VISTA.UserControls
{
    public partial class DashboardProyectoControl : UserControl
    {
        private readonly EquipoService _equipoService = new();
        private readonly ProyectoService _proyectoService = new();
        private readonly PermisosService _permisosService = new();
        private readonly ArchivoService _archivoService = new();
        
        public int IdProyecto { get; private set; }
        private int _idEmpresaActual;
        private List<EquipoDto> _equipos = new();

        public event EventHandler VolverEmpresaRequested;
        public event EventHandler<EquipoDto> GestionarEquipoRequested;

        public DashboardProyectoControl(int idProyecto, int idEmpresaActual)
        {
            InitializeComponent();
            IdProyecto = idProyecto;
            _idEmpresaActual = idEmpresaActual;
            Loaded += DashboardProyectoControl_Loaded;
        }

        private void DashboardProyectoControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarEquipos();
        }

        private void CargarEquipos()
        {
            try
            {
                var proyecto = _proyectoService.ObtenerPorId(IdProyecto, _idEmpresaActual);
                if (proyecto == null)
                {
                    MessageBox.Show("No se encontró el proyecto.");
                    return;
                }

                txtTituloEquipos.Text = $"Equipos de {proyecto.Nombre}";
                _equipos = _equipoService.ObtenerEquiposPorProyecto(IdProyecto);
                dgEquipos.ItemsSource = _equipos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando equipos:\n" + ex.Message);
            }
        }

        private void BtnVolverEmpresa_Click(object sender, RoutedEventArgs e)
        {
            VolverEmpresaRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnNuevoEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para crear equipos en este proyecto.");
                return;
            }

            var ventana = new EquipoFormWindow(IdProyecto) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarEquipos();
        }

        private void BtnEditarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipos.SelectedItem is not EquipoDto equipo)
            {
                MessageBox.Show("Selecciona un equipo.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para editar este equipo.");
                return;
            }

            var ventana = new EquipoFormWindow(IdProyecto, equipo.IdEquipo) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarEquipos();
        }

        private void BtnEliminarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipos.SelectedItem is not EquipoDto equipoDto)
            {
                MessageBox.Show("Selecciona un equipo.");
                return;
            }

            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para eliminar este equipo.");
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Deseas eliminar el equipo '{equipoDto.Nombre}'?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            try
            {
                _equipoService.EliminarEquipo(equipoDto.IdEquipo);
                CargarEquipos();
                MessageBox.Show("Equipo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando equipo:\n" + ex.Message);
            }
        }

        private void BtnGestionarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is EquipoDto equipo)
            {
                GestionarEquipoRequested?.Invoke(this, equipo);
            }
        }

        // ── Gestión de Archivos del Proyecto ─────────────────────────────────

        private void TabControlProyecto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Evitar bubbling de eventos provenientes de DataGrids u otros controles hijos
            if (e.AddedItems.Count == 0 || e.AddedItems[0] is not TabItem)
            {
                e.Handled = true;
                return;
            }

            e.Handled = true;

            if (tabItemArchivosGenerales != null && tabItemArchivosGenerales.IsSelected)
            {
                CargarArchivosProyecto();
            }
        }

        private void CargarArchivosProyecto()
        {
            try
            {
                var archivos = _archivoService.ObtenerArchivosPorProyectoYEquipos(IdProyecto);
                dgArchivosProyecto.ItemsSource = archivos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los archivos del proyecto:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSubirArchivoProyecto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Seleccionar archivo para subir al proyecto",
                    Filter = "Todos los archivos (*.*)|*.*|Documentos PDF (*.pdf)|*.pdf|Documentos Word (*.docx;*.doc)|*.docx;*.doc|Hojas de cálculo Excel (*.xlsx;*.xls)|*.xlsx;*.xls"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    int idUsuario = SesionActual.IdUsuario;
                    _archivoService.SubirArchivoProyecto(openFileDialog.FileName, IdProyecto, idUsuario);
                    CargarArchivosProyecto();
                    MessageBox.Show("Archivo cargado al proyecto con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al subir el archivo:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDescargarArchivoProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivosProyecto.SelectedItem is not ArchivoDto seleccionada)
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

        private void BtnEliminarArchivoProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivosProyecto.SelectedItem is not ArchivoDto seleccionada)
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
                    CargarArchivosProyecto();
                    MessageBox.Show("Archivo eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo eliminar el archivo:\n" + ex.Message, "Permiso Denegado", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void DgArchivosProyecto_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgArchivosProyecto.SelectedItem is not ArchivoDto seleccionada)
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
                MessageBox.Show("No se pudo abrir el archivo:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
