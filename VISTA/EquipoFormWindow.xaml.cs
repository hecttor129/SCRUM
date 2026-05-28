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
            cbSupervisor.ItemsSource = _equipoService.ObtenerSupervisoresEquipo();
            cbTrabajador.ItemsSource = _equipoService.ObtenerCandidatosEquipo();

            if (_idEquipo.HasValue)
                CargarEquipo(_idEquipo.Value);
            else
            {
                if (cbSupervisor.ItemsSource is List<MiembroDto> supervisores)
                    cbSupervisor.SelectedItem = supervisores.FirstOrDefault(s => s.IdUsuario == SesionActual.IdUsuario);
            }

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

            if (cbSupervisor.ItemsSource is List<MiembroDto> supervisores)
            {
                cbSupervisor.SelectedItem = supervisores.FirstOrDefault(s => s.IdUsuario == _equipoEditando.IdSupervisor);
            }

            var miembrosEquipo = _equipoService.ObtenerMiembros(idEquipo);

            _miembros.Clear();
            _miembros.AddRange(miembrosEquipo);

            RefrescarVista();
        }

        // ── Miembros ─────────────────────────────────────────────────────────

        private void BtnAgregarCorreo_Click(object sender, RoutedEventArgs e)
        {
            if (cbTrabajador.SelectedItem is not MiembroDto miembro)
            {
                MessageBox.Show("Selecciona un trabajador de la lista.");
                return;
            }

            if (_miembros.Any(x => x.IdUsuario == miembro.IdUsuario))
            {
                MessageBox.Show("Ese trabajador ya fue agregado.");
                return;
            }

            _miembros.Add(miembro);
            cbTrabajador.SelectedItem = null;
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

                int idSupervisor = SesionActual.IdUsuario;
                if (cbSupervisor.SelectedItem is MiembroDto sup)
                    idSupervisor = sup.IdUsuario;

                if (_equipoEditando == null)
                {
                    _equipoService.GuardarEquipo(_idProyecto, null, nombre, descripcion, idsUsuarios, idSupervisor);
                    MessageBox.Show("Equipo creado correctamente.");
                }
                else
                {
                    _equipoService.GuardarEquipo(_idProyecto, _equipoEditando.IdEquipo, nombre, descripcion, idsUsuarios, idSupervisor);
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

        private void TopBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                DragMove();
        }

        private void TxtNombre_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txtResumenNombre.Text = string.IsNullOrWhiteSpace(txtNombre.Text)
                ? "Sin nombre"
                : txtNombre.Text.Trim();
        }

        private void CbSupervisor_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            txtResumenSupervisor.Text = cbSupervisor.SelectedItem is MiembroDto sup
                ? sup.NombreCompleto
                : "Sin asignar";
        }

        private void RefrescarVista()
        {
            dgMiembros.ItemsSource = null;
            dgMiembros.ItemsSource = _miembros.OrderBy(x => x.NombreCompleto).ToList();

            txtResumenNombre.Text = string.IsNullOrWhiteSpace(txtNombre.Text)
                ? "Sin nombre"
                : txtNombre.Text.Trim();

            txtResumenSupervisor.Text = cbSupervisor.SelectedItem is MiembroDto sup
                ? sup.NombreCompleto
                : "Sin asignar";

            txtCantidadMiembros.Text = _miembros.Count.ToString();
        }
    }
}