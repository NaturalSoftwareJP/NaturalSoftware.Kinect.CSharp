using System.Linq;
using System.Collections.Generic;
using Microsoft.Kinect;
using System.Windows.Controls;

namespace NaturalSoftware.Kinect
{
    public static class SkeletonExtensions
    {
        /// <summary>
        /// 追跡されているJointの一覧を取得する
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        public static IEnumerable<Joint> GetTrackedJoints( this Skeleton skeleton )
        {
            return from j in skeleton.Joints
                   where j.TrackingState == JointTrackingState.Tracked
                   select j;
        }

        /// <summary>
        /// 追跡されているJointの一覧を取得する
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        public static IEnumerable<Joint> GetTrackedOrInferredJoints( this Skeleton skeleton )
        {
            return from j in skeleton.Joints
                   where j.TrackingState != JointTrackingState.NotTracked
                   select j;
        }

        /// <summary>
        /// 追跡されているJointの一覧を取得する
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        public static IEnumerable<Joint> GetNearModeJoints( this Skeleton skeleton )
        {
            return skeleton.Joints
                .Where( joint => joint.IsNearModeJoint() );
        }
    }
}
