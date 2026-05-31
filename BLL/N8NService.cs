using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL
{
    public class N8NService
    {
        // Flujo 1: análisis de documento
        private const string WebhookAnalisis =
            "https://flujoscrum.app.n8n.cloud/webhook/generate_backlog";

        // Flujo 2: generación de tareas a partir de un análisis previo
        private const string WebhookGenerarTareas =
            "https://flujoscrum.app.n8n.cloud/webhook/generate_backlog";

        // ── Flujo 1: Analizar documento ─────────────────────────────────────

        /// <summary>
        /// Envía un archivo al flujo de análisis de n8n y retorna el JSON de respuesta.
        /// </summary>
        public async Task<string> AnalizarDocumentoAsync(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.");

            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException(
                    "El archivo seleccionado no existe o fue movido.", rutaArchivo);

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(180);

            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(rutaArchivo);
            using var fileContent = new StreamContent(fileStream);

            string contentType = ObtenerContentType(Path.GetExtension(rutaArchivo));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            // "data" es el nombre del campo que espera el webhook de n8n
            form.Add(fileContent, "data", Path.GetFileName(rutaArchivo));

            var response = await client.PostAsync(WebhookAnalisis, form);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error del servidor n8n ({(int)response.StatusCode}): {errorBody}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        // ── Flujo 2: Generar tareas desde un análisis previo ────────────────

        /// <summary>
        /// Envía el IdAnalisis al flujo de generación de tareas de n8n
        /// y retorna el JSON con las tareas propuestas.
        /// </summary>
        public async Task<string> GenerarTareasAsync(int idAnalisis)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(180);

            var payload = JsonSerializer.Serialize(new { id_analisis = idAnalisis });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(WebhookGenerarTareas, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error del servidor n8n ({(int)response.StatusCode}): {errorBody}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private string ObtenerContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".pdf"  => "application/pdf",
                ".doc"  => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls"  => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt"  => "text/plain",
                _       => "application/octet-stream"
            };
        }
    }
}
