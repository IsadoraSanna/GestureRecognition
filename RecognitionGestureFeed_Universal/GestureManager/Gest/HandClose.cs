using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;

namespace RecognitionGestureFeed_Universal.GestureManager.Gest
{
    class HandClose : GroundTerm
    {
        public HandClose() : base()
        {

        }
        public bool _accepts(Skeleton token)
        {
            if (token.leftHandStatus != HandState.Closed)
                return false;
            else
                return true;
        }
    }
}
