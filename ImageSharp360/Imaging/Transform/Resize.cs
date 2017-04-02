namespace ImageSharp360.Imaging.Transform {

    public static class Resize {

        public static AbstractBitmap Apply<T>(T image, int newWidth, int newHeight) where T : AbstractBitmap {

            // Se crea una copia de la imagen

            var temp = image.Clone() as T;

            // Se crea la instancia de rotacion

            Accord.Imaging.Filters.ResizeBilinear res = new Accord.Imaging.Filters.ResizeBilinear(newWidth, newHeight);

            // Se aplica el resize

            temp._image = res.Apply(temp._image);
            temp.Width = temp._image.Width;
            temp.Height = temp._image.Height;


            return temp;

        }

    }

}
