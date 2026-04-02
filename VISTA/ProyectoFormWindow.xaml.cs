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

            txtNombre.Text = _proyectoEditando.Nombre ?? string.Empty;
            txtDescripcion.Text = _proyectoEditando.Descripcion ?? string.Empty;
            slProgreso.Value = Convert.ToDouble(_proyectoEditando.Progreso ?? 0);

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
                    // Crear
                    var nuevoProyecto = new Proyecto
                    {
                        IdEmpresa = _idEmpresa,
                        IdSupervisor = idSupervisor,
                        Nombre = nombre,
                        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion,
                        Estado = estado,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        Progreso = progreso,
                        Activo = 1,
                        FechaCreacion = DateTime.Now
                    };

                    _proyectoService.CrearProyecto(nuevoProyecto);
                    MessageBox.Show("Proyecto creado correctamente.");
                }
                else
                {
                    // Editar
                    _proyectoEditando.IdSupervisor = idSupervisor;
                    _proyectoEditando.Nombre = nombre;
                    _proyectoEditando.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion;
                    _proyectoEditando.Estado = estado;
                    _proyectoEditando.FechaInicio = fechaInicio;
                    _proyectoEditando.FechaFin = fechaFin;
                    _proyectoEditando.Progreso = progreso;

                    _proyectoService.EditarProyecto(_proyectoEditando);
                    MessageBox.Show("Proyecto actualizado correctamente.");
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
    }
}
