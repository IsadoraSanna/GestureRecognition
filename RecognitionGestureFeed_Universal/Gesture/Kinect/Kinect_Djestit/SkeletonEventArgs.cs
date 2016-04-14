using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_Djestit
{
    public class SkeletonEventArgs : EventArgs
    {
        public readonly SkeletonSensor sensor;

        public SkeletonEventArgs(SkeletonSensor sensor)
        {
            this.sensor = sensor;
        }
    }
}
