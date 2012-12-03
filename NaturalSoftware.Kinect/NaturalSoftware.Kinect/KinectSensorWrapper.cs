using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace NaturalSoftware.Kinect
{
    public class KinectSensorWrapper
    {
        public KinectSensor Sensor
        {
            get;
            private set;
        }

        public bool IsRunning
        {
            get
            {
                if ( Sensor == null ) {
                    return false;
                }

                return Sensor.IsRunning;
            }
        }

        public KinectSensorWrapper()
            : this( GetFirstActiveSensor() )
        {
        }

        public KinectSensorWrapper( KinectSensor kinect )
        {
            if ( kinect == null ) {
                throw new Exception( "Kinectが接続されていないか、準備ができていません" );
            }

            Sensor = kinect;

            // デフォルトの設定
            ColorImageFormat = ColorImageFormat.RgbResolution640x480Fps30;
            DepthImageFormat = DepthImageFormat.Resolution640x480Fps30;
            TransformSmoothParameters = new TransformSmoothParameters()
            {
                Smoothing = 0.7f,
                Correction = 0.3f,
                Prediction = 0.4f,
                JitterRadius = 0.10f,
                MaxDeviationRadius = 0.5f,
            };
        }

        public ColorImageFormat ColorImageFormat
        {
            get;
            set;
        }

        public DepthImageFormat DepthImageFormat
        {
            get;
            set;
        }

        public TransformSmoothParameters TransformSmoothParameters
        {
            get;
            set;
        }

        public void Start()
        {
            if ( Sensor == null && Sensor.IsRunning ) {
                return;
            }

            Sensor.ColorStream.Enable( ColorImageFormat );
            Sensor.DepthStream.Enable( DepthImageFormat );
            Sensor.SkeletonStream.Enable( TransformSmoothParameters );

            Sensor.AllFramesReady += Sensor_AllFramesReady;

            Sensor.Start();
        }

        public void Stop()
        {
            if ( Sensor != null ) {
                Sensor.Stop();
                Sensor.AllFramesReady -= Sensor_AllFramesReady;
            }
        }

        public delegate void AllFrameReadyEvent( KinectUpdateFrameData e );
        public event AllFrameReadyEvent AllFrameReady;

        void Sensor_AllFramesReady( object sender, AllFramesReadyEventArgs e )
        {
            using ( KinectUpdateFrameData data = new KinectUpdateFrameData( e ) ) {
                if ( AllFrameReady != null ) {
                    AllFrameReady( data );
                }
            }
        }

        public static KinectSensor GetFirstActiveSensor()
        {
            return KinectSensor.KinectSensors.FirstOrDefault( k =>
            {
                return k.Status == KinectStatus.Connected;
            } );
        }
    }
}
