using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VISTA.UserControls
{
    public partial class DocumentoAnalisisControl : UserControl
    {
        // ── Servicios ────────────────────────────────────────────────────────
        private readonly N8NService                _n8nService    = new();
        private readonly AnalisisTecnicoService    _analisisService = new();
        private readonly TareaService              _tareaService  = new();
        private readonly ArchivoService            _archivoService = new();

        // ── Estado ───────────────────────────────────────────────────────────
        public int IdProyecto { get; set; }

        private string  _rutaArchivoSeleccionado = string.Empty;
        private string  _nombreArchivoSeleccionado = string.Empty;
        private int?    _idAnalisisActual;          // IdAnalisis persistido tras el análisis
        private string  _jsonTareasPendientes = string.Empty; // JSON de tareas pendiente de confirmar

        public DocumentoAnalisisControl()
        {
            InitializeComponent();
        }

        // ── Selección de archivo ─────────────────────────────────────────────

        private void BtnSeleccionarArchivo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title  = "Seleccionar documento de requerimientos",
                Filter = "Todos los archivos (*.*)|*.*|PDF (*.pdf)|*.pdf|Word (*.docx;*.doc)|*.docx;*.doc|Excel (*.xlsx;*.xls)|*.xlsx;*.xls"
            };

            if (dlg.ShowDialog() != true) return;

            _rutaArchivoSeleccionado   = dlg.FileName;
            _nombreArchivoSeleccionado = System.IO.Path.GetFileName(dlg.FileName);
            txtNombreArchivo.Text      = _nombreArchivoSeleccionado;
            txtNombreArchivo.Foreground = (System.Windows.Media.Brush)FindResource("TextPrimary");

            // Habilitar botón de análisis y resetear estado
            btnGenerarAnalisis.IsEnabled = true;
            btnGenerarTareas.IsEnabled   = false;
            _idAnalisisActual            = null;
            _jsonTareasPendientes        = string.Empty;

            // Resetear paneles
            MostrarEstadoAnalisis(EstadoPanel.Vacio);
            MostrarEstadoTareas(EstadoPanel.Vacio);
            txtInfoAnalisisActivo.Text = $"Documento listo: {_nombreArchivoSeleccionado}";
        }

        // ── Generar análisis ─────────────────────────────────────────────────

        private async void BtnGenerarAnalisis_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_rutaArchivoSeleccionado))
            {
                MessageBox.Show("Selecciona un archivo primero.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetBotonesHabilitados(false);
            MostrarEstadoAnalisis(EstadoPanel.Cargando);
            txtInfoAnalisisActivo.Text = "Analizando documento con IA...";

            try
            {
                // 1. Llamar al webhook de análisis
                string resultado = await _n8nService.AnalizarDocumentoAsync(_rutaArchivoSeleccionado);

                // 2. Persistir en BD
                _idAnalisisActual = _analisisService.GuardarAnalisis(
                    IdProyecto,
                    _nombreArchivoSeleccionado,
                    resultado);

                // 3. Mostrar resultado en el panel izquierdo
                txtResultadoAnalisis.Text = FormatearJson(resultado);
                MostrarEstadoAnalisis(EstadoPanel.Resultado);

                // 4. Habilitar generación de tareas
                btnGenerarTareas.IsEnabled = true;
                txtInfoAnalisisActivo.Text =
                    $"Análisis completado y guardado (Id: {_idAnalisisActual}). " +
                    "Ya puedes generar las tareas.";
            }
            catch (Exception ex)
            {
                MostrarEstadoAnalisis(EstadoPanel.Vacio);
                MessageBox.Show("Error al analizar el documento:\n" + ex.Message,
                    "Error de análisis", MessageBoxButton.OK, MessageBoxImage.Error);
                txtInfoAnalisisActivo.Text = "El análisis falló. Intenta nuevamente.";
            }
            finally
            {
                btnGenerarAnalisis.IsEnabled = true;
                btnGenerarTareas.IsEnabled   = _idAnalisisActual.HasValue;
            }
        }

        // ── Generar tareas ───────────────────────────────────────────────────

        private async void BtnGenerarTareas_Click(object sender, RoutedEventArgs e)
        {
            if (!_idAnalisisActual.HasValue)
            {
                MessageBox.Show("Primero debes generar un análisis.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetBotonesHabilitados(false);
            MostrarEstadoTareas(EstadoPanel.Cargando);
            txtInfoAnalisisActivo.Text = "Generando planificación de tareas...";

            try
            {
                // 1. Llamar al webhook de generación de tareas con el IdAnalisis
                string resultado = await _n8nService.GenerarTareasAsync(_idAnalisisActual.Value);

                // 2. Persistir el JSON de tareas en el análisis
                _analisisService.GuardarResultadoTareas(_idAnalisisActual.Value, resultado);

                // 3. Guardar JSON para cuando el usuario confirme
                _jsonTareasPendientes = resultado;

                // 4. Mostrar en panel derecho con opción de confirmar
                txtResultadoTareas.Text = FormatearJson(resultado);
                txtResumenTareas.Text   = ExtraerResumenTareas(resultado);
                MostrarEstadoTareas(EstadoPanel.Resultado);

                txtInfoAnalisisActivo.Text = "Tareas generadas. Revísalas y confirma la inserción.";
            }
            catch (Exception ex)
            {
                MostrarEstadoTareas(EstadoPanel.Vacio);
                MessageBox.Show("Error al generar tareas:\n" + ex.Message,
                    "Error de generación", MessageBoxButton.OK, MessageBoxImage.Error);
                txtInfoAnalisisActivo.Text = "La generación de tareas falló. Intenta nuevamente.";
            }
            finally
            {
                SetBotonesHabilitados(true);
            }
        }

        // ── Confirmar inserción de tareas ────────────────────────────────────

        private void BtnConfirmarTareas_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_jsonTareasPendientes) || !_idAnalisisActual.HasValue)
            {
                MessageBox.Show("No hay tareas pendientes de confirmar.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var tareas = ParsearTareasDesdeJson(_jsonTareasPendientes, IdProyecto);

                if (tareas.Count == 0)
                {
                    MessageBox.Show(
                        "No se pudieron interpretar las tareas del JSON devuelto por la IA.\n\n" +
                        "El JSON no contiene un array de tareas con los campos esperados (titulo, descripcion, etc.).\n" +
                        "Las tareas han sido guardadas en el historial para revisión.",
                        "Sin tareas para insertar",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"Se van a insertar {tareas.Count} tarea(s) en el proyecto.\n¿Deseas continuar?",
                    "Confirmar inserción",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion != MessageBoxResult.Yes) return;

                _tareaService.CrearTareasDesdeIA(tareas, _idAnalisisActual.Value);

                MessageBox.Show(
                    $"✅ {tareas.Count} tarea(s) insertada(s) correctamente con origen IA.",
                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Deshabilitar para evitar doble inserción
                btnConfirmarTareas.IsEnabled = false;
                txtInfoAnalisisActivo.Text   = $"{tareas.Count} tarea(s) insertadas. Proceso completado.";
                _jsonTareasPendientes        = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar las tareas:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Parseo de JSON de tareas ─────────────────────────────────────────

        /// <summary>
        /// Intenta parsear el JSON de tareas retornado por n8n en una lista de entidades Tarea.
        /// Es resiliente a diferentes estructuras de respuesta.
        /// </summary>
        private List<Tarea> ParsearTareasDesdeJson(string json, int idProyecto)
        {
            var resultado = new List<Tarea>();
            if (string.IsNullOrWhiteSpace(json)) return resultado;

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Caso 1: JSON es directamente un array de tareas
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in root.EnumerateArray())
                        resultado.Add(MapearTarea(item, idProyecto));
                }
                // Caso 2: JSON tiene campo "tareas" que es un array
                else if (root.ValueKind == JsonValueKind.Object &&
                         root.TryGetProperty("tareas", out var tareas) &&
                         tareas.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in tareas.EnumerateArray())
                        resultado.Add(MapearTarea(item, idProyecto));
                }
                // Caso 3: JSON es un objeto con campo "backlog"
                else if (root.ValueKind == JsonValueKind.Object &&
                         root.TryGetProperty("backlog", out var backlog) &&
                         backlog.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in backlog.EnumerateArray())
                        resultado.Add(MapearTarea(item, idProyecto));
                }
            }
            catch
            {
                // Si falla el parseo, retornar lista vacía
            }

            // Filtrar tareas sin título
            resultado.RemoveAll(t => string.IsNullOrWhiteSpace(t.Titulo));
            return resultado;
        }

        private Tarea MapearTarea(JsonElement item, int idProyecto)
        {
            string titulo = ObtenerString(item, "titulo", "title", "nombre", "name", "Titulo");
            string descripcion = ObtenerString(item, "descripcion", "description", "Descripcion");
            string especializacion = ObtenerString(item, "especializacion", "especializacion_requerida", "skill", "area");
            int prioridad = ObtenerInt(item, "prioridad", "priority", "Prioridad");

            // Truncar a 500 chars si viene muy largo
            if (titulo.Length > 500) titulo = titulo[..500];

            return new Tarea
            {
                Titulo                  = titulo,
                Descripcion             = descripcion,
                EspecializacionRequerida = especializacion,
                Prioridad               = prioridad > 0 && prioridad <= 5 ? prioridad : 3,
                estadoTarea             = ENTITY.ENUMS.EstadoTarea.Pendiente,
                IdProyecto              = idProyecto,
                FechaCreacion           = DateTime.Now,
                Disponible              = true
            };
        }

        private string ObtenerString(JsonElement el, params string[] campos)
        {
            foreach (var c in campos)
                if (el.TryGetProperty(c, out var p) && p.ValueKind == JsonValueKind.String)
                    return p.GetString() ?? string.Empty;
            return string.Empty;
        }

        private int ObtenerInt(JsonElement el, params string[] campos)
        {
            foreach (var c in campos)
                if (el.TryGetProperty(c, out var p) && p.ValueKind == JsonValueKind.Number)
                    return p.GetInt32();
            return 3; // prioridad media por defecto
        }

        // ── Helpers de UI ────────────────────────────────────────────────────

        private enum EstadoPanel { Vacio, Cargando, Resultado }

        private void MostrarEstadoAnalisis(EstadoPanel estado)
        {
            panelAnalisisVacio.Visibility     = estado == EstadoPanel.Vacio     ? Visibility.Visible : Visibility.Collapsed;
            panelCargandoAnalisis.Visibility  = estado == EstadoPanel.Cargando  ? Visibility.Visible : Visibility.Collapsed;
            scrollAnalisis.Visibility         = estado == EstadoPanel.Resultado ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MostrarEstadoTareas(EstadoPanel estado)
        {
            panelTareasVacio.Visibility      = estado == EstadoPanel.Vacio     ? Visibility.Visible : Visibility.Collapsed;
            panelCargandoTareas.Visibility   = estado == EstadoPanel.Cargando  ? Visibility.Visible : Visibility.Collapsed;
            gridResultadoTareas.Visibility   = estado == EstadoPanel.Resultado ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetBotonesHabilitados(bool habilitado)
        {
            btnGenerarAnalisis.IsEnabled = habilitado;
            btnGenerarTareas.IsEnabled   = habilitado && _idAnalisisActual.HasValue;
            btnConfirmarTareas.IsEnabled  = habilitado && !string.IsNullOrWhiteSpace(_jsonTareasPendientes);
        }

        private string FormatearJson(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json;
            }
        }

        private string ExtraerResumenTareas(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                    return $"{root.GetArrayLength()} tarea(s) propuesta(s) por la IA";
                if (root.TryGetProperty("tareas", out var t) && t.ValueKind == JsonValueKind.Array)
                    return $"{t.GetArrayLength()} tarea(s) propuesta(s) por la IA";
                if (root.TryGetProperty("backlog", out var b) && b.ValueKind == JsonValueKind.Array)
                    return $"{b.GetArrayLength()} tarea(s) propuesta(s) por la IA";
            }
            catch { }
            return "Revisa el JSON para ver el detalle de las tareas";
        }
    }
}
