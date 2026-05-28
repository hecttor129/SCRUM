using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VISTA.UserControls
{
    public partial class DashboardProyectoControl : UserControl
    {
        private readonly EquipoService _equipoService = new();
        private readonly ProyectoService _proyectoService = new();
        private readonly PermisosService _permisosService = new();
        private readonly ArchivoService _archivoService = new();

        public int IdProyecto { get; private set; }
        private int _idEmpresaActual;
        private List<EquipoDto> _equipos = new();
        private EquipoDto _equipoSeleccionado;

        public event EventHandler VolverEmpresaRequested;
        public event EventHandler<EquipoDto> GestionarEquipoRequested;

        public DashboardProyectoControl(int idProyecto, int idEmpresaActual)
        {
            InitializeComponent();
            IdProyecto = idProyecto;
            _idEmpresaActual = idEmpresaActual;
            Loaded += DashboardProyectoControl_Loaded;
        }

        private void DashboardProyectoControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarEquipos();
        }

        private void CargarEquipos()
        {
            try
            {
                var proyecto = _proyectoService.ObtenerPorId(IdProyecto, _idEmpresaActual);
                if (proyecto == null)
                {
                    MessageBox.Show("No se encontró el proyecto.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                txtTituloEquipos.Text = $"Equipos de {proyecto.Nombre}";
                _equipos = _equipoService.ObtenerEquiposPorProyecto(IdProyecto);
                _equipoSeleccionado = null;
                RenderizarCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando equipos:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenderizarCards()
        {
            panelEquipoCards.Children.Clear();

            if (_equipos.Count == 0)
            {
                panelEquipoCards.Children.Add(new TextBlock
                {
                    Text = "No hay equipos en este proyecto. Crea el primero con \"+ Nuevo equipo\".",
                    Foreground = Brushes.Gray,
                    FontSize = 13,
                    Margin = new Thickness(0, 16, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                });
                return;
            }

            foreach (var eq in _equipos)
                panelEquipoCards.Children.Add(CrearCardEquipo(eq));
        }

        private Border CrearCardEquipo(EquipoDto eq)
        {
            var card = new Border
            {
                Width = 240,
                Margin = new Thickness(0, 0, 16, 16),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDE1E9")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Cursor = Cursors.Hand,
                Tag = eq
            };

            card.MouseLeftButtonUp += (s, e) =>
            {
                if (e.OriginalSource is Button) return; // no propagar si fue un botón
                _equipoSeleccionado = eq;
                GestionarEquipoRequested?.Invoke(this, eq);
            };

            var stack = new StackPanel { Margin = new Thickness(16) };

            // Nombre del equipo
            stack.Children.Add(new TextBlock
            {
                Text = "👥 " + eq.Nombre,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111827")),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 8)
            });

            // Separator
            stack.Children.Add(new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5")),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Supervisor
            var rowSup = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowSup.Children.Add(new TextBlock { Text = "👤 ", FontSize = 12 });
            rowSup.Children.Add(new TextBlock
            {
                Text = eq.Supervisor,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                TextWrapping = TextWrapping.Wrap
            });
            stack.Children.Add(rowSup);

            // Miembros
            var rowMiembros = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
            rowMiembros.Children.Add(new TextBlock { Text = "🧑‍💼 ", FontSize = 12 });
            rowMiembros.Children.Add(new TextBlock
            {
                Text = $"{eq.Trabajadores} miembro{(eq.Trabajadores != 1 ? "s" : "")}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            });
            stack.Children.Add(rowMiembros);

            // Botones de acción (editar / eliminar)
            var rowBtns = new Grid();
            rowBtns.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rowBtns.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var btnEditar = new Button
            {
                Content = "✏",
                ToolTip = "Editar equipo",
                Width = 28, Height = 28,
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDE1E9")),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 0, 6, 0),
                Tag = eq
            };
            btnEditar.Click += (s, ev) =>
            {
                _equipoSeleccionado = eq;
                BtnEditarEquipo_Click(s, ev);
            };

            var btnEliminar = new Button
            {
                Content = "🗑",
                ToolTip = "Eliminar equipo",
                Width = 28, Height = 28,
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FECACA")),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Tag = eq
            };
            btnEliminar.Click += (s, ev) =>
            {
                _equipoSeleccionado = eq;
                BtnEliminarEquipo_Click(s, ev);
            };

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal };
            btnPanel.Children.Add(btnEditar);
            btnPanel.Children.Add(btnEliminar);
            Grid.SetColumn(btnPanel, 1);
            rowBtns.Children.Add(btnPanel);
            stack.Children.Add(rowBtns);

            card.Child = stack;
            return card;
        }

        // ── Acciones de equipo ───────────────────────────────────────────────

        private void BtnVolverEmpresa_Click(object sender, RoutedEventArgs e)
            => VolverEmpresaRequested?.Invoke(this, EventArgs.Empty);

        private void BtnNuevoEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para crear equipos en este proyecto.",
                    "Permisos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var ventana = new EquipoFormWindow(IdProyecto) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarEquipos();
        }

        private void BtnEditarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (_equipoSeleccionado == null)
            {
                MessageBox.Show("Por favor, selecciona un equipo para editar.",
                    "Selección", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para editar equipos.", "Permisos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var ventana = new EquipoFormWindow(IdProyecto, _equipoSeleccionado.IdEquipo) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarEquipos();
        }

        private void BtnEliminarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (_equipoSeleccionado == null)
            {
                MessageBox.Show("Por favor, selecciona un equipo para eliminar.",
                    "Selección", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (!_permisosService.PuedeGestionarEquipos(IdProyecto))
            {
                MessageBox.Show("No tienes permisos para eliminar equipos.", "Permisos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = MessageBox.Show($"¿Eliminar el equipo «{_equipoSeleccionado.Nombre}»?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res != MessageBoxResult.Yes) return;

            try
            {
                _equipoService.EliminarEquipo(_equipoSeleccionado.IdEquipo);
                CargarEquipos();
                MessageBox.Show("Equipo eliminado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando equipo:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Botón Gestionar dentro de la card (si se usa en el futuro con botón explícito)
        private void BtnGestionarEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EquipoDto eq)
                GestionarEquipoRequested?.Invoke(this, eq);
        }

        // ── Archivos Generales del Proyecto ─────────────────────────────────

        private void TabControlProyecto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || e.AddedItems[0] is not TabItem)
            {
                e.Handled = true;
                return;
            }
            e.Handled = true;
            if (tabItemArchivosGenerales != null && tabItemArchivosGenerales.IsSelected)
                CargarArchivosProyecto();
        }

        private void CargarArchivosProyecto()
        {
            try
            {
                dgArchivosProyecto.ItemsSource =
                    _archivoService.ObtenerArchivosPorProyectoYEquipos(IdProyecto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar archivos:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSubirArchivoProyecto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Seleccionar archivo para el proyecto",
                    Filter = "Todos los archivos (*.*)|*.*|PDF (*.pdf)|*.pdf|Word (*.docx;*.doc)|*.docx;*.doc|Excel (*.xlsx;*.xls)|*.xlsx;*.xls"
                };
                if (dlg.ShowDialog() == true)
                {
                    _archivoService.SubirArchivoProyecto(dlg.FileName, IdProyecto, SesionActual.IdUsuario);
                    CargarArchivosProyecto();
                    MessageBox.Show("Archivo subido correctamente.", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error subiendo archivo:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDescargarArchivoProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivosProyecto.SelectedItem is not ArchivoDto sel)
            {
                MessageBox.Show("Selecciona un archivo de la lista.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Guardar archivo como...",
                FileName = sel.NombreOriginal,
                Filter = $"(*{sel.Extension})|*{sel.Extension}|Todos (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    _archivoService.DescargarArchivo(sel.IdArchivo, dlg.FileName);
                    MessageBox.Show("Archivo descargado con éxito.", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error descargando:\n" + ex.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnEliminarArchivoProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivosProyecto.SelectedItem is not ArchivoDto sel)
            {
                MessageBox.Show("Selecciona un archivo de la lista.", "Atención",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var res = MessageBox.Show($"¿Eliminar «{sel.NombreOriginal}»?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res != MessageBoxResult.Yes) return;

            try
            {
                _archivoService.EliminarArchivo(sel.IdArchivo, SesionActual.IdUsuario,
                    SesionActual.Rol.ToString());
                CargarArchivosProyecto();
                MessageBox.Show("Archivo eliminado.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar:\n" + ex.Message, "Permiso denegado",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void DgArchivosProyecto_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgArchivosProyecto.SelectedItem is not ArchivoDto sel) return;
            try
            {
                var temp = _archivoService.ObtenerRutaTemporalParaAbrir(sel.IdArchivo);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = temp,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el archivo:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
