namespace ImageSharp360.Imaging.Transform {

    // Se importa todo lo necesario

    using System.Drawing;

    /// <summary>
    /// Clase para realizar el corte de imgenes bitmap.
    /// </summary>
    public static class Crop {

        /// <summary>
        /// Realiza el proceso de corte de un bitmap dado.
        /// </summary>
        /// <param name="image">Bitmap</param>
        /// <param name="x">Indice de inicio X</param>
        /// <param name="y">Indice de inicio Y</param>
        /// <param name="width">Ancho a recortar</param>
        /// <param name="height">Alto a recortar</param>
        /// <returns></returns>
        public static Bitmap Apply(AbstractBitmap image, int x, int y, int width, int height) {

            Accord.Imaging.Filters.Crop crop = new Accord.Imaging.Filters.Crop(new Rectangle(x, y, width, height));

            return crop.Apply(image._image);

        }

    }

}
