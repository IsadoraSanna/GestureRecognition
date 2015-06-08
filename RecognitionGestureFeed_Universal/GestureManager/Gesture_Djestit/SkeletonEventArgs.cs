using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    public class SkeletonEventArgs : EventArgs
    {
        public readonly Sensor sensor;

        public SkeletonEventArgs(Sensor sensor)
        {
            this.sensor = sensor;
        }
    }
}
