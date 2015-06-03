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
    public class MoveX : GroundTerm
    {

        public MoveX() : base()
        {

        }
        public bool _accepts(Skeleton newToken, Skeleton oldToken)
        {
            if (newToken.getJointInformation(JointType.HandLeft).getPosition().X > oldToken.getJointInformation(JointType.HandLeft).getPosition().X)
                return true;
            else
                return false;
        }
    }
}
