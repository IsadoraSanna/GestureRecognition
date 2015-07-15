using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Djestit Kinect
using RecognitionGestureFeed_Universal.Gesture.Kinect_Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// Recognition
using RecognitionGestureFeed_Universal.Recognition;
// Feedback
using RecognitionGestureFeed_Universal.Feed.FeedBack;
// Feedback Handler/Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper;
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
                float positionNewX = skeletonToken.positionX;
                float positionNewY = skeletonToken.positionY;
                //
                List<float> listConfidenceX = new List<float>();
                List<float> listConfidenceY = new List<float>();
                // Calcolo la differenza lungo l'asse X e l'asse Y
                foreach (Skeleton sOld in skeletonToken.precSkeletons)
                {
                    // Preleva dal penultimo scheletro il JointInformation riguardante la mano
                    float positionOldX = sOld.handRightPositionX;
                    float positionOldY = sOld.handRightPositionY;
                    listConfidenceX.Add(Math.Abs(positionNewX - positionOldX));
                    listConfidenceY.Add(Math.Abs(positionNewY - positionOldY));
                }
                //
                if (listConfidenceX.Average() > listConfidenceY.Average())
                    return true;
                else
                    return false;
            }
            return false;
        }
        internal bool moveY(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken skeletonToken = (SkeletonToken)token;
                // Controlla se la mano destra è effettivamente chiusa e se c'è stato un qualche movimento (anche impercettibile)
                // Preleva dall'ultimo scheletro il JointInformation riguardante la mano
                float positionNewX = skeletonToken.positionX;
                float positionNewY = skeletonToken.positionY;
                //
                List<float> listConfidenceX = new List<float>();
                List<float> listConfidenceY = new List<float>();

                // Calcolo la differenza lungo l'asse X e l'asse Y
                foreach (Skeleton sOld in skeletonToken.precSkeletons)
                {
                    // Preleva dal penultimo scheletro il JointInformation riguardante la mano
                    float positionOldX = sOld.handRightPositionX;
                    float positionOldY = sOld.handRightPositionY;
                    listConfidenceX.Add(Math.Abs(positionNewX - positionOldX));
                    listConfidenceY.Add(Math.Abs(positionNewY - positionOldY));
                }
                if (listConfidenceX.Average() < listConfidenceY.Average())
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
            Debug.WriteLine("Eseguito gesto PanY");
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
            SkeletonSensor sensor;
            
            // Espressione
            // Close
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "Start";
            termx1.accepts = close;
            termx1.name = "GroundTerm CloseX";
            //termx1.Complete += Close;
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "Move";
            termx2.accepts = moveX;
            termx2.name = "GroundTerm MoveX";
            //termx2.Complete += MoveX;
            // Open
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "End";
            termx3.accepts = open;
            termx3.name = "GroundTerm OpenX";
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
            panX.name = "PanX";

            /* Pan Asse Y */
            // Close
            GroundTerm termy1 = new GroundTerm();
            termy1.type = "Start";
            termy1.accepts = close;
            termy1.name = "GroundTerm CloseY";
            //termy1.Complete += Close;
            // Move
            GroundTerm termy2 = new GroundTerm();
            termy2.type = "Move";
            termy2.accepts = moveY;
            termy2.name = "GroundTerm MoveY";
            //termy2.Complete += Move;
            // Open
            GroundTerm termy3 = new GroundTerm();
            termy3.type = "End";
            termy3.accepts = open;
            termy3.name = "GroundTerm OpenY";
            //termy3.Complete += Open;
            Iterative iterativey = new Iterative(termy2);
            List<Term> listTermy = new List<Term>();
            listTermy.Add(iterativey);
            listTermy.Add(termy3);
            Disabling disablingy = new Disabling(listTermy);
            List<Term> listTermy2 = new List<Term>();
            listTermy2.Add(termy1);
            listTermy2.Add(disablingy);
            Sequence panY = new Sequence(listTermy2);
            panY.Complete += PanY;
            panY.name = "PanY";
            //
            List<Term> listTerm = new List<Term>();
            listTerm.Add(panX);
            listTerm.Add(panY);
            Choice choice = new Choice(listTerm);
            // Assoccio l'espressione panX al sensor
            sensor = new SkeletonSensor(choice, 5);
            // Creo l'albero dei feedback
            Feedback tree = new Feedback(choice);
            // Definisco Modifies & Handler per le due gesture
            // PanX
            Modifies a = new Modifies("a", 0);
            Modifies b = new Modifies("b", 1);
            Modifies c = new Modifies("c", 2);
            List<Modifies> listModifiesX = new List<Modifies>();
            listModifiesX.Add(a);
            listModifiesX.Add(b);
            listModifiesX.Add(c);
            Handler handlerPanX = new Handler("PanX", listModifiesX, this.PanX);
            // PanY
            Modifies d = new Modifies("d", 1);
            Modifies e = new Modifies("e", 2);
            List<Modifies> listModifiesY = new List<Modifies>();
            listModifiesY.Add(a);
            listModifiesY.Add(d);
            listModifiesY.Add(e);
            Handler handlerPanY = new Handler("PanY", listModifiesY, this.PanY);

            // Associo gli handler alle due Gesture
            tree.tree.children[0].handlerGesture = (Handler)handlerPanX.Clone();
            tree.tree.children[1].handlerGesture = (Handler)handlerPanY.Clone();

            /// Simulazione Gesti
            // Simulo 1 gesto di start
            Skeleton sStart = new Skeleton(0, HandState.Closed, 0.0f, 0.0f);
            SkeletonToken tStart = (SkeletonToken)sensor.generateToken(TypeToken.Start, sStart);
            // E lo sparo al motorino
            sensor.root.fire(tStart);
            tree.tree.print();

            // Simulo 20 gesti di move
            for(int i = 0; i < 10; i++)
            {
                Skeleton sMove = null;
                SkeletonToken tMove = null;
                /*if (i == 1)
                {
                    sMove = new Skeleton(0, HandState.Closed, 0.0f, 10000f);
                    tMove = (SkeletonToken)sensor.generateToken(TypeToken.Move, sMove);
                }
                else*/
                //{
                    /*if (i == 140)
                        sMove = new Skeleton(0, HandState.Closed, (0.0f - 1000f), 0.0f);
                    if (i == 50)
                    {
                        i = 51;
                        sMove = new Skeleton(0, HandState.Closed, (1f + i), 0.0f);
                    }*/
                    // Creo lo scheletro
                    sMove = new Skeleton(0, HandState.Closed, (1f + i), 0.0f);
                    // Creo il gesto
                    tMove = (SkeletonToken)sensor.generateToken(TypeToken.Move, sMove);
                //}
                // E lo sparo
                sensor.root.fire(tMove);
            }
            tree.tree.print();

            // Simulo 1 gesto di end
            Skeleton sEnd = new Skeleton(0, HandState.Open, 22.0f, 0.0f);
            SkeletonToken tEnd = (SkeletonToken)sensor.generateToken(TypeToken.Move, sEnd);
            // E lo sparo al motorino
            sensor.root.fire(tEnd);
            tree.tree.print();
        }
    }
}
