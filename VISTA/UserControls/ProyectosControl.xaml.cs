using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VISTA.UserControls
{
    public partial class ProyectosControl : UserControl
    {
        private readonly ProyectoService _proyectoService = new();
        private readonly EmpresaService _empresaService = new();

        private List<ProyectoDto> _todosLosProyectos = new();
        private int _idEmpresaActual;

        // Evento para que MainWindow navegue al proyecto seleccionado
        public event EventHandler<ProyectoDto> GestionarProyectoRequested;

        public ProyectosControl()
        {
            InitializeComponent();
            Loaded += ProyectosControl_Loaded;
        }

        private void ProyectosControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarProyectos();
        }

        public void CargarProyectos()
        {
            try
            {
                var empresa = _empresaService.ObtenerEmpresa();
                if (empresa == null)
                {
                    txtSubtitulo.Text = "No hay empresa registrada.";
                    return;
                }
                _idEmpresaActual = empresa.IdEmpresa;
                txtSubtitulo.Text = $"Empresa: {empresa.Nombre}";

                _todosLosProyectos = _proyectoService.ObtenerProyectosPorEmpresa(_idEmpresaActual);
                ActualizarEstadisticas();
                RenderizarCards(_todosLosProyectos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando proyectos:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarEstadisticas()
        {
            txtActivos.Text    = _todosLosProyectos.Count(p => p.Estado.Equals("Activo", StringComparison.OrdinalIgnoreCase)).ToString();
            txtPausados.Text   = _todosLosProyectos.Count(p => p.Estado.Equals("Pausado", StringComparison.OrdinalIgnoreCase)).ToString();
            txtFinalizados.Text = _todosLosProyectos.Count(p => p.Estado.Equals("Finalizado", StringComparison.OrdinalIgnoreCase)).ToString();
            txtTotal.Text      = _todosLosProyectos.Count.ToString();
        }

        private void RenderizarCards(List<ProyectoDto> proyectos)
        {
            panelCards.Children.Clear();

            if (proyectos.Count == 0)
            {
                var msg = new TextBlock
                {
                    Text = "No se encontraron proyectos.",
                    Foreground = Brushes.Gray,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                panelCards.Children.Add(msg);
                return;
            }

            foreach (var p in proyectos)
                panelCards.Children.Add(CrearCard(p));
        }

        private Border CrearCard(ProyectoDto p)
        {
            // Color badge de estado
            var (badgeBg, badgeFg, estadoTexto) = p.Estado.ToLower() switch
            {
                "activo"     => ("#DCFCE7", "#166534", "● Activo"),
                "pausado"    => ("#FEF9C3", "#854D0E", "⏸ Pausado"),
                "finalizado" => ("#E5E7EB", "#374151", "✓ Finalizado"),
                _            => ("#EFF6FF", "#1D4ED8", p.Estado)
            };

            // Card container
            var card = new Border
            {
                Width = 280,
                Margin = new Thickness(0, 0, 16, 16),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDE1E9")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Cursor = Cursors.Hand,
                Tag = p
            };

            // Click en la card = Gestionar
            card.MouseLeftButtonUp += (s, e) =>
            {
                if (e.OriginalSource is Button) return; // no propagar si fue un botón
                GestionarProyectoRequested?.Invoke(this, p);
            };

            var stack = new StackPanel { Margin = new Thickness(16) };

            // Fila superior: nombre + badge estado
            var rowTop = new Grid();
            rowTop.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rowTop.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var txtNombre = new TextBlock
            {
                Text = p.Nombre,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111827")),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 8, 0)
            };
            Grid.SetColumn(txtNombre, 0);

            var badge = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(badgeBg)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 3, 8, 3),
                VerticalAlignment = VerticalAlignment.Top
            };
            badge.Child = new TextBlock
            {
                Text = estadoTexto,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(badgeFg))
            };
            Grid.SetColumn(badge, 1);

            rowTop.Children.Add(txtNombre);
            rowTop.Children.Add(badge);
            stack.Children.Add(rowTop);

            // Descripción
            stack.Children.Add(new TextBlock
            {
                Text = p.Descripcion.Length > 60 ? p.Descripcion[..60] + "…" : p.Descripcion,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Margin = new Thickness(0, 8, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });

            // Separator
            stack.Children.Add(new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5")),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(0, 12, 0, 12)
            });

            // Supervisor
            var rowSup = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowSup.Children.Add(new TextBlock { Text = "👤 ", FontSize = 12 });
            rowSup.Children.Add(new TextBlock
            {
                Text = p.Supervisor,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            });
            stack.Children.Add(rowSup);

            // Fechas
            var rowFechas = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowFechas.Children.Add(new TextBlock { Text = "📅 ", FontSize = 12 });
            rowFechas.Children.Add(new TextBlock
            {
                Text = $"{p.FechaInicio}  →  {p.FechaFin}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            });
            stack.Children.Add(rowFechas);

            // Progreso
            stack.Children.Add(new TextBlock
            {
                Text = $"Progreso: {p.Progreso}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                Margin = new Thickness(0, 0, 0, 12)
            });

            // Botones de acción (editar / eliminar)
            var rowBtns = new Grid();
            rowBtns.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rowBtns.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var btnEditar = new Button
            {
                Content = "✏",
                ToolTip = "Editar proyecto",
                Width = 30, Height = 30,
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDE1E9")),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 0, 6, 0),
                Tag = p
            };
            btnEditar.Click += BtnEditarCard_Click;

            var btnEliminar = new Button
            {
                Content = "🗑",
                ToolTip = "Eliminar proyecto",
                Width = 30, Height = 30,
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FECACA")),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Tag = p
            };
            btnEliminar.Click += BtnEliminarCard_Click;

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal };
            btnPanel.Children.Add(btnEditar);
            btnPanel.Children.Add(btnEliminar);
            Grid.SetColumn(btnPanel, 1);
            rowBtns.Children.Add(btnPanel);
            stack.Children.Add(rowBtns);

            card.Child = stack;
            return card;
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(txt))
                RenderizarCards(_todosLosProyectos);
            else
                RenderizarCards(_todosLosProyectos
                    .Where(p => p.Nombre.ToLower().Contains(txt) ||
                                p.Supervisor.ToLower().Contains(txt) ||
                                p.Estado.ToLower().Contains(txt))
                    .ToList());
        }

        private void BtnNuevoProyecto_Click(object sender, RoutedEventArgs e)
        {
            if (_idEmpresaActual <= 0)
            {
                MessageBox.Show("No se encontró una empresa válida.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var ventana = new ProyectoFormWindow(_idEmpresaActual, null) { Owner = Window.GetWindow(this) };
            if (ventana.ShowDialog() == true)
                CargarProyectos();
        }

        private void BtnEditarCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ProyectoDto p)
            {
                var ventana = new ProyectoFormWindow(_idEmpresaActual, p.IdProyecto) { Owner = Window.GetWindow(this) };
                if (ventana.ShowDialog() == true)
                    CargarProyectos();
            }
        }

        private void BtnEliminarCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ProyectoDto p)
            {
                var res = MessageBox.Show($"¿Eliminar el proyecto \"{p.Nombre}\"?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;

                try
                {
                    var svc = new ProyectoService();
                    svc.EliminarProyecto(p.IdProyecto);
                    CargarProyectos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo eliminar:\n" + ex.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
