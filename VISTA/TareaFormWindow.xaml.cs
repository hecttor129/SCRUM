using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VISTA
{
    public partial class TareaFormWindow : Window
    {
        private readonly TareaService _tareaService = new();
        private readonly int? _idEmpresa;
        private readonly int? _idProyecto;
        private readonly int? _idEquipo;
        private readonly int? _idTarea;

        private Tarea? _tareaEditando;
        private List<TareaDto> _todasLasTareasScope = new();
        private List<TareaDto> _dependenciasSeleccionadas = new();

        public TareaFormWindow(int? idEmpresa, int? idProyecto, int? idEquipo, int? idTarea = null)
        {
            InitializeComponent();
            _idEmpresa = idEmpresa;
            _idProyecto = idProyecto;
            _idEquipo = idEquipo;
            _idTarea = idTarea;

            Loaded += TareaFormWindow_Loaded;
        }

        private void TareaFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cbPrioridad.SelectedIndex = 2; // Por defecto: Media
            cbEstado.SelectedIndex = 0;    // Por defecto: Pendiente

            CargarTareasScope();

            if (_idTarea.HasValue)
            {
                CargarTareaExistente(_idTarea.Value);
            }
            else
            {
                chkDisponible.IsChecked = true; // Sin dependencias inicialmente disponible
            }

            RefrescarDependenciasVisuales();
        }

        private void CargarTareasScope()
        {
            try
            {
                if (_idEquipo.HasValue)
                    _todasLasTareasScope = _tareaService.ObtenerTareasPorEquipo(_idEquipo.Value);
                else if (_idProyecto.HasValue)
                    _todasLasTareasScope = _tareaService.ObtenerTareasPorProyecto(_idProyecto.Value);
                else if (_idEmpresa.HasValue)
                    _todasLasTareasScope = _tareaService.ObtenerTareasPorEmpresa(_idEmpresa.Value);

                // Filtrar para excluir la tarea actual (si se está editando)
                var comboItems = _todasLasTareasScope
                    .Where(t => !_idTarea.HasValue || t.IdTarea != _idTarea.Value)
                    .ToList();

                cbDependencia.ItemsSource = comboItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las tareas del entorno:\n" + ex.Message);
            }
        }

        private void CargarTareaExistente(int idTarea)
        {
            try
            {
                // Buscar la tarea editando en el listado del scope
                var dtos = _todasLasTareasScope.FirstOrDefault(t => t.IdTarea == idTarea);
                if (dtos == null)
                {
                    MessageBox.Show("No se encontró la tarea seleccionada.");
                    Close();
                    return;
                }

                txtTituloVentana.Text = "Editar Tarea";
                txtSubtituloVentana.Text = "Modifica los atributos o gestiona las dependencias de esta tarea.";

                // Para obtener la entidad completa, usaremos un truco simple o lo instanciamos del repo indirectamente
                // Pero como TareaService solo tiene Crear, Editar y Eliminar, y obtener por ID no está directo en el Service,
                // vamos a buscar directamente de la base de datos usando el DB_Context en el codebehind para evitar crear
                // servicios innecesarios, o instanciamos un TareaRepository temporalmente.
                var repo = new DAL.TareaRepository();
                _tareaEditando = repo.GetById(idTarea);

                if (_tareaEditando == null)
                {
                    MessageBox.Show("No se pudo obtener el detalle completo de la tarea.");
                    Close();
                    return;
                }

                txtTitulo.Text = _tareaEditando.Titulo;
                txtDescripcion.Text = _tareaEditando.Descripcion ?? "";
                txtEspecializacion.Text = _tareaEditando.EspecializacionRequerida ?? "";

                // Prioridad
                if (_tareaEditando.Prioridad.HasValue && _tareaEditando.Prioridad.Value >= 1 && _tareaEditando.Prioridad.Value <= 5)
                {
                    cbPrioridad.SelectedIndex = _tareaEditando.Prioridad.Value - 1;
                }

                // Estado
                foreach (ComboBoxItem item in cbEstado.Items)
                {
                    if (item.Content.ToString() == _tareaEditando.estadoTarea.ToString())
                    {
                        cbEstado.SelectedItem = item;
                        break;
                    }
                }

                dpFechaInicio.SelectedDate = _tareaEditando.FechaInicio;
                dpFechaLimite.SelectedDate = _tareaEditando.FechaLimite;
                chkDisponible.IsChecked = _tareaEditando.Disponible;

                // Cargar dependencias
                if (_tareaEditando.Dependencias != null && _tareaEditando.Dependencias.Count > 0)
                {
                    _dependenciasSeleccionadas = _todasLasTareasScope
                        .Where(t => _tareaEditando.Dependencias.Contains(t.IdTarea))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando tarea:\n" + ex.Message);
            }
        }

        private void RefrescarDependenciasVisuales()
        {
            lstDependencias.ItemsSource = null;
            lstDependencias.ItemsSource = _dependenciasSeleccionadas;

            // Reevaluar disponibilidad visualmente en caliente si cambiaron las dependencias
            ReevaluarDisponibilidadVisualEnCaliente();
        }

        private void ReevaluarDisponibilidadVisualEnCaliente()
        {
            // Si el estado de la tarea en pantalla es Completada, siempre es true o mantiene su estado.
            // Pero si hay dependencias no completadas en pantalla, entonces Disponible = false
            bool disp = true;
            foreach (var dep in _dependenciasSeleccionadas)
            {
                if (dep.Estado != "Completada")
                {
                    disp = false;
                    break;
                }
            }
            chkDisponible.IsChecked = disp;
        }

        private void BtnAgregarDep_Click(object sender, RoutedEventArgs e)
        {
            if (cbDependencia.SelectedItem is not TareaDto seleccionada)
            {
                MessageBox.Show("Selecciona una tarea válida de la lista.");
                return;
            }

            if (_dependenciasSeleccionadas.Any(d => d.IdTarea == seleccionada.IdTarea))
            {
                MessageBox.Show("Esta dependencia ya ha sido agregada.");
                return;
            }

            _dependenciasSeleccionadas.Add(seleccionada);
            cbDependencia.SelectedItem = null;
            RefrescarDependenciasVisuales();
        }

        private void BtnQuitarDep_Click(object sender, RoutedEventArgs e)
        {
            if (lstDependencias.SelectedItem is not TareaDto seleccionada)
            {
                MessageBox.Show("Selecciona una dependencia de la lista de abajo para quitarla.");
                return;
            }

            _dependenciasSeleccionadas.Remove(seleccionada);
            RefrescarDependenciasVisuales();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string titulo = txtTitulo.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();
                string especializacion = txtEspecializacion.Text.Trim();

                if (string.IsNullOrWhiteSpace(titulo))
                {
                    MessageBox.Show("El título es obligatorio.");
                    return;
                }

                if (cbEstado.SelectedItem is not ComboBoxItem estadoItem)
                {
                    MessageBox.Show("Selecciona un estado.");
                    return;
                }

                ENTITY.ENUMS.EstadoTarea estado = (ENTITY.ENUMS.EstadoTarea)Enum.Parse(typeof(ENTITY.ENUMS.EstadoTarea), estadoItem.Content.ToString()!);
                int prioridad = cbPrioridad.SelectedIndex + 1;

                var dependenciasIds = _dependenciasSeleccionadas.Select(d => d.IdTarea).ToList();

                if (_tareaEditando == null)
                {
                    var nueva = new Tarea
                    {
                        Titulo = titulo,
                        Descripcion = descripcion,
                        EspecializacionRequerida = especializacion,
                        Prioridad = prioridad,
                        estadoTarea = estado,
                        FechaInicio = dpFechaInicio.SelectedDate,
                        FechaLimite = dpFechaLimite.SelectedDate,
                        IdEmpresa = _idEmpresa,
                        IdProyecto = _idProyecto,
                        IdEquipo = _idEquipo,
                        Dependencias = dependenciasIds
                    };

                    _tareaService.CrearTarea(nueva);
                    MessageBox.Show("Tarea creada correctamente.");
                }
                else
                {
                    _tareaEditando.Titulo = titulo;
                    _tareaEditando.Descripcion = descripcion;
                    _tareaEditando.EspecializacionRequerida = especializacion;
                    _tareaEditando.Prioridad = prioridad;
                    _tareaEditando.estadoTarea = estado;
                    _tareaEditando.FechaInicio = dpFechaInicio.SelectedDate;
                    _tareaEditando.FechaLimite = dpFechaLimite.SelectedDate;
                    _tareaEditando.Dependencias = dependenciasIds;

                    _tareaService.EditarTarea(_tareaEditando);
                    MessageBox.Show("Tarea actualizada correctamente.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la tarea:\n" + ex.Message);
            }
        }
    }
}
