using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    public static class KinectUtility
    {
        public static double ScaleTo( double value, double source, double dest )
        {
            return (value * dest) / source;
        }

        public static Point ScaleTo( Point value, Point source, Point dest )
        {
            return new Point(
                ScaleTo( value.X, source.X, dest.X ),
                ScaleTo( value.Y, source.Y, dest.Y )
                );
        }

        /// <summary>
        /// 指定された点により近いユーザーをアクティブにする
        /// </summary>
        /// <param name="skeletons"></param>
        public static void SetActivePlayerAtCenter( SkeletonStream stream, SkeletonFrame frame, SkeletonPoint centerPosition )
        {
            Skeleton player = null;
            double distance = 100;

            var skeletons = frame.ToSkeletonData();
            foreach ( var skeleton in skeletons ) {
                if ( skeleton.TrackingState != SkeletonTrackingState.NotTracked ) {
                    double new_ = Distance( centerPosition, skeleton.Position );
                    if ( (new_ < distance) ) {
                        player = skeleton;
                        distance = new_;
                    }
                }
            }

            if ( player != null ) {
                stream.ChooseSkeletons( player.TrackingId );
            }
            else {
                stream.ChooseSkeletons();
            }
        }

        private static double Distance( SkeletonPoint centerPosition, SkeletonPoint position )
        {
            return Math.Sqrt( Math.Pow( (centerPosition.X - position.X), 2 ) + Math.Pow( (centerPosition.Z - position.Z), 2 ) );
        }
    }
}
