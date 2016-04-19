using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_VisualGestureBuilder
{
    public abstract class GestureProgress
    {
        /* Attributi */
        // Id scheletro a cui è associato
        public ulong bodyId { get; protected set; }
        public int skeletonId { get; protected set; }
        // Frame milliseconds rate
        protected const float frequencyRate = 35;
    }
}
