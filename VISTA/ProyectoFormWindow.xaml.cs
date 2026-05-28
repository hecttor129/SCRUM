using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VISTA
{
    public partial class ProyectoFormWindow : Window
    {
        // ── Servicios BLL ────────────────────────────────────────────────────
        private readonly ProyectoService _proyectoService = new();

        // ── Estado ───────────────────────────────────────────────────────────
        private readonly int _idEmpresa;
        private readonly int? _idProyecto;
        private Proyecto? _proyectoEditando;
        private string _rutaHistorias = string.Empty;
        private string _rutaRequerimientos = string.Empty;

        public ProyectoFormWindow(int idEmpresa, int? idProyecto = null)
        {
            InitializeComponent();
            _idEmpresa = idEmpresa;
            _idProyecto = idProyecto;
            Loaded += ProyectoFormWindow_Loaded;
        }

        private void ProyectoFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargarSupervisores();
            cbEstado.SelectedIndex = 0;

            if (_idProyecto.HasValue)
                CargarProyecto(_idProyecto.Value);

            RefrescarVistaPrevia();
        }

        // ── Carga inicial ────────────────────────────────────────────────────

        private void CargarSupervisores()
        {
            var supervisores = _proyectoService.ObtenerSupervisoresDisponibles();
            cbSupervisor.ItemsSource = supervisores;
            cbSupervisor.SelectedIndex = 0;
        }

        private void CargarProyecto(int idProyecto)
        {
            _proyectoEditando = _proyectoService.ObtenerPorId(idProyecto, _idEmpresa);

            if (_proyectoEditando == null)
            {
                MessageBox.Show("No se encontró el proyecto.");
                Close();
                return;
            }

            txtTituloVentana.Text = "Editar proyecto";
            txtSubtituloVentana.Text = "Modifica la información principal del proyecto.";
            txtModoVentana.Text = "Modo edición";
            btnGuardar.Content = "Guardar cambios";

            // Ocultar sección de subir archivos iniciales al editar
            spDocumentosCreacion.Visibility = Visibility.Collapsed;

            txtNombre.Text = _proyectoEditando.Nombre ?? string.Empty;
            txtDescripcion.Text = _proyectoEditando.Descripcion ?? string.Empty;
            slProgreso.Value = Convert.ToDouble(_proyectoEditando.Progreso.GetValueOrDefault());

            if (_proyectoEditando.FechaInicio.HasValue)
                dpFechaInicio.SelectedDate = _proyectoEditando.FechaInicio.Value;

            if (_proyectoEditando.FechaFin.HasValue)
                dpFechaFin.SelectedDate = _proyectoEditando.FechaFin.Value;

            foreach (ComboBoxItem item in cbEstado.Items)
            {
                if (string.Equals(item.Content?.ToString(), _proyectoEditando.Estado, StringComparison.OrdinalIgnoreCase))
                {
                    cbEstado.SelectedItem = item;
                    break;
                }
            }

            if (_proyectoEditando.IdSupervisor.HasValue && cbSupervisor.ItemsSource is System.Collections.IEnumerable items)
            {
                foreach (var item in items)
                {
                    if (item is UsuarioComboItem usuario && usuario.IdUsuario == _proyectoEditando.IdSupervisor.Value)
                    {
                        cbSupervisor.SelectedItem = item;
                        break;
                    }
                }
            }

            RefrescarVistaPrevia();
        }

        // ── Guardar ──────────────────────────────────────────────────────────

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();

                if (cbEstado.SelectedItem is not ComboBoxItem estadoItem)
                {
                    MessageBox.Show("Selecciona un estado.");
                    return;
                }

                string estado = estadoItem.Content?.ToString()?.Trim() ?? "Activo";
                DateTime? fechaInicio = dpFechaInicio.SelectedDate;
                DateTime? fechaFin = dpFechaFin.SelectedDate;

                int? idSupervisor = null;
                if (cbSupervisor.SelectedItem is UsuarioComboItem supervisor && supervisor.IdUsuario != 0)
                    idSupervisor = supervisor.IdUsuario;

                decimal progreso = Convert.ToDecimal(Math.Round(slProgreso.Value, 2));

                if (_proyectoEditando == null)
                {
                    // Validar documentos obligatorios
                    if (string.IsNullOrWhiteSpace(_rutaHistorias) || string.IsNullOrWhiteSpace(_rutaRequerimientos))
                    {
                        MessageBox.Show("Debes seleccionar obligatoriamente las Historias de Usuario y la Hoja de Requerimientos para crear el proyecto.", "Campos Requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    int nuevoIdProyecto = _proyectoService.GuardarProyecto(_idEmpresa, null, nombre, descripcion, estado, fechaInicio, fechaFin, idSupervisor, progreso);

                    // Subir los dos archivos iniciales obligatorios
                    var archivoService = new ArchivoService();
                    int idUsuarioLogueado = SesionActual.IdUsuario;

                    archivoService.SubirArchivoProyecto(_rutaHistorias, nuevoIdProyecto, idUsuarioLogueado, "Historias de Usuario");
                    archivoService.SubirArchivoProyecto(_rutaRequerimientos, nuevoIdProyecto, idUsuarioLogueado, "Hoja de Requerimientos");

                    MessageBox.Show("Proyecto creado correctamente junto con sus documentos iniciales.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _proyectoService.GuardarProyecto(_idEmpresa, _proyectoEditando.IdProyecto, nombre, descripcion, estado, fechaInicio, fechaFin, idSupervisor, progreso);
                    MessageBox.Show("Proyecto actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando proyecto:\n" + ex.Message);
            }
        }

        // ── UI helpers ───────────────────────────────────────────────────────

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnCerrarVentana_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnMinimizarVentana_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) return;
            DragMove();
        }

        private void slProgreso_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtProgresoValor.Text = $"{Math.Round(slProgreso.Value)}%";
            RefrescarVistaPrevia();
        }

        private void ActualizarVistaPrevia(object sender, EventArgs e) => RefrescarVistaPrevia();

        private void RefrescarVistaPrevia()
        {
            txtPreviewNombre.Text = string.IsNullOrWhiteSpace(txtNombre.Text)
                ? "Nombre del proyecto"
                : txtNombre.Text.Trim();

            txtPreviewDescripcion.Text = string.IsNullOrWhiteSpace(txtDescripcion.Text)
                ? "Descripción del proyecto."
                : txtDescripcion.Text.Trim();

            string estado = cbEstado.SelectedItem is ComboBoxItem estadoItem
                ? estadoItem.Content?.ToString() ?? "Activo"
                : "Activo";

            txtPreviewEstado.Text = estado;
            AplicarColorEstado(estado);

            string supervisorTexto = cbSupervisor.SelectedItem is UsuarioComboItem supervisor
                ? supervisor.NombreCompleto
                : "Sin asignar";

            txtPreviewSupervisor.Text = supervisorTexto;
            AplicarColorSupervisor(supervisorTexto);

            txtPreviewInicio.Text = dpFechaInicio.SelectedDate.HasValue
                ? dpFechaInicio.SelectedDate.Value.ToString("dd/MM/yyyy")
                : "-";

            txtPreviewFin.Text = dpFechaFin.SelectedDate.HasValue
                ? dpFechaFin.SelectedDate.Value.ToString("dd/MM/yyyy")
                : "-";

            pbPreviewProgreso.Value = slProgreso.Value;
            txtPreviewProgreso.Text = $"{Math.Round(slProgreso.Value)}%";
            txtProgresoValor.Text = $"{Math.Round(slProgreso.Value)}%";
        }

        private void AplicarColorEstado(string estado)
        {
            switch (estado)
            {
                case "Pausado":
                    bdPreviewEstado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B2A12"));
                    txtPreviewEstado.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCD34D"));
                    break;
                case "Finalizado":
                    bdPreviewEstado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#123026"));
                    txtPreviewEstado.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
                    break;
                default:
                    bdPreviewEstado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F"));
                    txtPreviewEstado.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
                    break;
            }
        }

        private void AplicarColorSupervisor(string supervisorTexto)
        {
            if (supervisorTexto == "Sin asignar")
            {
                bdPreviewSupervisor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D1B42"));
                txtPreviewSupervisor.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D8B4FE"));
                return;
            }
            bdPreviewSupervisor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#123026"));
            txtPreviewSupervisor.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
        }

        private void BtnSubirHistorias_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Seleccionar Historias de Usuario",
                    Filter = "Todos los archivos (*.*)|*.*|Documentos PDF (*.pdf)|*.pdf|Documentos Word (*.docx;*.doc)|*.docx;*.doc|Hojas de cálculo Excel (*.xlsx;*.xls)|*.xlsx;*.xls"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _rutaHistorias = openFileDialog.FileName;
                    txtRutaHistorias.Text = System.IO.Path.GetFileName(_rutaHistorias);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar Historias de Usuario:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSubirRequerimientos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Seleccionar Hoja de Requerimientos",
                    Filter = "Todos los archivos (*.*)|*.*|Documentos PDF (*.pdf)|*.pdf|Documentos Word (*.docx;*.doc)|*.docx;*.doc|Hojas de cálculo Excel (*.xlsx;*.xls)|*.xlsx;*.xls"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _rutaRequerimientos = openFileDialog.FileName;
                    txtRutaRequerimientos.Text = System.IO.Path.GetFileName(_rutaRequerimientos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar Hoja de Requerimientos:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
