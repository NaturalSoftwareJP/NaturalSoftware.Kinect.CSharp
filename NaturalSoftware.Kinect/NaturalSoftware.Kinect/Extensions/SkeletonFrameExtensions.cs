using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    public static class SkeletonFrameExtensions
    {
        /// <summary>
        /// スケルトンデータを取得する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static Skeleton[] ToSkeletonData( this SkeletonFrame frame )
        {
            Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
            frame.CopySkeletonDataTo( skeletons );
            return skeletons;
        }

        /// <summary>
        /// 追跡しているスケルトンの一覧を取得する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static IEnumerable<Skeleton> GetTrackedSkeleton( this SkeletonFrame frame )
        {
            return from s in frame.ToSkeletonData()
                   where s.TrackingState == SkeletonTrackingState.Tracked
                   select s;
        }

        /// <summary>
        /// 場所のみ追跡しているスケルトンの一覧を取得する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static IEnumerable<Skeleton> GetPositionOnlySkeleton( this SkeletonFrame frame )
        {
            return from s in frame.ToSkeletonData()
                   where s.TrackingState == SkeletonTrackingState.PositionOnly
                   select s;
        }

        /// <summary>
        /// 場所のみ追跡しているスケルトンの一覧を取得する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static IEnumerable<Skeleton> GetTrackedOrPositionOnlySkeleton( this SkeletonFrame frame )
        {
            return from s in frame.ToSkeletonData()
                   where s.TrackingState != SkeletonTrackingState.NotTracked
                   select s;
        }

        /// <summary>
        /// 追跡している最初のスケルトンを取得する
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static Skeleton GetFirstTrackedSkeleton( this SkeletonFrame frame )
        {
            return frame.GetTrackedSkeleton().FirstOrDefault();
        }

        /// <summary>
        /// スケルトンを追跡しているか調べる
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static bool IsTrackedSkeleton( this SkeletonFrame frame )
        {
            return frame.GetFirstTrackedSkeleton() != null;
        }
    }
}
