using System.Drawing;

namespace ImageSharp360.Imaging.Transform {

    public static class Rotate {

        public static AbstractBitmap Apply<T>(T image, float angle = 180) where T : AbstractBitmap {

            // Se crea una copia de la imagen

            var temp = image.Clone() as T;

            if (angle == 0)
                return temp;


            Bitmap rotatedImage = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(rotatedImage)) {

                graphics.TranslateTransform(image.Width / 2, image.Height / 2);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-image.Width / 2, -image.Height / 2);
                graphics.DrawImage(temp._image, 0, 0, image.Width, image.Height);

            }

            temp._image = rotatedImage;

            return temp;

        }

    }

}
