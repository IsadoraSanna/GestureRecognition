using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecognitionGestureFeed_Universal;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
using RecognitionGestureFeed_Universal.Djestit.ErrorToleranceManager;
// Gesture
using RecognitionGestureFeed_Universal.Gesture;
// Djestit Kinect
using RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;
// Recognition
using RecognitionGestureFeed_Universal.Recognition;
// Feedback
using RecognitionGestureFeed_Universal.Feed.FeedBack;
// Feedback Handler/Modifies/Likelihood
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood;
// Kinect
using Microsoft.Kinect;
// Debug
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    public enum Operation
    {
        Nothing = 0,
        PanX = 1,
        PanY = 2
    }

    [TestClass]
    public class Djestit_Skeleton
    {
        int numProve;

        /// <summary>
        /// Start Gesture
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Move Pan X
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal bool moveX(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                // Prendo lo skeletonToken
                SkeletonToken skeletonToken = (SkeletonToken)token;

                if ((numProve == 2) && skeletonToken.type == TypeToken.Move)
                    return false;
                else
                    return true;
                    /*//
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
                    if (listConfidenceX.Average() > listConfidenceY.Average())
                        return true;
                    else
                        return false;
                }
                return true;*/
            }
            return false;
        }
        /// <summary>
        /// Move Pan Y
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal bool moveY(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken skeletonToken = (SkeletonToken)token;
                if(skeletonToken.type == TypeToken.Move)
                    return true;
                /*
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
                    return false;*/
            }
            return false;
        }

        /// <summary>
        /// End Gesture
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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

        [Modifies("Operation", Operation.Nothing, Operation.PanX)]
        void PanX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Eseguito gesto PanX " + numProve);
        }

        [Modifies("Operation", Operation.Nothing, Operation.PanY)]
        void PanY(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Eseguito gesto PanY " + numProve);
        }
        void Close(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho la mano destra chiusa. " + numProve);
        }
        void MoveX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho mosso la mano destra chiusa X. " + numProve);
        }
        void MoveY(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho mosso la mano destra chiusa Y. " + numProve);
        }
        void Open(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho la mano destra aperta. " + numProve);
        }

        [TestMethod]
        public void PanX()
        {
            List<Modifies> porcoddio = new List<Modifies>();
            porcoddio.Add(new Modifies("Operation", Operation.Nothing));

            // Sensore
            SkeletonSensor sensor;

            /// Gesture
            // Start
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "Start";
            termx1.accepts = close;
            termx1.name = "GroundTerm CloseX";
            termx1.likelihood = 0.5f;//new Likelihood(0.01f);
            termx1.setHandler(Close, porcoddio);
            //termx1.Complete += Close;
            //termx1.handler = new Handler(Close, termx1);
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "Move";
            termx2.accepts = moveX;
            termx2.name = "GroundTerm MoveX";
            termx2.likelihood = 0.75f;//new Likelihood(0.02f);
            termx2.setHandler(MoveX, porcoddio);
            //termx2.Complete += MoveX;
            //termx2.handler = new Handler(MoveX, termx2);
            // End
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "End";
            termx3.accepts = open;
            termx3.name = "GroundTerm OpenX";
            termx3.likelihood = 0.5f;//new Likelihood(0.01f);
            termx3.setHandler(Open, porcoddio);
            //termx3.Complete += Open;
            //termx3.handler = new Handler(Open, termx3);
            // Iterative Move
            Iterative iterativex = new Iterative(termx2);
            iterativex.likelihood = ComputeLikelihood.indipendentEvents(iterativex);//new Likelihood(iterativex, ProbabilityType.IndipendentEvents);
            // Disabling Move - End
            List<Term> listTermx = new List<Term>();
            listTermx.Add(iterativex);
            listTermx.Add(termx3);
            Disabling disablingx = new Disabling(listTermx);
            disablingx.likelihood = ComputeLikelihood.indipendentEvents(disablingx); //new Likelihood(disablingx, ProbabilityType.IndipendentEvents);
            // Sequence
            List<Term> listTerm2 = new List<Term>();
            listTerm2.Add(termx1);
            listTerm2.Add(disablingx);
            Sequence panX = new Sequence(listTerm2);
            panX.likelihood = ComputeLikelihood.indipendentEvents(panX); //new Likelihood(panX, ProbabilityType.IndipendentEvents);
            panX.name = "PanX";
            panX.setHandler(PanX, porcoddio);
            //panX.Complete += PanX;
            //panX.handler = new Handler(this.PanX, panX);// Handler
            panX.setErrorTolerance(new ErrorTolerance());

            /* Pan Asse Y */
            // Start
            GroundTerm termy1 = new GroundTerm();
            termy1.type = "Start";
            termy1.accepts = close;
            termy1.name = "GroundTerm CloseY";
            termy1.likelihood = 0.5f;//new Likelihood(0.01f);
            termy1.setHandler(Close, porcoddio);
            //termy1.Complete += Close;
            //termy1.handler = new Handler(Close, termy1, porcoddio);
            // Move
            GroundTerm termy2 = new GroundTerm();
            termy2.type = "Move";
            termy2.accepts = moveY;
            termy2.name = "GroundTerm MoveY";
            termy2.likelihood = 0.75f;// new Likelihood(0.3f);
            termy2.setHandler(MoveY, porcoddio);
            //termy2.Complete += MoveY;
            //termy2.handler = new Handler(MoveY, termy2, porcoddio);
            // End
            GroundTerm termy3 = new GroundTerm();
            termy3.type = "End";
            termy3.accepts = open;
            termy3.name = "GroundTerm OpenY";
            termy3.likelihood = 0.5f;// new Likelihood(0.01f);
            termy3.setHandler(Open, porcoddio);
            //termy3.Complete += Open;
            //termy3.handler = new Handler(Open, termy3);
            // Iterative Move
            Iterative iterativey = new Iterative(termy2);
            iterativey.likelihood = ComputeLikelihood.indipendentEvents(iterativey); //new Likelihood(iterativey, ProbabilityType.IndipendentEvents);
            // Disabling Move - End
            List<Term> listTermy = new List<Term>();
            listTermy.Add(iterativey);
            listTermy.Add(termy3);
            Disabling disablingy = new Disabling(listTermy);
            disablingy.likelihood = ComputeLikelihood.indipendentEvents(disablingy); //new Likelihood(disablingy, ProbabilityType.IndipendentEvents);
            // Sequence
            List<Term> listTermy2 = new List<Term>();
            listTermy2.Add(termy1);
            listTermy2.Add(disablingy);
            Sequence panY = new Sequence(listTermy2);
            panY.likelihood = ComputeLikelihood.indipendentEvents(panY); //new Likelihood(panY, ProbabilityType.IndipendentEvents);
            panY.name = "PanY";
            panY.setErrorTolerance(new ErrorTolerance());
            panY.setHandler(PanY, porcoddio);
            //panY.Complete += PanY;
            //panY.handler = new Handler(this.PanY, panY);// Handler


            // Choice
            List<Term> listTerm = new List<Term>();
            listTerm.Add(panX);
            listTerm.Add(panY);
            Choice choice = new Choice(listTerm);
            // Assoccio l'espressione panX al sensor
            sensor = new SkeletonSensor(choice, 5);
            // Creo l'albero dei feedback
            Feedback tree = new Feedback(porcoddio, choice);
           
            /// Simulazione Gesti
            // Simulo 1 gesto di start
            //Skeleton sStart = new Skeleton(0, HandState.Closed, 0.0f, 0.0f);
            //SkeletonToken tStart = (SkeletonToken)sensor.generateToken(TypeToken.Start, sStart);
            // E lo sparo al motorino
            //sensor.root.fire(tStart);

            // Simulo 20 gesti di move
            for(int i = 0; i < 5; i++)
            {
                Skeleton sMove = null;
                SkeletonToken tMove = null;
                numProve++;
                //sMove = new Skeleton(0, HandState.Closed, 0.0f+i, 0.0f);
                tMove = (SkeletonToken)sensor.generateToken(TypeToken.Move, sMove);
                /*if (i == 1)
                {
                    sMove = new Skeleton(0, HandState.Closed, 0.1f, 0.0f);
                    tMove = (SkeletonToken)sensor.generateToken(TypeToken.Move, sMove);
                }
                else
                {
                    if (i == 140)
                        sMove = new Skeleton(0, HandState.Closed, (0.0f - 1000f), 0.0f);
                    if (i == 50)
                    {
                        i = 51;
                        sMove = new Skeleton(0, HandState.Closed, (1f + i), 0.0f);
                    }
                    // Creo lo scheletro
                    sMove = new Skeleton(0, HandState.Closed, (1f + i), 0.0f);
                    // Creo il gesto
                    tMove = (SkeletonToken)sensor.generateToken(TypeToken.Move, sMove);
                }*/
                // E lo sparo
                sensor.root.fire(tMove);
            }

            // Simulo 1 gesto di end
            //Skeleton sEnd = new Skeleton(0, HandState.Open, 22.0f, 0.0f);
            //SkeletonToken tEnd = (SkeletonToken)sensor.generateToken(TypeToken.Move, sEnd);
            // E lo sparo al motorino
            //sensor.root.fire(tEnd);
        }
    }
}
