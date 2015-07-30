using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Leap Motion
using Leap;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public class InitLeap
    {
        /* Attributi */
        Controller controller;

        /* Costruttore */
        public InitLeap()
        {
            AcquisitionManagerLeap leapManager = new AcquisitionManagerLeap();
            controller = new Controller();
            controller.AddListener(leapManager);
        }
    }
}
