using System;
using System.Windows;

namespace NaturalSoftware.Kinect.TestApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensorWrapper kinect;
        KinectSkeletonDrawer skeletonDrawer;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += ( s, e ) => Start();
            Unloaded += ( s, e ) => Stop();
        }

        /// <summary>
        /// Color,Depth,Skeletonの更新
        /// </summary>
        /// <param name="e"></param>
        void kinect_AllFrameReady( KinectUpdateFrameData e )
        {
            try {
                if ( !e.IsAllUpdated ) {
                    return;
                }

                imageRgb.Source = e.ColorFrame.ToBitmapSource();
                imageDepth.Source = e.DepthFrame.ToBitmapSource();

                canvasSkeleton.Children.Clear();
                foreach ( var skeleton in e.SkeletonFrame.GetTrackedOrPositionOnlySkeleton() ) {
                        skeletonDrawer.Draw( skeleton );
                }
            }
            catch ( Exception ex ) {
                MessageBox.Show( ex.Message );
            }
        }

        /// <summary>
        /// 動作の開始
        /// </summary>
        private void Start()
        {
            try {
                // 基本的な設定は行っているので、イベント設定と開始すれば動きます
                kinect = new KinectSensorWrapper();
                kinect.AllFrameReady += kinect_AllFrameReady;
                kinect.Start();

                skeletonDrawer =
                    new KinectSkeletonDrawer( kinect.Sensor, canvasSkeleton );
            }
            catch ( Exception ex ) {
                MessageBox.Show( ex.Message );
            }
        }

        /// <summary>
        /// 動作の停止
        /// </summary>
        private void Stop()
        {
            try {
                if ( kinect != null ) {
                    kinect.Stop();
                    kinect.AllFrameReady -= kinect_AllFrameReady;
                    kinect = null;
                }
            }
            catch ( Exception ex ) {
                MessageBox.Show( ex.Message );
            }
        }
    }
}
