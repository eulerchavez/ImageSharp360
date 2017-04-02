namespace ImageSharp360.Watermaking {

    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using Imaging;
    using Imaging.Transform;

    /// <summary>
    /// Regla de negocio para la inserción de la marca de agua.
    /// Funciona en conjunto con <see cref="TissotIndicatrix"/>
    /// los cuales se basan en la posicion de un <see cref="FisheyeBitmap"/>.
    /// </summary>
    public class Watermarking {

        // Imagen 360

        private Bitmap360 _image360;

        // Marca de agua

        private WatermarkBitmap _imageWatermark;

        // Fisheye de la parte superior

        private FisheyeBitmap _topFisheye;

        // Fisheye de la parte inferior

        private FisheyeBitmap _bottomFisheye;

        // Imagen que abarca toda la imagen 360, esta ya cuenta con las marcas de agua

        private WatermarkBitmap _waterMark;

        // Algoritmo de inserción

        private IWatermarkAlgorithm _algorithm;

        // Posiciones de donde se hará inserción la marca de agua

        private IEnumerable<TissotIndicatrix> _indicatrixes;

        /// <summary>
        /// Se inicializa el marcado de agua.
        /// </summary>
        /// <param name="image360">Imagen 360°</param>
        /// <param name="watermark">Marca de agua</param>
        /// <param name="algorithm">Algoritmo de inserción</param>
        /// <param name="indicatrix">Posiciones donde se realizará el marcado</param>
        public Watermarking(Bitmap360 image360, WatermarkBitmap watermark, IWatermarkAlgorithm algorithm, params TissotIndicatrix[] indicatrix) {

            if (indicatrix.Length > 0) {

                this._image360 = image360; //image360.Clone() as Bitmap360;
                this._imageWatermark = watermark; //watermark.Clone() as WatermarkBitmap;

                this._indicatrixes = indicatrix.ToList();

                this._waterMark = new WatermarkBitmap(new Bitmap(image360.Width, image360.Height, watermark.PixelFormat));

                // Se inicializan los fisheye

                this._topFisheye = new FisheyeBitmap(image360);
                this._bottomFisheye = new FisheyeBitmap(image360);

                // Se asigna el algoritmo

                this._algorithm = algorithm;

            }

        }

        /// <summary>
        /// Se realiza:
        /// <para>1. Se posiciona la marca de agua de acuerdo a los <see cref="TissotIndicatrix"/> (Top, Bottom y del 1 - 8)</para>
        /// <para>2. Se realiza la deformación <see cref="Fisheye.Convert(FisheyeBitmap)"/></para>
        /// <para>3. Se realiza la deformación <see cref="Fisheye.ToLandscape(FisheyeBitmap)"/></para>
        /// <para>4. Se posiciona la marca de agua de acuerdo a los <see cref="TissotIndicatrix"/> de la posición del centro (9 - 12)</para>
        /// </summary>
        public void Prepare() {

            foreach (var indicatrix in _indicatrixes.Where(i => i.Position != Position.Center)) {

                // Se conserva la relacion aspecto de acuerdo al ancho de la imagen
                var height = (indicatrix.MaxWidth * _imageWatermark.Height) / _imageWatermark.Width;

                var img = Resize.Apply(_imageWatermark, indicatrix.MaxWidth, height);

                img = Rotate.Apply(img, indicatrix.Angle);

                var posX = indicatrix.X - (img.Width / 2);
                var posY = indicatrix.Y - (img.Height / 2);

                if (indicatrix.Position == Position.Top) {

                    this._topFisheye.InsertImageUnmanaged(img, posX, posY);

                } else {

                    img._image.RotateFlip(RotateFlipType.RotateNoneFlipXY);

                    this._bottomFisheye.InsertImageUnmanaged(img, posX, posY);

                }

            }

            // Se realiza la deformación fisheye

            var feTop = Fisheye.Convert(this._topFisheye);
            var feBottom = Fisheye.Convert(this._bottomFisheye);

            // Se deforman los fisheye a landscape

            feBottom = Fisheye.ToLandscape(feBottom);

            feTop = Fisheye.ToLandscape(feTop);
            feTop._image.RotateFlip(RotateFlipType.RotateNoneFlipXY);

            // Se insertan en el bitmap "watermak"

            _waterMark.InsertImageUnmanaged(feTop, 0, 0);
            _waterMark.InsertImageUnmanaged(feBottom, 0, feBottom.Height);

            // Se insertan aquellos indicatrix que pertenezcan al centro de la imagen

            foreach (var indicatrix in _indicatrixes.Where(i => i.Position == Position.Center)) {

                var height = (indicatrix.MaxWidth * _imageWatermark.Height) / _imageWatermark.Width;

                var img = Resize.Apply(_imageWatermark, indicatrix.MaxWidth, height) as WatermarkBitmap;

                _waterMark.InsertImageUnmanaged(img, indicatrix.X - img.Width / 2, indicatrix.Y - img.Height / 2);

            }

            _waterMark.Save(@"C:\Users\Euler\Pictures\WMpreared2.png", ImageFormat.Png);

        }

        /// <summary>
        /// Se aplica la marca de agua.
        /// </summary>
        /// <returns>Imagen 360° con marca de agua</returns>
        public Bitmap360 Apply() {

            return new Bitmap360(_algorithm.InsertWatermark(_image360, _waterMark, 0, 0));

        }

    }

}
