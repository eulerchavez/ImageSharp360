namespace ImageSharp360Viewer.View {

    using ImageSharp360Viewer.ViewModel;
    using MahApps.Metro.Controls;
    using System.Windows;

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        public MainWindow() {

            InitializeComponent();
            
            // Asignamos el ViewModel en el contexto del MainWindow
            var viewModel = new MainViewModel(this);
            DataContext = viewModel;

            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {

            this.Flyouts.Items.Add(new Flyout() {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Header = "¡Exito!",
                Content = "La imagen 360° ha sido cargada correctamente y renderizada.",
                Position = Position.Bottom,
                CloseButtonVisibility = Visibility.Visible,
                Theme = FlyoutTheme.Accent,
                IsAutoCloseEnabled = true,
                AutoCloseInterval = 5000,
                IsPinned = true
            });

            this.Flyouts.Items.Add(new Flyout() {
                Header = "¡Exito!",
                Content = "La imagen 360° ha sido importada.",
                VerticalContentAlignment = VerticalAlignment.Bottom,
                Position = Position.Bottom,
                CloseButtonVisibility = Visibility.Visible,
                Theme = FlyoutTheme.Accent,
                IsAutoCloseEnabled = true,
                AutoCloseInterval = 5000,
                IsPinned = true
            });

            this.Flyouts.Items.Add(new Flyout() {
                Header = "¡Exito!",
                Content = "La marca de agua ha sido importada.",
                VerticalContentAlignment = VerticalAlignment.Bottom,
                Position = Position.Bottom,
                CloseButtonVisibility = Visibility.Visible,
                Theme = FlyoutTheme.Accent,
                IsAutoCloseEnabled = true,
                AutoCloseInterval = 5000,
                IsPinned = true
            });

            this.Flyouts.Items.Add(new Flyout() {
                Header = "¡Exito!",
                Content = "El proceso de marcado ha finalizado.",
                VerticalContentAlignment = VerticalAlignment.Bottom,
                Position = Position.Bottom,
                CloseButtonVisibility = Visibility.Visible,
                Theme = FlyoutTheme.Accent,
                IsAutoCloseEnabled = true,
                AutoCloseInterval = 5000,
                IsPinned = true
            });

        }

    }

}
