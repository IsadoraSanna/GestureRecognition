using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unica.Djestit.Audio
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
