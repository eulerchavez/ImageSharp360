namespace ImageSharp360.Imaging {

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Clase que deriva de <see cref="AbstractBitmap"/>.
    /// Representa una imagen PNG (con canal de transparencia).
    /// </summary>
    public unsafe class WatermarkBitmap : AbstractBitmap {

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="WatermarkBitmap"/>.
        /// </summary>
        public WatermarkBitmap(Bitmap image) : base(image) {

            // Se valida el parametro de entrada

            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if (image.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("La imágen no cuenta con el formato Format32bppArgb");

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

                var column = x * 4;

                return Color.FromArgb(
                    currentLine[column + 3], // A
                    currentLine[column + 2], // R
                    currentLine[column + 1], // G
                    currentLine[column]);    // B

            }

            set {

                // Si los bits no estan bloqueados

                if (this._data == null) {

                    base[x, y] = value;

                    return;

                }

                // Se valida el rango

                if ((x < 0 || x > this.Width) || (y < 0 || y > this.Height))
                    throw new IndexOutOfRangeException();

                byte* currentLine = _firstPixel + (y * _data.Stride);

                var column = x * 4;

                currentLine[column + 3] = value.A;
                currentLine[column + 2] = value.R;
                currentLine[column + 1] = value.G;
                currentLine[column] = value.B;

            }

        }

        /// <summary>
        /// Se liberan los recursos.
        /// </summary>
        ~WatermarkBitmap() {

            Dispose(false);

        }

    }

}
