using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Gesture.Audio_Djestit
{
    public class AudioEventArgs : EventArgs
    {
        /* Attributi */
        public readonly AudioSensor sensor;

        /* Costruttore */
        public AudioEventArgs(AudioSensor sensor)
        {
            this.sensor = sensor;
        }
    }
}
