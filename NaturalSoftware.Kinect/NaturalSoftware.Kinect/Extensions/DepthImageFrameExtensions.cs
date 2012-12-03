using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    /// <summary>
    /// DepthImageFrameの拡張メソッド
    /// </summary>
    public static class DepthImageFrameExtensions
    {
        /// <summary>
        /// 画像化するときのピクセルフォーマット
        /// </summary>
        private static readonly PixelFormat DepthPixelFormat = PixelFormats.Bgr32;

        /// <summary>
        /// ピクセルあたりのバイト数
        /// </summary>
        private static readonly int BytesPerPixel = DepthPixelFormat.BitsPerPixel / 8;

        /// <summary>
        /// 距離データをshort列に変換する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static short[] ToPixelData( this DepthImageFrame depthFrame )
        {
            short[] pixels = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo( pixels );
            return pixels;
        }

        /// <summary>
        /// 距離データをDepthImagePixel列に変換する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static DepthImagePixel[] ToDepthImagePixel( this DepthImageFrame depthFrame )
        {
            var pixels = new DepthImagePixel[depthFrame.PixelDataLength];
            depthFrame.CopyDepthImagePixelDataTo( pixels );
            return pixels;
        }

        /// <summary>
        /// BitmapSourceに変換する
        /// </summary>
        /// <param name="depthFrame"></param>
        /// <param name="depthStream"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource( this DepthImageFrame depthFrame )
        {
            return BitmapSource.Create( depthFrame.Width,
                          depthFrame.Height, 96, 96, DepthPixelFormat, null,
                          ConvertDepthToColor( depthFrame ),
                          depthFrame.Width * BytesPerPixel );
        }

        /// <summary>
        /// 距離データをカラー画像に変換する
        /// </summary>
        /// <param name="depthFrame"></param>
        /// <returns></returns>
        private static byte[] ConvertDepthToColor( DepthImageFrame depthFrame )
        {
            // 距離カメラのピクセルごとのデータを取得する
            short[] depthPixel = depthFrame.ToPixelData();

            // 画像化データを作成する
            byte[] depthColor = new byte[depthFrame.Width * depthFrame.Height * BytesPerPixel];

            // 画像化する
            for ( int i = 0; i < depthPixel.Length; i++ ) {
                // 距離カメラのデータから、距離とプレイヤーIDを取得する
                int distance = depthPixel[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                int player = depthPixel[i] & DepthImageFrame.PlayerIndexBitmask;

                // バイトインデックスを計算する
                int index = i * BytesPerPixel;

                byte gray = (byte)~(byte)KinectUtility.ScaleTo( distance, 0x0FFF, 0xFF );
                depthColor[index + 0] = gray;
                depthColor[index + 1] = gray;
                depthColor[index + 2] = gray;
            }

            return depthColor;
        }

        /// <summary>
        /// BitmapSourceに変換する
        /// </summary>
        /// <param name="depthFrame"></param>
        /// <param name="depthStream"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource( this DepthImageFrame depthFrame, DepthImageStream depthStream )
        {
            return BitmapSource.Create( depthFrame.Width,
                          depthFrame.Height, 96, 96, DepthPixelFormat, null,
                          ConvertDepthToColor( depthFrame, depthStream ),
                          depthFrame.Width * BytesPerPixel );
        }

        /// <summary>
        /// プレイヤーにつける色
        /// </summary>
        static readonly Color[] PlayerColors = new Color[] {
            Colors.Silver,
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Yellow,
            Colors.Pink,
            Colors.Brown,
        };

        /// <summary>
        /// 距離データをカラー画像に変換する
        /// </summary>
        /// <param name="kinect"></param>
        /// <param name="depthFrame"></param>
        /// <returns></returns>
        private static byte[] ConvertDepthToColor( DepthImageFrame depthFrame, DepthImageStream depthStream )
        {
            // 距離カメラのピクセルごとのデータを取得する
            short[] depthPixel = depthFrame.ToPixelData();

            // 画像化データを作成する
            byte[] depthColor = new byte[depthFrame.Width * depthFrame.Height * BytesPerPixel];

            // 画像化する
            for ( int i = 0; i < depthPixel.Length; i++ ) {
                // 距離カメラのデータから、距離とプレイヤーIDを取得する
                int distance = depthPixel[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                int player = depthPixel[i] & DepthImageFrame.PlayerIndexBitmask;

                // バイトインデックスを計算する
                int index = i * BytesPerPixel;

                // プレイヤーがいる座標
                if ( player != 0 ) {
                    depthColor[index + 0] = PlayerColors[player].B;
                    depthColor[index + 1] = PlayerColors[player].G;
                    depthColor[index + 2] = PlayerColors[player].R;
                }
                // プレイヤーがいない座標
                else {
                    // サポート外 0-40cm
                    if ( distance == depthStream.UnknownDepth ) {
                        // Colors.DarkGoldenrod
                        depthColor[index + 0] = 0x0B;
                        depthColor[index + 1] = 0x86;
                        depthColor[index + 2] = 0xB8;
                    }
                    // 近すぎ 40cm-80cm(default mode)
                    else if ( distance == depthStream.TooNearDepth ) {
                        // Colors.White
                        depthColor[index + 0] = 0xFF;
                        depthColor[index + 1] = 0xFF;
                        depthColor[index + 2] = 0xFF;
                    }
                    // 遠すぎ 3m(Near),4m(Default)-8m
                    else if ( distance == depthStream.TooFarDepth ) {
                        // Colors.Purple
                        depthColor[index + 0] = 0x80;
                        depthColor[index + 1] = 0x00;
                        depthColor[index + 2] = 0x80;
                    }
                    // 有効な距離データ
                    else {
                        // Colors.DarkCyan
                        depthColor[index + 0] = 0x8B;
                        depthColor[index + 1] = 0x8B;
                        depthColor[index + 2] = 0x00;
                    }
                }
            }

            return depthColor;
        }
    }
}
