using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_VisualGestureBuilder
{
    public class ResultContinuous : Result
    {
        /* Attributi */
        // Valore di progressione della gesture
        public float progress { get; private set; }

        /* Costruttore */
        public ResultContinuous(float progress, TimeSpan timeSpan)
        {
            this.progress = progress;
            this.timeSpan = timeSpan;
        }
    }
}
