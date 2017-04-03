using ImageSharp360Viewer.Commands;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageSharp360Viewer.ViewModel {

    enum OptionImage {

        OpenImage360,
        ImportImage360,
        ImportWatermark

    }

    /// <summary>
    /// Main ViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase {

        private MetroWindow _metroWindow;

        // Commands
        #region commands

        /// <summary>
        /// Abrir imagen 360 con FileDialog
        /// </summary>
        public ICommand OpenImage360 { get; private set; }

        /// <summary>
        /// Importar imagen 360 con FileDialog
        /// </summary>
        public ICommand ImportImage360 { get; private set; }

        /// <summary>
        /// Importar marca de agua con FileDialog
        /// </summary>
        public ICommand ImportWatermark { get; private set; }

        /// <summary>
        /// Guarda marca de agua con FileDialog
        /// </summary>
        public ICommand SaveImage360WithWatermark { get; private set; }

        /// <summary>
        /// Se procesa la imagen para ser marcada
        /// </summary>
        public ICommand AddWatermark { get; private set; }

        /// <summary>
        /// Salir de la aplicación
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        #endregion

        // Propiedades Publicas
        #region PropiedadesPublicas

        /// <summary>
        /// Imagen 360° renderizada
        /// </summary>
        public BitmapImage Image360Rendered { get; private set; }

        /// <summary>
        /// Imagen 360° visualizada en landscape
        /// </summary>
        public BitmapImage Image360 { get; private set; }

        /// <summary>
        /// Imagen marca de agua visualizada en landscape
        /// </summary>
        public BitmapImage WatermarkImage { get; private set; }

        /// <summary>
        /// Imagen 360° con la marca de agua aplicada visualizada en landscape
        /// </summary>
        public BitmapImage Image360WithWatermark { get; private set; }

        /// <summary>
        /// Estado del visualizador de 360° al cargar la imagen
        /// </summary>
        public bool IsLoading { get; private set; }

        #endregion

        // Propiedades Privadas
        #region PropiedadesPrivadas

        /// <summary>
        /// Path de la imagen 360
        /// </summary>
        private string _uriImage360;

        /// <summary>
        /// Path de la marca de agua
        /// </summary>
        private string _uriWatermarkImage;

        /// <summary>
        /// Path de la imagen 360 con marca de agua
        /// </summary>
        private string _uriImage360WithWatermark;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel(MetroWindow metroWindow) {

            _metroWindow = metroWindow;

            // Commands
            OpenImage360 = new RelayCommand(a => OpenImage(OptionImage.OpenImage360, "Abrir imagen 360°"));
            ImportImage360 = new RelayCommand(a => OpenImage(OptionImage.ImportImage360, "Importar imagen 360°"));
            ImportWatermark = new RelayCommand(a => OpenImage(OptionImage.ImportWatermark, "Importar marca de agua"));
            AddWatermark = new RelayCommand(a => InsertWatermark());
            ExitCommand = new RelayCommand(a => Exit());

            // Dependency
            Image360Rendered = null;
            RaisePropertyChanged(nameof(Image360Rendered));

            IsLoading = false;
            RaisePropertyChanged(nameof(IsLoading));

        }

        // Metodos
        #region Metodos

        // Abrir imagen
        private async void OpenImage(OptionImage optionImage, string title) {

            // Creamos el FileDialog
            var fileDialog = new OpenFileDialog() {
                Title = title,
                Filter = "Image (*.jpg; *.jpeg; *.bmp; *.png)|*.jpg; *.jpeg; *.bmp; *.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Multiselect = false
            };

            // Se muestra y se valida la respuesta
            if (fileDialog.ShowDialog() == DialogResult.OK) {

                // Se obtiene la ruta y el nombre del archivo
                var fileName = fileDialog.FileName;

                switch (optionImage) {

                    case OptionImage.OpenImage360:
                        await Render360Image(fileName);
                        break;

                    case OptionImage.ImportImage360:
                        _uriImage360 = fileName;
                        await Display360Image(fileName);
                        break;

                    case OptionImage.ImportWatermark:
                        _uriWatermarkImage = fileName;
                        await DisplayWaterMark(fileName);
                        break;

                }

            }

        }

        // Se carga la imagen a renderizar
        public async Task Render360Image(string path) {

            Image360Rendered = null;
            RaisePropertyChanged(nameof(Image360Rendered));

            IsLoading = true;
            RaisePropertyChanged(nameof(IsLoading));

            await Task.Factory.StartNew(() => {

                Image360Rendered = new BitmapImage(new Uri(path));
                Image360Rendered.Freeze();

            });

            if (Math.Abs(Image360Rendered.Width / Image360Rendered.Height - 2) > 0.001)
                await DialogManager.ShowMessageAsync(_metroWindow, "Advertencia", "!La imagen no es equirectangular (2:1)!\nEl renderizado puede no ser el apropiado.", settings: new MetroDialogSettings() {
                    ColorScheme = MetroDialogColorScheme.Accented
                });

            IsLoading = false;
            RaisePropertyChanged(nameof(IsLoading));
            RaisePropertyChanged(nameof(Image360Rendered));

            var flyout = _metroWindow.Flyouts.Items[0] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;

        }

        /// <summary>
        /// Se muestra la imagen 360° en landscape
        /// </summary>
        /// <param name="path"></param>
        public async Task Display360Image(string path) {

            Image360 = null;
            RaisePropertyChanged(nameof(Image360));

            await Task.Factory.StartNew(() => {

                Image360 = new BitmapImage(new Uri(path));
                Image360.Freeze();

                RaisePropertyChanged(nameof(Image360));

            });

            if (Math.Abs(Image360.Width / Image360.Height - 2) > 0.001)
                await DialogManager.ShowMessageAsync(_metroWindow, "Advertencia", "!La imagen no es equirectangular (2:1)!\nEl proceso de marcado puede no ser el apropiado.", settings: new MetroDialogSettings() {
                    ColorScheme = MetroDialogColorScheme.Accented
                });

            var flyout = _metroWindow.Flyouts.Items[1] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;

        }

        /// <summary>
        /// Se muestra la marca de agua en landscape
        /// </summary>
        /// <param name="path"></param>
        private async Task DisplayWaterMark(string path) {

            WatermarkImage = null;
            RaisePropertyChanged(nameof(WatermarkImage));

            await Task.Factory.StartNew(() => {

                WatermarkImage = new BitmapImage(new Uri(path));
                WatermarkImage.Freeze();

                RaisePropertyChanged(nameof(WatermarkImage));

            });

            var flyout = _metroWindow.Flyouts.Items[2] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;

        }

        private async void InsertWatermark() {

            if (Image360 == null || WatermarkImage == null) {

                await DialogManager.ShowMessageAsync(_metroWindow, "Advertencia", "Se requiere de la imagen 360° y de la marca de agua para poder realizar esta función.", settings: new MetroDialogSettings() {
                    ColorScheme = MetroDialogColorScheme.Accented
                });

                return;

            }

            var controller = await _metroWindow.ShowProgressAsync("Por favor espere", "Se está aplicando la marca de agua.", settings: new MetroDialogSettings() {
                ColorScheme = MetroDialogColorScheme.Accented
            });
            controller.SetIndeterminate();

            await Task.Factory.StartNew(() => {

                //var image360 = (Bitmap) Image.FromFile(_uriImage360, true);
                //var watermark = (Bitmap) Image.FromFile(_uriWatermarkImage, true);

                //var image360WithWatermark = image360.AddWatermark(watermark);
                //_uriImage360WithWatermark = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Marked.jpg";
                //image360WithWatermark.Save(_uriImage360WithWatermark, ImageFormat.Jpeg);

            });

            await controller.CloseAsync();

            Image360WithWatermark = new BitmapImage(new Uri(_uriImage360WithWatermark));
            RaisePropertyChanged(nameof(Image360WithWatermark));

        }

        // Salir de la aplicación
        private void Exit() {

            System.Windows.Application.Current.Shutdown();

        }

        #endregion

    }
}
