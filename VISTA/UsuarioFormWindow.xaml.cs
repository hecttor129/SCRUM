using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VISTA
{
    public partial class UsuarioFormWindow : Window
    {
        private readonly UsuarioService _service;
        private readonly int? _idUsuario;
        private int _adminId = 0;

        // Lista en memoria de especialidades del usuario actual
        private readonly ObservableCollection<string> _especializaciones = new();

        // Mapeo DiaSemana → TextBox (0=Dom, 1=Lun, … 6=Sáb)
        private Dictionary<int, TextBox> _txtHoras;

        public UsuarioFormWindow(int? idUsuario = null)
        {
            InitializeComponent();
            _service   = new UsuarioService();
            _idUsuario = idUsuario;

            // Inicializar mapeo de horas DESPUÉS de InitializeComponent
            _txtHoras = new Dictionary<int, TextBox>
            {
                { 0, txtHrsDom },
                { 1, txtHrsLun },
                { 2, txtHrsMar },
                { 3, txtHrsMie },
                { 4, txtHrsJue },
                { 5, txtHrsVie },
                { 6, txtHrsSab }
            };

            Loaded += UsuarioFormWindow_Loaded;
        }

        // ── Carga inicial ────────────────────────────────────────────────────

        private void UsuarioFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Cargar superiores (excluyendo al propio usuario en modo edición)
            var supervisores = _service.ObtenerSupervisoresDisponibles(_idUsuario);
            cbSuperior.ItemsSource = supervisores;

            var admin = supervisores.FirstOrDefault(s => s.NivelJerarquico == 1);
            if (admin != null) _adminId = admin.IdUsuario;

            // Cargar sugerencias de especialidades existentes en el sistema
            var todasEspecialidades = _service.ObtenerTodasLasEspecialidades();
            cbEspecialidad.ItemsSource = todasEspecialidades;

            if (_idUsuario == null)
            {
                txtTitulo.Text         = "Nuevo usuario";
                btnGuardar.Content     = "Crear usuario";
                txtHintPwd.Visibility  = Visibility.Collapsed;
                cbRol.SelectedIndex    = 1; // Empleado
                if (cbSuperior.Items.Count > 0) cbSuperior.SelectedIndex = 0;
            }
            else
            {
                txtTitulo.Text        = "Editar usuario";
                btnGuardar.Content    = "Guardar cambios";
                txtHintPwd.Visibility = Visibility.Visible;
                CargarDatos(_idUsuario.Value);
            }

            // Renderizar tags iniciales (vacío al crear, cargados al editar)
            RenderizarTags();
        }

        private void CargarDatos(int id)
        {
            try
            {
                var usuario = _service.ObtenerTodos().FirstOrDefault(u => u.IdUsuario == id);
                if (usuario == null) { Close(); return; }

                txtNombre.Text  = usuario.Nombre;
                txtApellido.Text = usuario.Apellido;
                txtEmail.Text   = usuario.Email;
                txtSalario.Text = usuario.Salario?.ToString();

                // Rol
                foreach (ComboBoxItem item in cbRol.Items)
                {
                    if (item.Content.ToString() == usuario.Rol.ToString())
                    {
                        cbRol.SelectedItem = item;
                        break;
                    }
                }

                // Superior directo
                var rel = _service.ObtenerSuperiorActual(id);
                if (rel != null)
                {
                    var supItem = cbSuperior.Items
                        .Cast<UsuarioSuperiorItem>()
                        .FirstOrDefault(s => s.IdUsuario == rel.IdJefe);
                    if (supItem != null)
                    {
                        cbSuperior.SelectedItem = supItem;
                        txtNivelCalculado.Text =
                            $"Nivel jerárquico asignado: {supItem.NivelJerarquico + 1}";
                    }
                }
                else if (cbSuperior.Items.Count > 0)
                {
                    cbSuperior.SelectedIndex = 0;
                    if (cbSuperior.SelectedItem is UsuarioSuperiorItem s)
                        txtNivelCalculado.Text = $"Nivel jerárquico asignado: {s.NivelJerarquico + 1}";
                }

                // Especialidades
                _especializaciones.Clear();
                foreach (var esp in (usuario.Especializaciones ?? new List<string>()))
                    _especializaciones.Add(esp);

                // Disponibilidad semanal
                var disponibilidades = _service.ObtenerDisponibilidadUsuario(id);
                foreach (var d in disponibilidades)
                {
                    if (d.DiaSemana.HasValue && _txtHoras.TryGetValue(d.DiaSemana.Value, out var tb))
                        tb.Text = (d.CapacidadPorDia ?? 0).ToString("0.##");
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        // ── Especialidades ───────────────────────────────────────────────────

        private void BtnAgregarEspecialidad_Click(object sender, RoutedEventArgs e)
            => AgregarEspecialidad();

        private void CbEspecialidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) AgregarEspecialidad();
        }

        private void AgregarEspecialidad()
        {
            string valor = (cbEspecialidad.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(valor)) return;

            if (_especializaciones.Any(x =>
                    string.Equals(x, valor, StringComparison.OrdinalIgnoreCase)))
            {
                cbEspecialidad.Text = string.Empty;
                return; // ya existe, no duplicar
            }

            _especializaciones.Add(valor);
            cbEspecialidad.Text = string.Empty;
            RenderizarTags();
        }

        private void RenderizarTags()
        {
            wrapEspecialidades.Children.Clear();
            foreach (var esp in _especializaciones)
            {
                var tag = CrearTag(esp);
                wrapEspecialidades.Children.Add(tag);
            }
        }

        private Border CrearTag(string texto)
        {
            var lbl = new TextBlock
            {
                Text = texto,
                Foreground = Brushes.White,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };

            var closeBtn = new TextBlock
            {
                Text = "✕",
                Foreground = Brushes.White,
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Hand,
                Tag = texto
            };
            closeBtn.MouseLeftButtonUp += (s, e) =>
            {
                _especializaciones.Remove(texto);
                RenderizarTags();
            };

            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 4, 4)
            };
            stack.Children.Add(lbl);
            stack.Children.Add(closeBtn);

            return new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(37, 99, 235)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(10, 4, 10, 4),
                Child = stack
            };
        }

        // ── Navegación del ComboBox superior ────────────────────────────────

        private void CbSuperior_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSuperior.SelectedItem is UsuarioSuperiorItem s)
                txtNivelCalculado.Text = $"Nivel jerárquico asignado: {s.NivelJerarquico + 1}";
        }

        // ── Guardar ──────────────────────────────────────────────────────────

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;

            try
            {
                // Rol
                string rolStr = ((ComboBoxItem)cbRol.SelectedItem).Content.ToString();
                var rol = rolStr == "Jefe" ? ENUMS.RolUsuario.Jefe : ENUMS.RolUsuario.Empleado;

                // Superior
                int idSup = _adminId;
                if (cbSuperior.SelectedItem is UsuarioSuperiorItem s) idSup = s.IdUsuario;

                // Salario
                decimal? sal = null;
                if (decimal.TryParse(txtSalario.Text, out decimal parsedSal)) sal = parsedSal;

                // Especialidades
                var especializaciones = _especializaciones.ToList();

                // Disponibilidad
                var horasPorDia = LeerHorasDisponibilidad();

                if (_idUsuario == null)
                {
                    _service.CrearUsuario(
                        txtNombre.Text, txtApellido.Text, txtEmail.Text,
                        pwdPassword.Password, rol, idSup,
                        especializaciones, horasPorDia);
                }
                else
                {
                    _service.EditarUsuario(
                        _idUsuario.Value, txtNombre.Text, txtApellido.Text,
                        txtEmail.Text, rol, sal, idSup,
                        especializaciones, horasPorDia);

                    if (!string.IsNullOrWhiteSpace(pwdPassword.Password))
                        _service.CambiarPassword(_idUsuario.Value, pwdPassword.Password);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        // ── Utilidades ───────────────────────────────────────────────────────

        /// <summary>
        /// Lee los 7 TextBox de horas y los convierte al diccionario DiaSemana → horas.
        /// </summary>
        private Dictionary<int, decimal> LeerHorasDisponibilidad()
        {
            var result = new Dictionary<int, decimal>();
            foreach (var kvp in _txtHoras)
            {
                string raw = kvp.Value.Text.Trim();
                if (!decimal.TryParse(raw, out decimal horas) || horas < 0 || horas > 24)
                    throw new Exception(
                        $"El valor de horas para el día {NombreDia(kvp.Key)} debe ser un número entre 0 y 24.");
                result[kvp.Key] = horas;
            }
            return result;
        }

        private static string NombreDia(int diaSemana) => diaSemana switch
        {
            0 => "Domingo",
            1 => "Lunes",
            2 => "Martes",
            3 => "Miércoles",
            4 => "Jueves",
            5 => "Viernes",
            6 => "Sábado",
            _ => $"día {diaSemana}"
        };

        private void MostrarError(string msg)
        {
            txtError.Text = msg;
            txtError.Visibility = Visibility.Visible;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();
    }
}
