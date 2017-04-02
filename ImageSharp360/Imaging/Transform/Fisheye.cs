namespace ImageSharp360.Imaging.Transform {

    // Se importa todo lo necesario

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    /// <summary>
    /// Clase para poder realizar las transformaciones relacionadas con el efecto/transformación Fisheye.
    /// </summary>
    public static class Fisheye {

        /// <summary>
        /// Se aplica la tranformación Fisheye.
        /// </summary>
        public static FisheyeBitmap Convert(FisheyeBitmap image) {

            var temp = image.Clone() as FisheyeBitmap;

            temp._image = Convert(temp._image);

            return temp;

        }

        /// <summary>
        /// Algoritmo de transformación a Fisheye.
        /// </summary>
        private static unsafe Bitmap Convert(Bitmap image) {

            // Se valida el tamaño de la imagen
            if (image.Width != image.Height) {
                throw new FormatException("El tamaño de la imagen debe ser igual en ancho y alto.");
            }

            // Se valida el formato de la imagen
            if (image.PixelFormat != PixelFormat.Format32bppArgb) {
                throw new FormatException("El formato de los pixeles debe ser de 32 bits ARGB");
            }

            // Obtenemos una copia de la imagen
            var tempImg = (Bitmap) image.Clone();

            // Obtenemos el ancho y alto de la imagen
            var ancho = tempImg.Width;
            var alto = tempImg.Height;

            // Creamos la imagen fisheye
            var fisheyeImage = new Bitmap(ancho, alto, tempImg.PixelFormat);

            // Bloqueamos los bits de la imagen
            BitmapData bitmapDataTempImg = tempImg.LockBits(new Rectangle(0, 0, ancho, alto), ImageLockMode.ReadWrite, tempImg.PixelFormat);
            BitmapData bitmapDataFishEye = fisheyeImage.LockBits(new Rectangle(0, 0, ancho, alto), ImageLockMode.ReadWrite, fisheyeImage.PixelFormat);

            // Obtenemos el primer pixel
            byte* PtrFirstPixelImg = (byte*) bitmapDataTempImg.Scan0;
            byte* PtrFirstPixelFishEye = (byte*) bitmapDataFishEye.Scan0;

            try {

                Parallel.For(0, alto, fila => {

                    // Normalizaremos la coordenada Y en el intervalo [-1 , 1]
                    double n_y = ((2.0 * (double) fila) / alto) - 1.0;

                    // Obtenemos n_y^2
                    double n_y_2 = n_y * n_y;

                    // Iteramos por columna
                    for (int columna = 0; columna < ancho; columna++) {

                        // Normalizamos de la misma forma la coordenada X
                        double n_x = ((2.0 * (double) columna) / ancho) - 1.0;

                        // Obtenemos n_x^2
                        double n_x_2 = n_x * n_x;

                        // Se calcula la distancia del centro
                        double r = Math.Sqrt(n_x_2 + n_y_2);

                        // Descartamos los pixeles fuera del circulo
                        if ((0.0 <= r) && (r <= 1.0)) {

                            // Calculamos n_r
                            double n_r = Math.Sqrt(1.0 - (r * r));

                            // Ajustamos esto entre 0 y 1
                            n_r = (r + (1.0 - n_r)) / 2.0;

                            // Descartamos radios mayores a 1
                            if (n_r <= 1.0) {

                                // Calculamos el angulo para coordenadas polares
                                double theta = Math.Atan2(n_y, n_x);

                                // Calculamos la posición en X con este angulo
                                double n_x_n = (n_r * Math.Cos(theta));

                                // Ahora la posición en Y
                                double n_y_n = (n_r * Math.Sin(theta));

                                // Se pasan las coordenadas [-1, 1] a [0, ancho]
                                int x_d = (int) (((n_x_n + 1.0) * ancho) / 2.0);

                                // Lo mismo pasa con Y
                                int y_d = (int) (((n_y_n + 1.0) * alto) / 2.0);

                                // Mapeamos el pixel en estas coordenadas de la Imagen Original a la posición actual en la Imagen Destino

                                // Obtenemos la fila de la Imagen
                                byte* currentLineImg = PtrFirstPixelImg + ((y_d % alto) * bitmapDataTempImg.Stride);

                                // Obtenemos la fila de la Imagen fisheye
                                byte* currentLineFishEye = PtrFirstPixelFishEye + (fila * bitmapDataFishEye.Stride);

                                var temp = (x_d % ancho) * 4;
                                var col = columna * 4;

                                currentLineFishEye[col] = currentLineImg[temp];
                                currentLineFishEye[col + 1] = currentLineImg[temp + 1];
                                currentLineFishEye[col + 2] = currentLineImg[temp + 2];
                                currentLineFishEye[col + 3] = currentLineImg[temp + 3];

                            }

                        }

                    }

                });

            } finally {

                tempImg.UnlockBits(bitmapDataTempImg);
                fisheyeImage.UnlockBits(bitmapDataFishEye);

            }

            return fisheyeImage;

        }

        /// <summary>
        /// Se realiza la transformación de una imagen bitmap Fisheye a su equivalente en vista Panoramica.
        /// </summary>
        public static FisheyeBitmap ToLandscape(FisheyeBitmap image) {

            var temp = image.Clone() as FisheyeBitmap;

            temp._image = ToLandscape(temp._image);
            temp.Width = temp._image.Width;
            temp.Height = temp._image.Height;

            return temp;

        }

        /// <summary>
        /// Algoritmo de transformación a Panoramica.
        /// </summary>
        private unsafe static Bitmap ToLandscape(Bitmap image) {

            // Obtenemos una copia de la imagen
            var tempImg = (Bitmap) image.Clone();

            // Obtenemos el ancho y alto de la imagen Fisheye
            var anchoFisheye = tempImg.Width;
            var altoFisheye = tempImg.Height;

            // Calulamos el ancho y alto de la imagen landscape
            var altoLandscape = anchoFisheye / 2;
            var anchoLandscape = (anchoFisheye / 2) * 4;

            // Creamos la imagen landscape
            Bitmap landscapeImage = new Bitmap(anchoLandscape, altoLandscape, tempImg.PixelFormat);

            // Bloqueamos los bits de la imagen
            BitmapData bitmapDataTempImg = tempImg.LockBits(new Rectangle(0, 0, anchoFisheye, altoFisheye), ImageLockMode.ReadWrite, tempImg.PixelFormat);
            BitmapData bitmapDataLandscape = landscapeImage.LockBits(new Rectangle(0, 0, anchoLandscape, altoLandscape), ImageLockMode.ReadWrite, landscapeImage.PixelFormat);

            // Obtenemos el primer pixel
            byte* PtrFirstPixelImg = (byte*) bitmapDataTempImg.Scan0;
            byte* PtrFirstPixelLandscape = (byte*) bitmapDataLandscape.Scan0;

            try {

                Parallel.For(0, altoLandscape, fila => { // for (int fila = 0; fila < landscapeImage.Height; ++fila) { 

                    for (int columna = 0; columna < anchoLandscape; ++columna) {

                        double radius = (double) (altoLandscape - fila);

                        double theta = 2.0 * Math.PI * (double) (-columna) / (double) (4.0 * altoLandscape);

                        double fTrueX = radius * Math.Cos(theta);
                        double fTrueY = radius * Math.Sin(theta);

                        int x = (int) (Math.Round(fTrueX)) + altoLandscape;
                        int y = altoLandscape - (int) (Math.Round(fTrueY));

                        if (x >= 0 && x < (2 * altoLandscape) && y >= 0 && y < (2 * altoLandscape)) {

                            byte* currentLineImg = PtrFirstPixelImg + (y * bitmapDataTempImg.Stride);

                            byte* currentLineLandscape = PtrFirstPixelLandscape + (fila * bitmapDataLandscape.Stride);

                            var col = columna * 4;

                            currentLineLandscape[col] = currentLineImg[x * 4];
                            currentLineLandscape[col + 1] = currentLineImg[x * 4 + 1];
                            currentLineLandscape[col + 2] = currentLineImg[x * 4 + 2];
                            currentLineLandscape[col + 3] = currentLineImg[x * 4 + 3];

                        }

                    }

                });

            } finally {

                tempImg.UnlockBits(bitmapDataTempImg);
                landscapeImage.UnlockBits(bitmapDataLandscape);

            }

            return landscapeImage;

        }

    }

}
