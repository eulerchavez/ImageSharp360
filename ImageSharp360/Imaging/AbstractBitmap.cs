namespace ImageSharp360.Imaging {

    // Se importa todo lo necesario

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// Clase base para los Bitmaps de ImageSharp.
    /// </summary>
    public abstract unsafe class AbstractBitmap : ICloneable, IDisposable {

        // Flag: ¿Ha sido llamado "Dispose"?
        internal bool _disposed = false;

        internal Bitmap _image;

        internal BitmapData _data;

        internal byte* _firstPixel;

        private PixelFormat _pixelFormat;

        /// <summary>
        /// Se obtiene el formato de pixel del Bitmap.
        /// </summary>
        public PixelFormat PixelFormat { get { return _pixelFormat; } internal set { _pixelFormat = value; } }

        private int _height;

        /// <summary>
        /// Se obtiene el Alto del Bitmap.
        /// </summary>
        public int Height { get { return this._height; } internal set { _height = value; } }

        private int _width;

        /// <summary>
        /// Se obtiene el Ancho del Bitmap.
        /// </summary>
        public int Width { get { return this._width; } internal set { _width = value; } }

        internal AbstractBitmap() { }

        internal AbstractBitmap(Bitmap bitmap) {

            this._image = bitmap;
            this.Height = bitmap.Height;
            this.Width = bitmap.Width;
            this.PixelFormat = bitmap.PixelFormat;

        }

        /// <summary>
        /// Se inicializa el Bitmap de ImageSharp360 a traves de su ubicación.
        /// </summary>
        public AbstractBitmap(string image) {

            // Se valida el parametro de entrada

            if (image == null)
                throw new ArgumentNullException(nameof(image));

            var imageBytes = File.ReadAllBytes(image);

            using (MemoryStream ms = new MemoryStream(imageBytes)) {

                this._image = (Bitmap) Bitmap.FromStream(ms);

                this.Height = _image.Height;
                this.Width = _image.Width;
                this.PixelFormat = _image.PixelFormat;

            }

        }

        /// <summary>
        /// Se obtiene el pixel (<see cref="Color"/>) 
        /// en la posición X y Y especificada.
        /// <para>
        /// En caso de desear accesar a los pixeles empleando codigo no seguro.
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
        public virtual Color this[int x, int y] {

            get { return _image.GetPixel(x, y); }
            set { _image.SetPixel(x, y, value); }

        }

        /// <summary>
        /// Se bloquean todos los pixeles del Bitmap 
        /// para manejar los metodos Unmaged.
        /// </summary>
        public virtual void LockBits() {

            if (this._data == null) {

                // Se bloquen los bits

                this._data = this._image.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadWrite, this.PixelFormat);

                // Se obtiene el primer pixel

                this._firstPixel = (byte*) this._data.Scan0;

            }

        }

        /// <summary>
        /// Se desbloquean todos los pixeles del Bitmap.
        /// </summary>
        public virtual void UnlockBits() {

            if (this._data != null)
                this._image.UnlockBits(this._data);

            this._data = null;
            this._firstPixel = null;

        }

        /// <summary>
        /// Se guarda la imagen con el nombre especificado.
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename) {

            this._image.Save(filename);

        }

        /// <summary>
        /// Se guarda la imagen con el nombre y formato especificado.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void Save(string filename, ImageFormat format) {

            this._image.Save(filename, format);

        }

        /// <summary>
        /// Se guarda la imagen a traves de un buffer y un formato especificado.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        public void Save(System.IO.Stream stream, ImageFormat format) {

            this._image.Save(stream, format);

        }

        /// <summary>
        /// Se inserta toda la imagen empezando en las coordenadas (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="image">Imagen a insertar</param>
        /// <param name="x">Coordenada X</param>
        /// <param name="y">Coordenada Y</param>
        public virtual void InsertImageUnmanaged<T>(T image, int x, int y) where T : AbstractBitmap {

            InsertImageUnmanaged(image, x, y, image.Width, image.Height);

        }

        /// <summary>
        /// Se inserta imagen empezando en las coordenadas (<paramref name="x"/>, <paramref name="y"/>)
        /// de acuerdo aun tamaño especificado.
        /// </summary>
        /// <param name="image">Imagen a insertar</param>
        /// <param name="x">Coordenada X</param>
        /// <param name="y">Coordenada Y</param>
        /// <param name="width">Tamaño en ancho</param>
        /// <param name="height">Tamaño en alto</param>
        public virtual void InsertImageUnmanaged<T>(T image, int x, int y, int width, int height) where T : AbstractBitmap {

            try {

                // Se bloquean los pixeles

                this.LockBits();
                image.LockBits();

                var posY = y;

                for (int fila = 0; fila < height; fila++, posY++) { //Parallel.For(0, height, fila => { //

                    for (int columna = 0, posX = x; columna < width; columna++, posX++) {

                        this[posX, posY] = image[columna, fila];

                    }

                }

            } finally {

                // Se desbloquean los pixeles

                this.UnlockBits();
                image.UnlockBits();

            }

        }

        /// <summary>
        /// Se obtiene una copia del mismo objeto.
        /// </summary>
        public object Clone() {

            // Se crea una copia superficial

            var img = this.MemberwiseClone() as AbstractBitmap;

            // Se reasigna la misma imagen pero con diferente referencia

            img._image = new Bitmap(this._image.Clone() as Bitmap);

            
            img._pixelFormat = this.PixelFormat;

            return img;

        }

        /// <summary>
        /// Se liberan los recursos.
        /// </summary>
        public void Dispose() {

            Dispose(true);

            GC.SuppressFinalize(this);

        }

        /// <summary>
        /// Se liberan los recursos.
        /// </summary>
        protected virtual void Dispose(bool disposing) {

            if (!_disposed) {

                if (disposing) {

                    // Se libera cualquier objeto gestionado.

                    UnlockBits();
                    this._image.Dispose();

                }

                // Se libera cualquier objeto NO gestionado.

                _disposed = true;

            }

        }

        /// <summary>
        /// Se liberan los recursos.
        /// </summary>
        ~AbstractBitmap() {

            Dispose(false);

        }

    }

}
