using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;
using NaturalSoftware.Kinect;

namespace NaturalSoftware.Kinect
{
    public static class JointExtensions
    {
        static JointType[] nearModeJoints = new JointType[] {
            JointType.ShoulderCenter,
            JointType.Head,
            JointType.ShoulderLeft,
            JointType.ElbowLeft,
            JointType.WristLeft,
            JointType.HandLeft,
            JointType.ShoulderRight,
            JointType.ElbowRight,
            JointType.WristRight,
            JointType.HandRight,
        };

        public static bool IsNearModeJoint( this Joint joint )
        {
            return nearModeJoints.Where( type => (joint.JointType == type) ).Count() != 0;
        }

        public static bool IsTracking( this Joint joint )
        {
            return joint.TrackingState == JointTrackingState.Tracked;
        }

        public static bool IsTrackingOrInferred( this Joint joint )
        {
            return (joint.TrackingState == JointTrackingState.Tracked) ||
                   (joint.TrackingState == JointTrackingState.Inferred);
        }

        public static double Distance( this Joint joint, Joint dest )
        {
            return Math.Sqrt( Math.Pow( (joint.Position.X - dest.Position.X), 2 ) +
                              Math.Pow( (joint.Position.Y - dest.Position.Y), 2 ) + 
                              Math.Pow( (joint.Position.Z - dest.Position.Z), 2 ) );
        }
    } 
}
