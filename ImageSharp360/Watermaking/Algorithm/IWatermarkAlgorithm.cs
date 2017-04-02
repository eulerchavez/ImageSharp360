namespace ImageSharp360.Watermaking {

    // Se importa todo lo necesario
    using System.Drawing;
    using Imaging;

    /// <summary>
    /// Interfaz que otorga la habilidad de realizar la inserción de la marca con diferente algoritmo.
    /// </summary>
    public interface IWatermarkAlgorithm {

        /// <summary>
        /// Se realiza la inserción de la marca de agua en toda la imagen 360°.
        /// </summary>
        /// <param name="image360">Imagen 360°</param>
        /// <param name="watermark">Marca de agua</param>
        Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark);

        /// <summary>
        /// Se realiza la inserción de la marca de agua en toda la imagen 360° a partir de una coordenada (X, Y) de la imagen 360°.
        /// </summary>
        /// <param name="image360">Imagen 360°</param>
        /// <param name="watermark">Marca de agua</param>
        /// <param name="x">Indice X</param>
        /// <param name="y">Indice Y</param>
        Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark, int x, int y);

    }

}
