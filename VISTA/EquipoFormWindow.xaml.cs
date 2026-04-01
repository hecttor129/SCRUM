using DAL;
using ENTITY;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VISTA
{
    public partial class EquipoFormWindow : Window
    {
        private readonly DB_Context _context = new DB_Context();
        private readonly int _idProyecto;
        private readonly int? _idEquipo;
        private Equipo? _equipoEditando;

        private readonly List<MiembroTemp> _miembros = new();

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
            {
                CargarEquipo(_idEquipo.Value);
            }

            RefrescarVista();
        }

        private void CargarEquipo(int idEquipo)
        {
            _equipoEditando = _context.Equipos.FirstOrDefault(e => e.IdEquipo == idEquipo && e.Activo == 1);

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

            var supervisor = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.IdUsuario == _equipoEditando.IdSupervisor);

            txtSupervisor.Text = supervisor == null
                ? "Sin supervisor"
                : $"{supervisor.Nombre} {supervisor.Apellido}".Trim();

            var miembrosDb = (
                from eu in _context.EquipoUsuarios.AsNoTracking()
                join u in _context.Usuarios.AsNoTracking()
                    on eu.IdUsuario equals u.IdUsuario
                where eu.IdEquipo == idEquipo && eu.Activo == 1
                select new MiembroTemp
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = (u.Nombre + " " + u.Apellido).Trim(),
                    Correo = u.Email
                }).ToList();

            _miembros.Clear();
            _miembros.AddRange(miembrosDb);

            RefrescarVista();
        }

        private void BtnAgregarCorreo_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreoTrabajador.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show("Escribe el correo del trabajador.");
                txtCorreoTrabajador.Focus();
                return;
            }

            var usuario = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u =>
                    u.Email.ToLower() == correo &&
                    u.Activo == 1 &&
                    u.Rol == ENTITY.ENUMS.RolUsuario.Empleado);

            if (usuario == null)
            {
                MessageBox.Show("No existe un trabajador activo con ese correo.");
                return;
            }

            if (_miembros.Any(x => x.IdUsuario == usuario.IdUsuario))
            {
                MessageBox.Show("Ese trabajador ya fue agregado.");
                return;
            }

            _miembros.Add(new MiembroTemp
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                Correo = usuario.Email
            });

            txtCorreoTrabajador.Clear();
            RefrescarVista();
        }

        private void BtnQuitarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            if (dgMiembros.SelectedItem is not MiembroTemp seleccionado)
            {
                MessageBox.Show("Selecciona un trabajador de la lista.");
                return;
            }

            _miembros.RemoveAll(x => x.IdUsuario == seleccionado.IdUsuario);
            RefrescarVista();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!PermisosEquipoHelper.PuedeGestionarEquipos(_context, _idProyecto))
                {
                    MessageBox.Show("No tienes permisos para gestionar equipos en este proyecto.");
                    return;
                }

                string nombre = txtNombre.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("El nombre del equipo es obligatorio.");
                    txtNombre.Focus();
                    return;
                }

                bool nombreRepetido = _context.Equipos.Any(e =>
                    e.IdProyecto == _idProyecto &&
                    e.Activo == 1 &&
                    e.Nombre.ToLower() == nombre.ToLower() &&
                    e.IdEquipo != (_idEquipo ?? 0));

                if (nombreRepetido)
                {
                    MessageBox.Show("Ya existe un equipo con ese nombre en este proyecto.");
                    return;
                }

                if (_equipoEditando == null)
                {
                    var nuevoEquipo = new Equipo
                    {
                        IdProyecto = _idProyecto,
                        IdSupervisor = SesionActual.IdUsuario,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Activo = 1,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Equipos.Add(nuevoEquipo);
                    _context.SaveChanges();

                    foreach (var miembro in _miembros)
                    {
                        _context.EquipoUsuarios.Add(new EquipoUsuario
                        {
                            IdEquipo = nuevoEquipo.IdEquipo,
                            IdUsuario = miembro.IdUsuario,
                            FechaAsignacion = DateTime.Now,
                            Activo = 1
                        });
                    }

                    _context.SaveChanges();
                    MessageBox.Show("Equipo creado correctamente.");
                }
                else
                {
                    _equipoEditando.Nombre = nombre;
                    _equipoEditando.Descripcion = descripcion;

                    _context.SaveChanges();

                    var relacionesActuales = _context.EquipoUsuarios
                        .Where(x => x.IdEquipo == _equipoEditando.IdEquipo)
                        .ToList();

                    _context.EquipoUsuarios.RemoveRange(relacionesActuales);
                    _context.SaveChanges();

                    foreach (var miembro in _miembros)
                    {
                        _context.EquipoUsuarios.Add(new EquipoUsuario
                        {
                            IdEquipo = _equipoEditando.IdEquipo,
                            IdUsuario = miembro.IdUsuario,
                            FechaAsignacion = DateTime.Now,
                            Activo = 1
                        });
                    }

                    _context.SaveChanges();
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

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

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

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private class MiembroTemp
        {
            public int IdUsuario { get; set; }
            public string NombreCompleto { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
        }
    }
}