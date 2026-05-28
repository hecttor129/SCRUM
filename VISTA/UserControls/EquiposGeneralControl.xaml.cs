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
    public partial class EquiposGeneralControl : UserControl
    {
        private readonly EquipoService _equipoService = new();
        private readonly EmpresaService _empresaService = new();

        private List<EquipoDto> _todosLosEquipos = new();
        private int _idEmpresaActual;

        // Evento: navegar a DashboardEquipoControl desde MainWindow
        public event EventHandler<EquipoDto> GestionarEquipoRequested;

        public EquiposGeneralControl()
        {
            InitializeComponent();
            Loaded += EquiposGeneralControl_Loaded;
        }

        private void EquiposGeneralControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarEquipos();
        }

        public void CargarEquipos()
        {
            try
            {
                var empresa = _empresaService.ObtenerEmpresa();
                if (empresa == null) return;
                _idEmpresaActual = empresa.IdEmpresa;

                _todosLosEquipos = _equipoService.ObtenerEquiposPorEmpresa(_idEmpresaActual);
                RenderizarCards(_todosLosEquipos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando equipos:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenderizarCards(List<EquipoDto> equipos)
        {
            panelCards.Children.Clear();

            if (equipos.Count == 0)
            {
                panelCards.Children.Add(new TextBlock
                {
                    Text = "No hay equipos registrados en la empresa.",
                    Foreground = Brushes.Gray,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            foreach (var eq in equipos)
                panelCards.Children.Add(CrearCard(eq));
        }

        private Border CrearCard(EquipoDto eq)
        {
            var card = new Border
            {
                Width = 260,
                Margin = new Thickness(0, 0, 16, 16),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDE1E9")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Cursor = Cursors.Hand,
                Tag = eq
            };

            card.MouseLeftButtonUp += (s, e) => GestionarEquipoRequested?.Invoke(this, eq);

            var stack = new StackPanel { Margin = new Thickness(16) };

            // Ícono + Nombre del equipo
            var rowNombre = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            rowNombre.Children.Add(new TextBlock { Text = "👥 ", FontSize = 16, VerticalAlignment = VerticalAlignment.Center });
            rowNombre.Children.Add(new TextBlock
            {
                Text = eq.Nombre,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111827")),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            });
            stack.Children.Add(rowNombre);

            // Proyecto al que pertenece
            var proyectoBadge = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EFF6FF")),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 3, 8, 3),
                Margin = new Thickness(0, 4, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            proyectoBadge.Child = new TextBlock
            {
                Text = $"📁 {eq.NombreProyecto}",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D4ED8")),
                FontWeight = FontWeights.SemiBold
            };
            stack.Children.Add(proyectoBadge);

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
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            });
            stack.Children.Add(rowSup);

            // Miembros
            var rowMiembros = new StackPanel { Orientation = Orientation.Horizontal };
            rowMiembros.Children.Add(new TextBlock { Text = "🧑‍💼 ", FontSize = 12 });
            rowMiembros.Children.Add(new TextBlock
            {
                Text = $"{eq.Trabajadores} miembro{(eq.Trabajadores != 1 ? "s" : "")}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            });
            stack.Children.Add(rowMiembros);

            card.Child = stack;
            return card;
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(txt))
                RenderizarCards(_todosLosEquipos);
            else
                RenderizarCards(_todosLosEquipos
                    .Where(eq => eq.Nombre.ToLower().Contains(txt) ||
                                 eq.NombreProyecto.ToLower().Contains(txt) ||
                                 eq.Supervisor.ToLower().Contains(txt))
                    .ToList());
        }
    }
}
