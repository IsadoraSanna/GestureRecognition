using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Leap Motion
using Leap;

namespace RecognitionGestureFeed_Universal.Recognition.Leap
{
    public class InitLeap
    {
        /* Attributi */
        Controller controller;

        /* Costruttore */
        public InitLeap()
        {
            controller = new Controller();
            AcquisitionManagerLeap leapManager = new AcquisitionManagerLeap();
            controller.AddListener(leapManager);
        }
    }
}
