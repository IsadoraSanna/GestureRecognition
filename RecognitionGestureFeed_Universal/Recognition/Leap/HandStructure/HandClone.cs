using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Leap
using Leap;

namespace RecognitionGestureFeed_Universal.Recognition.Leap.HandStructure
{
    public class HandClone : Hand, ICloneable
    {
        /* Attributi */

        /* Costruttore */

        /* Metodi */
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
