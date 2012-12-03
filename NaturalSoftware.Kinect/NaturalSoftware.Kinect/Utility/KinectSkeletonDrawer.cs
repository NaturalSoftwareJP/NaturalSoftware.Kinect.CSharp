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
        Canvas canvas;

        public KinectSkeletonDrawer( KinectSensor kinect, Canvas canvas )
        {
            this.kinect = kinect;
            this.canvas = canvas;
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
            var startPoint = kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(
                skeleton.Joints[start].Position, kinect.DepthStream.Format );
            var endPoint = kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(
                skeleton.Joints[end].Position, kinect.DepthStream.Format );


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
            var point = kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(
                position, kinect.DepthStream.Format );

            // 円を描く
            canvas.Children.Add( new Ellipse()
            {
                Fill = new SolidColorBrush( Colors.Red ),
                Margin = new Thickness( point.X - R, point.Y - R, 0, 0 ),
                Width = R * 2,
                Height = R * 2,
            } );
        }
    }
}
