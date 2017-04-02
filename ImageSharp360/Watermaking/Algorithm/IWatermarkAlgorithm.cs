namespace ImageSharp360.Watermaking {

    using System.Drawing;
    using Imaging;

    public interface IWatermarkAlgorithm {

        Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark);

        Bitmap InsertWatermark(Bitmap360 image360, WatermarkBitmap watermark, int x, int y);

    }

}
