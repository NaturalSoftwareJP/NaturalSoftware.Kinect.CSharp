using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    public class KinectUpdateFrameData : IDisposable
    {
        private bool disposed = false;

        public ColorImageFrame ColorFrame
        {
            get;
            private set;
        }

        public DepthImageFrame DepthFrame
        {
            get;
            private set;
        }

        public SkeletonFrame SkeletonFrame
        {
            get;
            private set;
        }

        public bool IsAllUpdated
        {
            get
            {
                return (ColorFrame != null) &&
                    (DepthFrame != null) &&
                    (SkeletonFrame != null);
            }
        }

        public KinectUpdateFrameData( AllFramesReadyEventArgs e )
        {
            ColorFrame = e.OpenColorImageFrame();
            DepthFrame = e.OpenDepthImageFrame();
            SkeletonFrame = e.OpenSkeletonFrame();
        }

        public KinectUpdateFrameData( KinectSensorWrapper kinect, int millisecondsWait )
        {
            ColorFrame = kinect.Sensor.ColorStream.OpenNextFrame( millisecondsWait );
            DepthFrame = kinect.Sensor.DepthStream.OpenNextFrame( millisecondsWait );
            SkeletonFrame = kinect.Sensor.SkeletonStream.OpenNextFrame( millisecondsWait );
        }

        ~KinectUpdateFrameData()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );

            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposed ) {
                // Dispose managed resource.
                if ( disposing ) {
                }

                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                if ( ColorFrame != null ) {
                    ColorFrame.Dispose();
                    ColorFrame = null;
                }

                if ( DepthFrame != null ) {
                    DepthFrame.Dispose();
                    DepthFrame = null;
                }

                if ( SkeletonFrame != null ) {
                    SkeletonFrame.Dispose();
                    SkeletonFrame = null;
                }
            }

            disposed = true;
        }
    }
}
