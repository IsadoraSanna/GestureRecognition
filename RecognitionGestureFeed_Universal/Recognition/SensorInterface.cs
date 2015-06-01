using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Djestit Kinect
using RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// Kinect
using Microsoft.Kinect;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public class SensorInterface
    {
        // Attributi
        public JointSensor jointSensor;

        public SensorInterface(AcquisitionManager am)
        {
            jointSensor = new JointSensor(3);
            //+= updateJoint;
        }

        public void updateJoint(AcquisitionManager am)
        {
            foreach (Skeleton sk in am.skeletonList)
            {
                if (sk.getStatus())
                {
                    foreach (JointInformation ji in sk.getListJointInformation())
                    {
                        if (ji.getStatus() == TrackingState.Tracked)
                        {
                            if (jointSensor.checkJoint(ji.getType() + 1))
                            {
                                jointSensor.generateToken(TypeToken.Move, ji);
                            }
                            else
                            {
                                jointSensor.generateToken(TypeToken.Start, ji);
                            }
                        }
                        //else if(jointSensor.checkJoint(ji.getType()))
                        //rimuovi token
                    }
                    Debug.WriteLine(jointSensor.sequence.moves.Count);
                    return;
                }
            }
        }
        public void removeAllJoints(AcquisitionManager am)
        {
            foreach(JointInformation ji in am.skeletonList[0].getListJointInformation())
            {
                jointSensor.generateToken(TypeToken.End, ji);
            }
        }
    }
}
