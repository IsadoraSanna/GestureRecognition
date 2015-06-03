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
    public class MoveY : GroundTerm
    {
        public MoveY() : base()
        {

        }
        public bool _accepts(Skeleton newToken, Skeleton oldToken)
        {
            if (newToken.getJointInformation(JointType.HandLeft).getPosition().Y > oldToken.getJointInformation(JointType.HandLeft).getPosition().Y)
                return true;
            else
                return false;
        }
    }
}
