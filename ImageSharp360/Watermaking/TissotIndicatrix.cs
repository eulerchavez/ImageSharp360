namespace ImageSharp360.Watermaking {

    using Imaging;

    /// <summary>
    /// Indica a que imagen se aplicará durante el proceso de marcado: <see cref="FisheyeBitmap"/> o <see cref="Bitmap360"/>.
    /// </summary>
    public enum Position {

        /// <summary>
        /// Fisheye de la parte superior.
        /// </summary>
        Top,

        /// <summary>
        /// Parte central de la imagen 360°.
        /// </summary>
        Center,

        /// <summary>
        /// Fisheye de la parte inferior.
        /// </summary>
        Bottom

    }

    /// <summary>
    /// Posición de inserción de la marca de agua.
    /// </summary>
    public struct TissotIndicatrix {

        /// <summary>
        /// Coordenada X de la imagen.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Coordenada Y de la imagen.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Ancho máximo posible de la imagen.
        /// </summary>
        public int MaxWidth { get; private set; }

        /// <summary>
        /// Alto máximo posible de la imagen.
        /// </summary>
        public int MaxHeight { get; private set; }

        /// <summary>
        /// Establece la posición de inserción de acuerdo a <see cref="Position"/>.
        /// </summary>
        public Position Position { get; private set; }

        /// <summary>
        /// Ángulo de rotación.
        /// </summary>
        public int Angle { get; private set; }

        /// <summary>
        /// Posicion central del Fisheye superior.
        /// </summary>
        public static readonly TissotIndicatrix TopIndicatrix = new TissotIndicatrix() {

            X = 1944,
            Y = 1944,
            MaxWidth = 400,
            MaxHeight = 400,
            Position = Position.Top,
            Angle = 0

        };

        /// <summary>
        /// Posicion central del Fisheye inferior.
        /// </summary>
        public static readonly TissotIndicatrix BottomIndicatrix = new TissotIndicatrix() {

            X = 1944,
            Y = 1944,
            MaxWidth = 400,
            MaxHeight = 400,
            Position = Position.Bottom,
            Angle = 0

        };

        /// <summary>
        /// Primero indice superior.
        /// </summary>
        public static readonly TissotIndicatrix FirstIndicatrix = new TissotIndicatrix() {

            X = 2325,
            Y = 1560,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Top,
            Angle = 225

        };

        /// <summary>
        /// Segundo indice superior.
        /// </summary>
        public static readonly TissotIndicatrix SecondIndicatrix = new TissotIndicatrix() {

            X = 1560,
            Y = 1560,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Top,
            Angle = 135

        };

        /// <summary>
        /// Tercer indice superior.
        /// </summary>
        public static readonly TissotIndicatrix ThirdIndicatrix = new TissotIndicatrix() {

            X = 1560,
            Y = 2325,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Top,
            Angle = 45

        };

        /// <summary>
        /// Cuarto indice superior.
        /// </summary>
        public static readonly TissotIndicatrix FourthIndicatrix = new TissotIndicatrix() {

            X = 2325,
            Y = 2325,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Top,
            Angle = 315

        };

        /// <summary>
        /// Primer indice inferior.
        /// </summary>
        public static readonly TissotIndicatrix FifthIndicatrix = new TissotIndicatrix() {

            X = 2325,
            Y = 1560,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Bottom,
            Angle = 225

        };

        /// <summary>
        /// Segundo indice inferior.
        /// </summary>
        public static readonly TissotIndicatrix SixthIndicatrix = new TissotIndicatrix() {

            X = 1560,
            Y = 1560,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Bottom,
            Angle = 135

        };

        /// <summary>
        /// Tercer indice inferior.
        /// </summary>
        public static readonly TissotIndicatrix SeventhIndicatrix = new TissotIndicatrix() {

            X = 1560,
            Y = 2325,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Bottom,
            Angle = 45

        };

        /// <summary>
        /// Cuarto indice inferior.
        /// </summary>
        public static readonly TissotIndicatrix EighthIndicatrix = new TissotIndicatrix() {

            X = 2325,
            Y = 2325,
            MaxWidth = 600,
            MaxHeight = 600,
            Position = Position.Bottom,
            Angle = 315

        };

        /// <summary>
        /// Primer indice central.
        /// </summary>
        public static readonly TissotIndicatrix NinthIndicatrix = new TissotIndicatrix() {

            X = 972,
            Y = 1944,
            MaxWidth = 1000,
            MaxHeight = 1000,
            Position = Position.Center,
            Angle = 0

        };

        /// <summary>
        /// Segundo indice central.
        /// </summary>
        public static readonly TissotIndicatrix TenthIndicatrix = new TissotIndicatrix() {

            X = 2916,
            Y = 1944,
            MaxWidth = 1000,
            MaxHeight = 1000,
            Position = Position.Center,
            Angle = 0

        };

        /// <summary>
        /// Tercer indice central.
        /// </summary>
        public static readonly TissotIndicatrix EleventhIndicatrix = new TissotIndicatrix() {

            X = 4860,
            Y = 1944,
            MaxWidth = 1000,
            MaxHeight = 1000,
            Position = Position.Center,
            Angle = 0

        };

        /// <summary>
        /// Cuarto indice central.
        /// </summary>
        public static readonly TissotIndicatrix TwelfthIndicatrix = new TissotIndicatrix() {

            X = 6804,
            Y = 1944,
            MaxWidth = 1000,
            MaxHeight = 1000,
            Position = Position.Center,
            Angle = 0

        };

    }

}