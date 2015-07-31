using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Gesture.Leap.Leap_Djestit
{
    class LeapEventArgs : EventArgs
    {
        public readonly LeapSensor sensor;

        public LeapEventArgs(LeapSensor sensor)
        {
            this.sensor = sensor;
        }
    }
}
