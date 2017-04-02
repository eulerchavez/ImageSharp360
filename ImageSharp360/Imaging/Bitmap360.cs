namespace ImageSharp360.Imaging {

    // Se importa todo lo necesario

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Clase que deriva de <see cref="AbstractBitmap"/>.
    /// Representa una imagen 360° JPEG.
    /// </summary>
    public unsafe class Bitmap360 : AbstractBitmap {

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Bitmap360"/>.
        /// </summary>
        public Bitmap360(String image) : base(image) {

            if (this._image.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("La imágen no cuenta con el formato Format24bppRgb");

            if (Math.Abs(this._image.Width / this._image.Height - 2) > 0.001)
                throw new FormatException("La relación de aspecto no es 2:1.");

        }

        /// <summary>
        /// Se obtiene el pixel (<see cref="Color"/>) 
        /// en la posición X y Y especificada.
        /// <para>
        /// En caso de desear accesar a los pixeles de forma "rapida".
        ///     <para>
        ///         <code>
        ///             <para>// Bloquear los pixeles</para>
        ///             <para>LockBits();</para>
        ///             <para>// Accedder a ellos</para>
        ///             <para>var pixel = img[x, y];</para>
        ///             <para>// Desbloquear los pixeles</para>
        ///             <para>UnlockBits();</para>
        ///         </code>
        ///     </para>
        /// </para>
        /// </summary>
        /// <param name="x">Posición X</param>
        /// <param name="y">Posición Y</param>
        /// <returns><see cref="Color"/></returns>
        public override Color this[int x, int y] {

            get {

                // Si los bits no estan bloqueados

                if (this._data == null)
                    return base[x, y];

                // Se valida el rango

                if ((x < 0 || x > this.Width) || (y < 0 || y > this.Height))
                    throw new IndexOutOfRangeException();

                byte* currentLine = _firstPixel + (y * _data.Stride);

                var column = x * 3;

                return Color.FromArgb(
                    // currentLine[column + 3], // A
                    currentLine[column + 2], // R
                    currentLine[column + 1], // G
                    currentLine[column]);    // B

            }

            set {

                // Si los bits NO estan bloqueados

                if (this._data == null) {

                    base[x, y] = value;

                    return;

                }

                // Se valida el rango

                if ((x < 0 || x > this.Width) || (y < 0 || y > this.Height))
                    throw new IndexOutOfRangeException();

                byte* currentLine = _firstPixel + (y * _data.Stride);

                var column = x * 3;

                // currentLine[column + 3] = value.A;
                currentLine[column + 2] = value.R;
                currentLine[column + 1] = value.G;
                currentLine[column] = value.B;

            }

        }

        /// <summary>
        /// Se liberan los recursos.
        /// </summary>
        ~Bitmap360() {

            Dispose(false);

        }

    }

}
