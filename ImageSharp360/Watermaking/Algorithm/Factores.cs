namespace ImageSharp360.Watermaking.Algorithm {

    // Se importa todo lo necesario

    using System.Drawing;
    using System.Threading.Tasks;
    using Imaging;

    /// <summary>
    /// Clase que proporciona el algoritmo para el marcado.
    /// </summary>
    public class Factores : IWatermarkAlgorithm {

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

            return InsertWatermarkUnmanaged(image360, watermark, 0.5F)._image;

        }

        /// <summary>
        /// Algoritmo para la inserción de la marca de agua, se emplea un factor de 0.5.
        /// </summary>
        internal unsafe Bitmap360 InsertWatermarkUnmanaged(Bitmap360 image360, WatermarkBitmap watermark, float factor) {

            try {

                image360.LockBits();
                watermark.LockBits();

                Parallel.For(0, image360.Height, fila => {

                    Parallel.For(0, image360.Width, columna => {

                        // Se obtienen los pixeles

                        var pixel360 = image360[columna, fila];
                        var pixelWM = watermark[columna, fila];

                        // Solo se toma en cuenta a los pixeles que NO son transparentes

                        if (pixelWM.A == 255) {

                            // Se asigna el pixel resultante

                            image360[columna, fila] = Color.FromArgb(
                                (int) (pixel360.R * factor + pixelWM.R * factor),
                                (int) (pixel360.G * factor + pixelWM.G * factor),
                                (int) (pixel360.B * factor + pixelWM.B * factor));

                        }

                    });

                });

            } finally {

                image360.UnlockBits();
                watermark.UnlockBits();

            }

            return image360;

        }

    }

}