namespace ImageSharp360.Watermaking.Algorithm {

    // Se importa todo lo necesario

    using System.Drawing;
    using System.Threading.Tasks;
    using Imaging;

    /// <summary>
    /// Clase que proporciona el algoritmo para el marcado.
    /// </summary>
    public class Factores : IWatermarkAlgorithm {

        private double _factor;

        /// <summary>
        /// Factor aplicado a la marca de agua.
        /// </summary>
        public double Factor {

            get { return _factor; }

            set {

                if (value > 1.0) {
                    _factor = 1;
                } else if (value < 0.0) {
                    _factor = 0;
                } else {
                    _factor = value;
                }

            }

        }

        /// <summary>
        /// Se inicializa una nueva instancia de la clase <see cref="Factores"/>.
        /// </summary>
        /// <param name="factor">Factor aplicado a la marca de agua.</param>
        public Factores(double factor = 0.5) {

            this.Factor = factor;

        }

        /// <summary>
        /// Se realiza la inserción de la marca de agua en toda la imagen 360°.
        /// Se emplea un factor de 0.5.
        /// </summary>
        /// <param name="image360">Imagen 360°</param>
        /// <param name="watermark">Marca de agua</param>
        public Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark) {

            return InsertWatermark(image360, watermark, 0, 0);

        }

        /// <summary>
        /// Se realiza la inserción de la marca de agua en toda la imagen 360° a partir de una coordenada (X, Y) de la imagen 360°.
        /// Se emplea un factor de 0.5.
        /// </summary>
        /// <param name="image360">Imagen 360°</param>
        /// <param name="watermark">Marca de agua</param>
        /// <param name="x">Indice X</param>
        /// <param name="y">Indice Y</param>
        public Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark, int x, int y) {

            return InsertWatermarkUnmanaged(image360, watermark, this.Factor)._image;

        }

        /// <summary>
        /// Algoritmo para la inserción de la marca de agua, se emplea un factor de 0.5 por defecto.
        /// </summary>
        internal unsafe Bitmap360 InsertWatermarkUnmanaged(Bitmap360 image360, WatermarkBitmap watermark, double factor = 0.5) {

            var factorA = (1 - factor);
            var factorB = factor;

            try {

                image360.LockBits();
                watermark.LockBits();

                Parallel.For(0, image360.Height, fila => {

                    for (int columna = 0; columna < image360.Width; columna++) {

                        // Se obtienen los pixeles

                        var pixel360 = image360[columna, fila];
                        var pixelWM = watermark[columna, fila];

                        // Solo se toma en cuenta a los pixeles que NO son transparentes

                        if (pixelWM.A == 255) {

                            // Se asigna el pixel resultante

                            image360[columna, fila] = Color.FromArgb(
                                (int) (pixel360.R * factorA + pixelWM.R * factorB),
                                (int) (pixel360.G * factorA + pixelWM.G * factorB),
                                (int) (pixel360.B * factorA + pixelWM.B * factorB));

                        }

                    }

                });

            } finally {

                image360.UnlockBits();
                watermark.UnlockBits();

            }

            return image360;

        }

    }

}