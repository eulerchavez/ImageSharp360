namespace ImageSharp360.Watermaking.Algorithm {

    using System.Drawing;
    using System.Threading.Tasks;
    using Imaging;

    public class Factores : IWatermarkAlgorithm {

        public Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark) {

            return InsertWatermark(image360, watermark, 0, 0);

        }

        public Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark, int x, int y) {

            return InsertWatermarkUnmanaged(image360, watermark, 0.5F)._image;

        }

        internal unsafe Bitmap360 InsertWatermarkUnmanaged(Bitmap360 image360, WatermarkBitmap watermark, float factor) {

            var res = image360.Clone() as Bitmap360;

            try {

                image360.LockBits();
                watermark.LockBits();
                res.LockBits();

                Parallel.For(0, image360.Height, fila => {

                    Parallel.For(0, image360.Width, columna => {

                        // Se obtienen los pixeles

                        var pixel360 = image360[columna, fila];
                        var pixelWM = watermark[columna, fila];

                        // Solo se toma en cuenta a los pixeles que NO son transparentes

                        if (pixelWM.A == 255) {

                            // Se asigna el pixel resultante

                            res[columna, fila] = Color.FromArgb(
                                (int) (pixel360.R * factor + pixelWM.R * factor),
                                (int) (pixel360.G * factor + pixelWM.G * factor),
                                (int) (pixel360.B * factor + pixelWM.B * factor));

                        }

                    });

                });

            } finally {

                image360.UnlockBits();
                watermark.UnlockBits();
                res.UnlockBits();

            }

            return res;

        }

    }

}