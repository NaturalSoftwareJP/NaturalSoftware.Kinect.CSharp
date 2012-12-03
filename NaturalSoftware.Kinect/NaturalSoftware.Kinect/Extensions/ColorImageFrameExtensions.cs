using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    /// <summary>
    /// ColorImageFrameの拡張メソッド
    /// </summary>
    public static class ColorImageFrameExtensions
    {
        /// <summary>
        /// 画素のバイト列を取得する
        /// </summary>
        /// <param name="colorFrame"></param>
        /// <returns></returns>
        public static byte[] ToPixelData(this ColorImageFrame colorFrame)
        {
            byte[] pixels = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo(pixels);
            return pixels;
        }

        /// <summary>
        /// 画像をBitmapSourceに変換する
        /// </summary>
        /// <param name="colorFrame"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(this ColorImageFrame colorFrame)
        {
            return BitmapSource.Create(colorFrame.Width, colorFrame.Height,
                96, 96, PixelFormats.Bgr32, null, colorFrame.ToPixelData(),
                colorFrame.Width * colorFrame.BytesPerPixel);
        }
    }
}
