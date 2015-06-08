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
        public Sensor sensor;

        internal bool close(Token token)
        {
           
            if (token.GetType() == typeof(SkeletonToken))
            {
                //
                SkeletonToken skeletonToken = (SkeletonToken)token;
                //
                //JointInformation i = skeletonToken.skeleton.getJointInformation(JointType.HandRight);
                //Debug.WriteLine(skeletonToken.skeleton.rightHandStatus);
                if (skeletonToken.skeleton.rightHandStatus == HandState.Closed)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;

        }
        internal bool moveX(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                // 
                SkeletonToken skeletonToken = (SkeletonToken)token;
                //
                JointInformation jiNew = skeletonToken.skeleton.getJointInformation(JointType.HandRight);
                // 
                JointInformation jiOld = skeletonToken.sOld.getJointInformation(JointType.HandRight);
                float confidence = Math.Abs(jiNew.position.X - jiOld.position.X);
                //
                if (skeletonToken.skeleton.rightHandStatus == HandState.Closed && (confidence < 0.2))
                    return true;
                else
                    return false;
            }
            return false;
        }
        internal bool open(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                // 
                SkeletonToken skeletonToken = (SkeletonToken)token;
                // 
                JointInformation jiOld = skeletonToken.sOld.getJointInformation(JointType.HandRight);
                if (skeletonToken.skeleton.rightHandStatus == HandState.Open)// && skeletonToken.sOld.rightHandStatus == HandState.Closed)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public SensorInterface(AcquisitionManager am)
        {
            /* Pan Asse x */
            // Close
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "Start";
            termx1.accepts = close;
            //termx1.Complete += Close;
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "Move";
            termx2.accepts = moveX;
            //termx2.Complete += MoveX;
            // Open
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "End";
            termx3.accepts = open;
            //termx3.Complete += Open;
            Iterative iterativex = new Iterative(termx2);
            List<Term> listTermx = new List<Term>();
            listTermx.Add(iterativex);
            listTermx.Add(termx3);
            Disabling disablingx = new Disabling(listTermx);
            List<Term> listTerm2 = new List<Term>();
            listTerm2.Add(termx1);
            listTerm2.Add(disablingx);
            Sequence panX = new Sequence(listTerm2);
            panX.Complete += PanX;

            this.sensor = new Sensor(panX, 3);
            //am.OnSkeletonFrameManaged+= updateJoint;
        }

        public void updateSkeleton(AcquisitionManager am)
        {
            foreach(Skeleton skeleton in am.skeletonList)
            {
                SkeletonToken token = null;
                if (skeleton.getStatus())
                {
                    if (sensor.checkSkeleton(skeleton.getIdSkeleton()))
                    {
                        token = (SkeletonToken)sensor.generateToken(TypeToken.Move, skeleton);
                    }
                    else
                    {
                        //Debug.WriteLine("Start");
                        token = (SkeletonToken)sensor.generateToken(TypeToken.Start, skeleton);
                    }
                }
                else if (sensor.checkSkeleton(skeleton.getIdSkeleton()))
                {
                    token = (SkeletonToken)sensor.generateToken(TypeToken.End, skeleton);
                }

                if (token != null)
                    this.sensor.root.fire(token);

                if (this.sensor.root.state == expressionState.Error)
                    this.sensor.root.reset();
            }
        }

        /*
        public void removeAllJoints(AcquisitionManager am)
        {
            foreach(JointInformation ji in am.skeletonList[0].getListJointInformation())
            {
                jointSensor.generateToken(TypeToken.End, ji);
            }
        }*/
        void PanX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("PorcamadonnaX");
        }
        void PanY(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("PorcoddioY");
        }
        void Close(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho la mano destra chiusa.");
        }
        void MoveX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho mosso la mano destra chiusa.");
        }
        void Open(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho la mano destra chiusa.");
        }

    }
}
