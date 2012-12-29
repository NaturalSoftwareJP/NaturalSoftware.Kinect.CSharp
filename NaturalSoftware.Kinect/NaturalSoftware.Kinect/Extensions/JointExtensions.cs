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
    } 
}
