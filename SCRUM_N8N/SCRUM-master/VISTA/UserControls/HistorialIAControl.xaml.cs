using BLL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace VISTA.UserControls
{
    public partial class HistorialIAControl : UserControl
    {
        private readonly AnalisisTecnicoService _analisisService = new();

        public int IdProyecto { get; set; }

        public HistorialIAControl()
        {
            InitializeComponent();
        }

        // ── Carga inicial ────────────────────────────────────────────────────

        public void CargarHistorial()
        {
            try
            {
                var historial = _analisisService.ObtenerHistorialPorProyecto(IdProyecto);
                dgHistorial.ItemsSource = historial;
                LimpiarDetalle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial IA:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Selección de fila ────────────────────────────────────────────────

        private void DgHistorial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgHistorial.SelectedItem is not AnalisisHistorialDto sel)
            {
                LimpiarDetalle();
                return;
            }

            try
            {
                var detalle = _analisisService.ObtenerDetalle(sel.IdAnalisis);
                MostrarDetalle(detalle, sel.TipoOperacion);
            }
            catch (Exception ex)
            {
                LimpiarDetalle();
                MessageBox.Show("Error al cargar el detalle:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Mostrar detalle ──────────────────────────────────────────────────

        private void MostrarDetalle(AnalisisDetalleDto detalle, string tipoOperacion)
        {
            panelDetalleVacio.Visibility = Visibility.Collapsed;
            gridDetalle.Visibility       = Visibility.Visible;

            bool esAnalisis = tipoOperacion == "Análisis";

            // Metadatos comunes
            txtDetalleTipo.Text          = esAnalisis ? "🔍  Análisis IA" : "⚡  Generación de tareas IA";
            txtDetalleDocumento.Text     = detalle.NombreDocumento;
            txtDetalleFechaAnalisis.Text = detalle.FechaAnalisis;
            txtDetalleEstado.Text        = detalle.Estado;

            // Campos específicos
            spCamposAnalisis.Visibility = esAnalisis ? Visibility.Visible : Visibility.Collapsed;
            spCamposTareas.Visibility   = esAnalisis ? Visibility.Collapsed : Visibility.Visible;

            if (esAnalisis)
            {
                txtDetalleHU.Text        = detalle.HuProcesadas;
                txtDetalleRF.Text        = detalle.RfProcesados;
                txtLabelJson.Text        = "JSON del resultado del análisis";
                txtDetalleJson.Text      = detalle.ResultadoJson.Length > 0
                                           ? detalle.ResumenAnalisis
                                           : "(sin datos)";
            }
            else
            {
                txtDetalleTareasResumen.Text  = detalle.TareasGeneradasResumen;
                txtDetalleEquipos.Text        = detalle.EquiposAsignados;
                txtDetalleFechaGeneracion.Text = detalle.FechaGeneracionTareas;
                txtLabelJson.Text             = "JSON de las tareas generadas";
                txtDetalleJson.Text           = string.IsNullOrWhiteSpace(detalle.TareasGeneradasJson)
                                               ? "(sin datos)"
                                               : FormatearJson(detalle.TareasGeneradasJson);
            }
        }

        private void LimpiarDetalle()
        {
            panelDetalleVacio.Visibility = Visibility.Visible;
            gridDetalle.Visibility       = Visibility.Collapsed;
        }

        // ── Refrescar ────────────────────────────────────────────────────────

        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
            => CargarHistorial();

        // ── Helpers ──────────────────────────────────────────────────────────

        private string FormatearJson(string json)
        {
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(json);
                return System.Text.Json.JsonSerializer.Serialize(
                    doc, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json;
            }
        }
    }
}
