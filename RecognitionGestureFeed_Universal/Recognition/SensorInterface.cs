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
// Likelihood-Handler-Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood;
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Kinect
using Microsoft.Kinect;
// Debug
using System.Diagnostics;
// Feedback
using RecognitionGestureFeed_Universal.Feed.FeedBack;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public class SensorInterface
    {
        // Attributi
        internal SkeletonSensor sensor;
        // Prova
        internal Feedback tree;

        public SensorInterface(AcquisitionManager am, Term expression)
        {
            this.sensor = new SkeletonSensor(expression, 3);
            am.SkeletonsFrameManaged += updateSkeleton;
        }

        public void updateSkeleton(Skeleton[] skeletonList)
        {
            // Per ogni scheletro rilevato avvio il motorino
            foreach (Skeleton skeleton in skeletonList)
            {
                // Creo uno skeleton token
                SkeletonToken token = null;
                // Determino il tipo (Start, Move o End) e ne creo il token, e quindo lo genero
                if (skeleton.getStatus())
                {
                    if (sensor.checkSkeleton(skeleton.getIdSkeleton()))
                        token = (SkeletonToken)sensor.generateToken(TypeToken.Move, skeleton);
                    else
                        token = (SkeletonToken)sensor.generateToken(TypeToken.Start, skeleton);
                }
                else if (sensor.checkSkeleton(skeleton.getIdSkeleton()))
                {
                    token = (SkeletonToken)sensor.generateToken(TypeToken.End, skeleton);
                }

                // Se è stato creato un token, lo sparo al motore
                if (token != null)
                {
                    if (token.type != TypeToken.End)
                        this.sensor.root.fire(token);
                }

                // Se lo stato della choice è in error o complete allora lo riazzero
                if (this.sensor.root.state == expressionState.Error || this.sensor.root.state == expressionState.Complete)
                    this.sensor.root.reset();
            }
            tree.tree.print();
        }

        #region Prove
        public SensorInterface(Term term)
        {
            this.sensor = new SkeletonSensor(term, 5);
        }

        public SensorInterface(AcquisitionManager am)
        {
            /* Pan Asse X */
            // Close
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "Start";
            termx1.accepts = close;
            termx1.name = "GroundTerm CloseX";
            termx1.likelihood = new Likelihood(0.01f);
            //termx1.Complete += Close;
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "Move";
            termx2.accepts = moveX;
            termx2.name = "GroundTerm MoveX";
            termx2.likelihood = new Likelihood(0.02f);
            //termx2.Complete += Move;
            // Open
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "End";
            termx3.accepts = open;
            termx3.name = "GroundTerm OpenX";
            termx3.likelihood = new Likelihood(0.01f);
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
            // Handler PanX
            panX.handler = new Handler(this.PanX, "PanX");

            /* Pan Asse Y */
            // Close
            GroundTerm termy1 = new GroundTerm();
            termy1.type = "Start";
            termy1.accepts = close;
            termy1.name = "GroundTerm CloseY";
            termy1.likelihood = new Likelihood(0.01f);
            //termy1.Complete += Close;
            // Move
            GroundTerm termy2 = new GroundTerm();
            termy2.type = "Move";
            termy2.accepts = moveY;
            termy2.name = "GroundTerm MoveY";
            termy2.likelihood = new Likelihood(0.3f);
            //termy2.Complete += Move;
            // Open
            GroundTerm termy3 = new GroundTerm();
            termy3.type = "End";
            termy3.accepts = open;
            termy3.name = "GroundTerm OpenY";
            termy3.likelihood = new Likelihood(0.01f);
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
            // PanY
            panY.handler = new Handler(this.PanY, "PanY");

            // Choice
            List<Term> listTerm = new List<Term>();
            listTerm.Add(panX);
            listTerm.Add(panY);
            Choice choice = new Choice(listTerm);
            // Assoccio l'espressione panX al sensor
            this.sensor = new SkeletonSensor(choice, 5);
            am.SkeletonsFrameManaged += updateSkeleton;
            // Creo l'albero dei feedback
            this.tree = new Feedback(choice);
        }

        // Example
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
                SkeletonToken skeletonToken = (SkeletonToken)token;
                // Controlla se la mano destra è effettivamente chiusa e se c'è stato un qualche movimento (anche impercettibile)
                // Preleva dall'ultimo scheletro il JointInformation riguardante la mano
                JointInformation jNew = skeletonToken.skeleton.getJointInformation(JointType.HandRight);
                //Debug.WriteLine("mano X: " +jNew.position.X);
                List<float> listConfidenceX = new List<float>();
                List<float> listConfidenceY = new List<float>();

                // Calcolo la differenza lungo l'asse X e l'asse Y
                foreach (Skeleton sOld in skeletonToken.precSkeletons)
                {
                    // Preleva dal penultimo scheletro il JointInformation riguardante la mano
                    JointInformation jOld = sOld.getJointInformation(JointType.HandRight);
                    listConfidenceX.Add(Math.Abs(jNew.position.X - jOld.position.X));
                    listConfidenceY.Add(Math.Abs(jNew.position.Y - jOld.position.Y));
                }

                if (listConfidenceX.Average() > listConfidenceY.Average()
                    && Math.Abs(jNew.position.X - skeletonToken.precSkeletons[0].getJointInformation(JointType.HandRight).position.X) <= 0.5f
                    && Math.Abs(jNew.position.Y - skeletonToken.precSkeletons[0].getJointInformation(JointType.HandRight).position.Y) <= 0.1f
                    )
                {
                    return true;
                }
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
                JointInformation jNew = skeletonToken.skeleton.getJointInformation(JointType.HandRight);

                List<float> listConfidenceX = new List<float>();
                List<float> listConfidenceY = new List<float>();

                // Calcolo la differenza lungo l'asse X e l'asse Y
                foreach (Skeleton sOld in skeletonToken.precSkeletons)
                {
                    // Preleva dal penultimo scheletro il JointInformation riguardante la mano
                    JointInformation jOld = sOld.getJointInformation(JointType.HandRight);
                    listConfidenceX.Add(Math.Abs(jNew.position.X - jOld.position.X));
                    listConfidenceY.Add(Math.Abs(jNew.position.Y - jOld.position.Y));
                }

                if (listConfidenceY.Average() > listConfidenceX.Average()
                     && Math.Abs(jNew.position.Y - skeletonToken.precSkeletons[0].getJointInformation(JointType.HandRight).position.Y) <= 0.5f
                     && Math.Abs(jNew.position.X - skeletonToken.precSkeletons[0].getJointInformation(JointType.HandRight).position.X) <= 0.1f
                    )
                {
                    return true;
                }
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

        [Modifies("a", 0), Modifies("b", 1), Modifies("c", 2)]
        void PanX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Eseguito gesto PanX");
        }

        [Modifies("a", 0), Modifies("d", 1), Modifies("e", 2)]
        void PanY(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Eseguito gesto PanY");
        }
        void Close(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho la mano destra chiusa.");
        }
        void Move(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho mosso la mano destra.");
        }
        void Open(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Ho la mano destra aperta.");
        }
        #endregion

    }
}
