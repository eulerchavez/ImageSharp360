namespace ImageSharp360.Imaging {

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Transform;

    /// <summary>
    /// Clase que deriva de <see cref="AbstractBitmap"/>.
    /// Representa una imagen Fisheye en conjunto con la transformación <see cref="Fisheye"/>.
    /// </summary>
    public unsafe class FisheyeBitmap : AbstractBitmap {

        /// <summary>
        /// La imagen ya sufrió la transformacion <see cref="Fisheye"/>.
        /// </summary>
        public bool IsTransformed { get; private set; } = false;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FisheyeBitmap"/> con base al tamaño de una instancia de <see cref="Bitmap360"/>.
        /// </summary>
        public FisheyeBitmap(Bitmap360 dimension) : base(new Bitmap(dimension.Width / 2, dimension.Height, PixelFormat.Format32bppArgb)) {

            // Se valida el parametro de entrada

            if (dimension == null)
                throw new ArgumentNullException(nameof(dimension));

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
        ~FisheyeBitmap() {

            Dispose(false);

        }

    }

}
