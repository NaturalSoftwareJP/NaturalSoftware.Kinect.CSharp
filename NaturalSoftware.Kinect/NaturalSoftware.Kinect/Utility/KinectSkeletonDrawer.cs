using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    public class KinectSkeletonDrawer
    {
        KinectSensor kinect;
        CoordinateMapper mapper;
        Canvas canvas;

        /// <summary>
        /// スケルトン座標を、カラーカメラの座標にするか、Depthカメラ座標にするか
        /// </summary>
        public enum Skeleton2DPoint
        {
            Color,
            Depth,
        }

        /// <summary>
        /// 変換種別
        /// </summary>
        public Skeleton2DPoint SkeletonConvert
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kinect"></param>
        /// <param name="canvas"></param>
        public KinectSkeletonDrawer( KinectSensor kinect, Canvas canvas )
        {
            this.kinect = kinect;
            this.mapper = kinect.CoordinateMapper;
            this.canvas = canvas;

            SkeletonConvert = Skeleton2DPoint.Depth;
        }

        /// <summary>
        /// スケルトンを描画する
        /// </summary>
        /// <param name="kinect"></param>
        /// <param name="skeletonFrame"></param>
        public void Draw( Skeleton skeleton )
        {
            // スケルトンがトラッキング状態の場合は、ジョイントを描画する
            if ( skeleton.TrackingState == SkeletonTrackingState.Tracked ) {
                foreach ( Joint joint in skeleton.GetTrackedOrInferredJoints() ) {
                    // ジョイントがトラッキングされていれば、ジョイントの座標を描く
                    DrawJointLine( skeleton, joint );
                    DrawEllipse( joint.Position );
                }
            }
            // スケルトンが位置追跡の場合は、スケルトン位置(Center hip)を描画する
            else if ( skeleton.TrackingState == SkeletonTrackingState.PositionOnly ) {
                DrawEllipse( skeleton.Position );
            }
        }

        /// <summary>
        /// ジョイント間の線を引く
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        private void DrawJointLine( Skeleton skeleton, Joint joint )
        {
            // ジョイントと関連づいている一方のジョイントを取得する
            var start = skeleton.BoneOrientations[joint.JointType].StartJoint;
            var end = skeleton.BoneOrientations[joint.JointType].EndJoint;
            var startJoint = skeleton.Joints[start];
            var endJoint = skeleton.Joints[end];

            // どちらかが追跡状態でない場合には描画しない
            if ( (startJoint.TrackingState == JointTrackingState.NotTracked) || 
                 (endJoint.TrackingState == JointTrackingState.NotTracked) ) {
                return;
            }

            // 3次元座標を距離カメラの2次元座標に変換する
            var startPoint = SkeletonToPiont( skeleton.Joints[start].Position );
            var endPoint = SkeletonToPiont( skeleton.Joints[end].Position );
            if ( !IsDrawablePoint( startPoint ) || !IsDrawablePoint( endPoint ) ) {
                return;
            }


            // ジョイント間の線を引く
            canvas.Children.Add( new Line()
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = new SolidColorBrush( Colors.Red ),
            } );
        }

        /// <summary>
        /// ジョイントの円を描く
        /// </summary>
        /// <param name="kinect"></param>
        /// <param name="position"></param>
        private void DrawEllipse( SkeletonPoint position )
        {
            const int R = 5;

            // スケルトンの座標を、Depthカメラの座標に変換する
            var point = SkeletonToPiont( position );
            if ( !IsDrawablePoint( point ) ) {
                return;
            }

            // 円を描く
            canvas.Children.Add( new Ellipse()
            {
                Fill = new SolidColorBrush( Colors.Red ),
                Margin = new Thickness( point.X - R, point.Y - R, 0, 0 ),
                Width = R * 2,
                Height = R * 2,
            } );
        }

        /// <summary>
        /// スケルトン(3D)座標を2D座標に変換する
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Point SkeletonToPiont( SkeletonPoint position )
        {
            Point point;
            Point _2d;
            if ( SkeletonConvert == Skeleton2DPoint.Color ) {
                var p = mapper.MapSkeletonPointToColorPoint( position, kinect.ColorStream.Format );
                point = new Point( p.X, p.Y );
                _2d = new Point( kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight );
            }
            else {
                var p = mapper.MapSkeletonPointToDepthPoint( position, kinect.DepthStream.Format );
                point = new Point( p.X, p.Y );
                _2d = new Point( kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight );
            }

            return KinectUtility.ScaleTo( point, _2d, new Point( canvas.ActualWidth, canvas.ActualHeight ) );
        }

        /// <summary>
        /// キャンバスに描画可能な座標か確認
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        bool IsDrawablePoint( Point point )
        {
            bool expr1 = (0 <= point.X) && (point.X < canvas.ActualWidth);
            bool expr2 = (0 <= point.Y) && (point.Y < canvas.ActualHeight);

            return expr1 && expr2;
        }
    }
}
