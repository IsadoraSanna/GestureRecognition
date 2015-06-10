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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class Djestit_Skeleton
    {
        /*
        internal bool close(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                //
                SkeletonToken skeletonToken = (SkeletonToken)token;
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
                //JointInformation jiNew = skeletonToken.skeleton.getJointInformation(JointType.HandRight);
                // 
                //JointInformation jiOld = skeletonToken.sOld.getJointInformation(JointType.HandRight);
                //
                if (skeletonToken.skeleton.rightHandStatus == HandState.Closed)// && skeletonToken.sOld.rightHandStatus == HandState.Closed && skeletonToken.skeleton.positionX > skeletonToken.sOld.positionX)
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
                SkeletonToken skeletonToken = (SkeletonToken)token;
                if (skeletonToken.skeleton.rightHandStatus == HandState.Open)
                    return true;
                else
                    return false;
            }
            return false;
        }
        void PanX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Eseguito gesto PanX");
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
            Debug.WriteLine("Ho la mano destra aperta.");
        }

        [TestMethod]
        public void PanX()
        {
            // Sensore
            Sensor sensor;

            // Espressione
            // Close
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "Start";
            termx1.accepts = close;
            termx1.Complete += Close;
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "Move";
            termx2.accepts = moveX;
            termx2.Complete += MoveX;
            // Open
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "End";
            termx3.accepts = open;
            termx3.Complete += Open;
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
            // Assoccio l'espressione panX al sensor
            sensor = new Sensor(panX, 5);

            /// Simulazione Gesti
            // Simulo 1 gesto di start
            Skeleton sStart = new Skeleton(0, HandState.Closed, 0.0f);
            SkeletonToken tStart = (SkeletonToken)sensor.generateToken(TypeToken.Start, sStart);
            // E lo sparo al motorino
            sensor.root.fire(tStart);
            // Simulo 20 gesti di move
            for(int i = 0; i < 150; i++)
            {
                Skeleton sMove = null;
                /*if (i == 140)
                    sMove = new Skeleton(0, HandState.Closed, (0.0f - 1000f));
                else*
                if (i == 50) 
                    i = 51;
                sMove = new Skeleton(0, HandState.Closed, (1f + i));
                    

                // Creo il gesto
                SkeletonToken tMove = (SkeletonToken)sensor.generateToken(TypeToken.Move, sMove);
                // E lo sparo
                sensor.root.fire(tMove);
            }
            // Simulo 1 gesto di end
            Skeleton sEnd = new Skeleton(0, HandState.Open, 22.0f);
            SkeletonToken tEnd = (SkeletonToken)sensor.generateToken(TypeToken.Move, sEnd);
            // E lo sparo al motorino
            sensor.root.fire(tEnd);

            /// Controllo lo stato dell'espressione
            Assert.IsTrue(sensor.root.state == expressionState.Complete, "Pan X eseguita");
        }*/
    }
}
