using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Recognition.FrameDataManager
{
    internal class BodyIndexData : FrameData
    {
        /* Attributi */
        public byte[] frameData { get; private set; }

        /* Costruttore */
        public BodyIndexData(FrameDescription frameDescription) : base(frameDescription)
        {
            this.frameData = null;
        }
            
        /* Metodi */
        public void update(BodyIndexFrame frame)
        {
            frame.CopyFrameDataToArray(this.frameData);
        }
    }
}
