namespace ImageSharp360.Imaging.Transform {

    using System.Drawing;

    public static class Crop {

        public static Bitmap Apply(AbstractBitmap image, int x, int y, int width, int height) {

            Accord.Imaging.Filters.Crop crop = new Accord.Imaging.Filters.Crop(new Rectangle(x, y, width, height));

            return crop.Apply(image._image);

        }

    }

}
