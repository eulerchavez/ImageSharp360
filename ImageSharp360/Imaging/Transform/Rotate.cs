// Se importa todo lo necesario

using System;
using System.Drawing;

namespace ImageSharp360.Imaging.Transform {

    /// <summary>
    /// Clase para realizar la rotación de imágenes bitmap.
    /// </summary>
    public static class Rotate {

        /// <summary>
        /// Realiza el proceso de rotación de un bitmap dado.
        /// </summary>
        /// <param name="image">Bitmap</param>
        /// <param name="angle">Angulo de giro</param>
        public static AbstractBitmap Apply<T>(T image, float angle = 180) where T : AbstractBitmap {

            // Se crea una copia de la imagen

            var rotated = image.Clone() as T;

            angle = angle % 360;
            if (angle > 180)
                angle -= 360;

            float sin = (float) Math.Abs(Math.Sin(angle * Math.PI / 180.0)); // this function takes radians
            float cos = (float) Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
            float newImgWidth = sin * image.Height + cos * image.Width;
            float newImgHeight = sin * image.Width + cos * image.Height;
            float originX = 0f;
            float originY = 0f;

            if (angle > 0) {
                if (angle <= 90)
                    originX = sin * image.Height;
                else {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * image.Width;
                }
            } else {
                if (angle >= -90)
                    originY = sin * image.Width;
                else {
                    originX = newImgWidth - sin * image.Height;
                    originY = newImgHeight;
                }
            }

            Bitmap newImg = new Bitmap((int) newImgWidth, (int) newImgHeight, image.PixelFormat);

            Graphics g = Graphics.FromImage(newImg);
            g.TranslateTransform(originX, originY); // offset the origin to our calculated values
            g.RotateTransform(angle); // set up rotate
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(image._image, 0, 0); // draw the image at 0, 0
            g.Dispose();

            rotated._image = newImg;
            rotated.Width = rotated._image.Width;
            rotated.Height = rotated._image.Height;

            return rotated;

        }

    }

}
