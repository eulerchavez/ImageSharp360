using ImageSharp360.Imaging;
using ImageSharp360.Watermaking;
using ImageSharp360.Watermaking.Algorithm;
using ImageSharp360Viewer.Commands;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        /// <summary>
        /// Factor utilizado para el proceso de marcado
        /// </summary>
        public double Factor { get; set; } = 0.5;

        private Bitmap360 _Result;

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

                //Image360Rendered = new BitmapImage(new Uri(path));
                Image360Rendered = new BitmapImage();

                Image360Rendered.BeginInit();
                Image360Rendered.StreamSource = new MemoryStream(File.ReadAllBytes(path));
                Image360Rendered.EndInit();

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

                Image360 = new BitmapImage();

                Image360.BeginInit();
                Image360.StreamSource = new MemoryStream(File.ReadAllBytes(path));
                Image360.EndInit();

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

                WatermarkImage = new BitmapImage();

                WatermarkImage.BeginInit();
                WatermarkImage.StreamSource = new MemoryStream(File.ReadAllBytes(path));
                WatermarkImage.EndInit();


                WatermarkImage.Freeze();

                RaisePropertyChanged(nameof(WatermarkImage));

            });

            var flyout = _metroWindow.Flyouts.Items[2] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;

        }

        private async void InsertWatermark() {

            if (Image360 == null || WatermarkImage == null) {

                await DialogManager.ShowMessageAsync(_metroWindow, "Advertencia", "Se requiere de la imagen 360° y de la marca de agua para poder realizar este proceso.", settings: new MetroDialogSettings() {
                    ColorScheme = MetroDialogColorScheme.Accented
                });

                return;

            }

            var controller = await _metroWindow.ShowProgressAsync("Por favor espere", "Se está aplicando la marca de agua.", settings: new MetroDialogSettings() {
                ColorScheme = MetroDialogColorScheme.Accented
            });
            controller.SetIndeterminate();

            await Task.Factory.StartNew(() => {

                var image360 = new Bitmap360(_uriImage360);
                var watermark = new WatermarkBitmap(_uriWatermarkImage);

                Watermarking proceso = new Watermarking(image360, watermark, new Factores(Factor),
                TissotIndicatrix.TopIndicatrix,
                TissotIndicatrix.BottomIndicatrix,
                TissotIndicatrix.FirstIndicatrix,
                TissotIndicatrix.SecondIndicatrix,
                TissotIndicatrix.ThirdIndicatrix,
                TissotIndicatrix.FourthIndicatrix,
                TissotIndicatrix.FifthIndicatrix,
                TissotIndicatrix.SixthIndicatrix,
                TissotIndicatrix.SeventhIndicatrix,
                TissotIndicatrix.EighthIndicatrix,
                TissotIndicatrix.NinthIndicatrix,
                TissotIndicatrix.TenthIndicatrix,
                TissotIndicatrix.EleventhIndicatrix,
                TissotIndicatrix.TwelfthIndicatrix
                );

                proceso.Prepare();

                _Result = proceso.Apply();

            });

            await controller.CloseAsync();

            // Se guarda

            // Creamos el FileDialog
            var fileDialog = new SaveFileDialog() {
                Title = "Guardar imagen 360° marcada",
                Filter = "Imagen 360° (*.jpg; *.jpeg)|*.jpg; *.jpeg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            };

            if (fileDialog.ShowDialog() == DialogResult.OK) {

                using (FileStream fs = (FileStream) fileDialog.OpenFile()) {

                    _Result.Save(fs, ImageFormat.Jpeg);

                }

                _uriImage360WithWatermark = fileDialog.FileName;

                Image360WithWatermark = new BitmapImage();

                Image360WithWatermark.BeginInit();
                Image360WithWatermark.StreamSource = new MemoryStream(File.ReadAllBytes(fileDialog.FileName));
                Image360WithWatermark.EndInit();

                RaisePropertyChanged(nameof(Image360WithWatermark));

            }



        }

        // Salir de la aplicación
        private void Exit() {

            System.Windows.Application.Current.Shutdown();

        }

        #endregion

    }

}
