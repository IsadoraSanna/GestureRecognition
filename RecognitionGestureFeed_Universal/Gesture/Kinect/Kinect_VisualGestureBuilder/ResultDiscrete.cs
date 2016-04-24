using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unica.Djestit.Kinect2.VisualGestureBuilder
{
    public class ResultDiscrete : Result
    {
        /* Attributi */
        // Indica se la gesture è stata rilevata o meno
        public bool detected { get; protected set; }
        // Valore di confidenza associato al risultato di rilevamento
        public float confidence { get; protected set; }

        /* Costruttore */
        public ResultDiscrete(bool detected, float confidence, TimeSpan timeSpan)
        {
            this.detected = detected;
            this.confidence = confidence;
            this.timeSpan = timeSpan;
        }
    }
}
