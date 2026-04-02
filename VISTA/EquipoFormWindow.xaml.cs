using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VISTA
{
    public partial class EquipoFormWindow : Window
    {
        // ── Servicios BLL ────────────────────────────────────────────────────
        private readonly EquipoService _equipoService = new();
        private readonly PermisosService _permisosService = new();

        // ── Estado ───────────────────────────────────────────────────────────
        private readonly int _idProyecto;
        private readonly int? _idEquipo;
        private Equipo? _equipoEditando;

        private readonly List<MiembroDto> _miembros = new();

        public EquipoFormWindow(int idProyecto, int? idEquipo = null)
        {
            InitializeComponent();
            _idProyecto = idProyecto;
            _idEquipo = idEquipo;
            Loaded += EquipoFormWindow_Loaded;
        }

        private void EquipoFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtSupervisor.Text = SesionActual.NombreCompleto;

            if (_idEquipo.HasValue)
                CargarEquipo(_idEquipo.Value);

            RefrescarVista();
        }

        // ── Carga inicial ────────────────────────────────────────────────────

        private void CargarEquipo(int idEquipo)
        {
            _equipoEditando = _equipoService.ObtenerPorId(idEquipo);

            if (_equipoEditando == null)
            {
                MessageBox.Show("No se encontró el equipo.");
                Close();
                return;
            }

            txtTituloVentana.Text = "Editar equipo";
            txtSubtituloVentana.Text = "Modifica el nombre, descripción y trabajadores.";
            btnGuardar.Content = "Guardar cambios";

            txtNombre.Text = _equipoEditando.Nombre;
            txtDescripcion.Text = _equipoEditando.Descripcion ?? "";

            // Cargar nombre del supervisor vía servicio
            var miembrosEquipo = _equipoService.ObtenerMiembros(idEquipo);
            // El supervisor se muestra desde SesionActual al crear; al editar lo cargamos del equipo
            // Para el supervisor usamos el EquipoDto cargado en la pantalla principal
            // txtSupervisor ya fue asignado en Loaded; aquí mantenemos ese comportamiento

            _miembros.Clear();
            _miembros.AddRange(miembrosEquipo);

            RefrescarVista();
        }

        // ── Miembros ─────────────────────────────────────────────────────────

        private void BtnAgregarCorreo_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreoTrabajador.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show("Escribe el correo del trabajador.");
                txtCorreoTrabajador.Focus();
                return;
            }

            var miembro = _equipoService.BuscarEmpleadoPorCorreo(correo);

            if (miembro == null)
            {
                MessageBox.Show("No existe un trabajador activo con ese correo.");
                return;
            }

            if (_miembros.Any(x => x.IdUsuario == miembro.IdUsuario))
            {
                MessageBox.Show("Ese trabajador ya fue agregado.");
                return;
            }

            _miembros.Add(miembro);
            txtCorreoTrabajador.Clear();
            RefrescarVista();
        }

        private void BtnQuitarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            if (dgMiembros.SelectedItem is not MiembroDto seleccionado)
            {
                MessageBox.Show("Selecciona un trabajador de la lista.");
                return;
            }

            _miembros.RemoveAll(x => x.IdUsuario == seleccionado.IdUsuario);
            RefrescarVista();
        }

        // ── Guardar ──────────────────────────────────────────────────────────

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_permisosService.PuedeGestionarEquipos(_idProyecto))
                {
                    MessageBox.Show("No tienes permisos para gestionar equipos en este proyecto.");
                    return;
                }

                string nombre = txtNombre.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();

                var idsUsuarios = _miembros.Select(m => m.IdUsuario).ToList();

                if (_equipoEditando == null)
                {
                    // Crear nuevo equipo
                    var nuevoEquipo = new Equipo
                    {
                        IdProyecto = _idProyecto,
                        IdSupervisor = SesionActual.IdUsuario,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Activo = 1,
                        FechaCreacion = DateTime.Now
                    };

                    _equipoService.CrearEquipo(nuevoEquipo, idsUsuarios);
                    MessageBox.Show("Equipo creado correctamente.");
                }
                else
                {
                    // Editar equipo existente
                    _equipoEditando.Nombre = nombre;
                    _equipoEditando.Descripcion = descripcion;

                    _equipoService.EditarEquipo(_equipoEditando, idsUsuarios);
                    MessageBox.Show("Equipo actualizado correctamente.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando equipo:\n" + ex.Message);
            }
        }

        // ── UI helpers ───────────────────────────────────────────────────────

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void RefrescarVista()
        {
            dgMiembros.ItemsSource = null;
            dgMiembros.ItemsSource = _miembros.OrderBy(x => x.NombreCompleto).ToList();

            txtResumenNombre.Text = string.IsNullOrWhiteSpace(txtNombre.Text)
                ? "Sin nombre"
                : txtNombre.Text.Trim();

            txtResumenSupervisor.Text = txtSupervisor.Text;
            txtCantidadMiembros.Text = _miembros.Count.ToString();
        }
    }
}