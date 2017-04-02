using System;
using System.Drawing;
using ImageSharp360.Imaging;
using ImageSharp360.Watermaking;
using ImageSharp360.Watermaking.Algorithm;
using System.Drawing.Imaging;
using System.IO;

namespace Pruebas {

    class Program {

        static void Main(string[] args) {

            var img360 = (Bitmap) Image.FromStream(new MemoryStream(File.ReadAllBytes(@"C:\Users\Euler\Pictures\SAM_100_0007.jpg")));
            var imgWm = (Bitmap) Image.FromStream(new MemoryStream(File.ReadAllBytes(@"C:\Users\Euler\Pictures\ESIME.png")));

            var imagen360 = new Bitmap360(img360);
            var imagenWm = new WatermarkBitmap(imgWm);

            Watermarking wm = new Watermarking(imagen360, imagenWm, new Factores(),
                //TissotIndicatrix.TopIndicatrix,
                //TissotIndicatrix.BottomIndicatrix,
                TissotIndicatrix.FirstIndicatrix,
                TissotIndicatrix.SecondIndicatrix,
                TissotIndicatrix.ThirdIndicatrix,
                TissotIndicatrix.FourthIndicatrix,
                TissotIndicatrix.FifthIndicatrix,
                TissotIndicatrix.SixthIndicatrix,
                TissotIndicatrix.SeventhIndicatrix,
                TissotIndicatrix.EighthIndicatrix,
                TissotIndicatrix.NinthIndicatrix,
                TissotIndicatrix.TenthIndicatrix,
                TissotIndicatrix.EleventhIndicatrix,
                TissotIndicatrix.TwelfthIndicatrix);

            wm.Prepare();

            wm.Apply().Save(@"C:\Users\Euler\Pictures\ImagenMarcada7.jpg", ImageFormat.Jpeg);

            Console.ReadKey();

        }

    }

}
