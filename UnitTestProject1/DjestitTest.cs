﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Debug
using System.Diagnostics;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
using RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_Djestit;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;
// Kinect
using Microsoft.Kinect;
// FeedBack
using RecognitionGestureFeed_Universal.Feed.FeedBack;
// Handler
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;
// Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Test
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class DjestitTest
    {
        [TestMethod]
        [Modifies("A", 0)]
        public void TestMethod1()
{
            /* Pan Asse X */
            // Close
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "Start";
            termx1.accepts = close;
            termx1.Complete += Close;
            //termx1.handler = new Handler(Close, termx1);
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "Move";
            termx2.accepts = moveX;
            termx2.Complete += Move;
            //termx2.handler = new Handler(Move, termx2);
            // Open
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "End";
            termx3.accepts = open;
            termx3.Complete += Open;
            //termx3.handler = new Handler(Open, termx3);
            // Iterative Move
            Iterative iterativex = new Iterative(termx2);
            List<Term> listTermx = new List<Term>();
            listTermx.Add(iterativex);
            listTermx.Add(termx3);
            // Disabling Move - End
            Disabling disablingx = new Disabling(listTermx);
            List<Term> listTermx2 = new List<Term>();
            listTermx2.Add(termx1);
            listTermx2.Add(disablingx);
            // Sequence Start - Move - End
            Sequence panX = new Sequence(listTermx2);
            panX.Complete += PanX;
            //panX.handler = new Handler(PanX, panX);

            //panX.setErrorTollerance();
            /* Pan Asse Y */
            // Close
            GroundTerm termy1 = new GroundTerm();
            termy1.type = "Start";
            termy1.accepts = close;
            termy1.Complete += Close;
            //termy1.handler = new Handler(Close, termy1);
            // Move
            GroundTerm termy2 = new GroundTerm();
            termy2.type = "Move";
            termy2.accepts = moveY;
            termy2.Complete += Move;
            //termy2.handler = new Handler(Move, termy2);
            // Open
            GroundTerm termy3 = new GroundTerm();
            termy3.type = "End";
            termy3.accepts = open;
            termy3.Complete += Open;
            //termy3.handler = new Handler(Open, termy3);
            // Iterative Move
            Iterative iterativey = new Iterative(termy2);
            List<Term> listTermy = new List<Term>();
            listTermy.Add(iterativey);
            listTermy.Add(termy3);
            // Disabling Move - End
            Disabling disablingy = new Disabling(listTermy);
            List<Term> listTermy2 = new List<Term>();
            listTermy2.Add(termy1);
            listTermy2.Add(disablingy);
            // Sequence Start - Move - End
            Sequence panY = new Sequence(listTermy2);
            panY.Complete += PanY;
            //panY.handler = new Handler(PanY, panY);
            
            // Gesture Database
            List<Term> listTerm = new List<Term>();
            listTerm.Add(panX);
            listTerm.Add(panY);
            Choice choice = new Choice(listTerm);
            
            // Crea Albero
            //Feedback feedback = new Feedback(choice);
            panY.getErrorTolerance().errorDetect();
            panY.getErrorTolerance().reset();
        }

        // Example
        internal bool close(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken skeletonToken = (SkeletonToken)token;
                // La gesture inizia se l'utente chiude la mano destra
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
                //Debug.WriteLine(listConfidenceX.Average() + " - " + listConfidenceY.Average());
                if (skeletonToken.skeleton.rightHandStatus == HandState.Closed && listConfidenceX.Average() > listConfidenceY.Average())
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
                if (skeletonToken.skeleton.rightHandStatus == HandState.Closed && listConfidenceX.Average() < listConfidenceY.Average())
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
                // La gesture termina quando l'utente apre la mano destra
                if (skeletonToken.skeleton.rightHandStatus == HandState.Open)
                    return true;
                else
                    return false;
            }
            return false;
        }

        void PanX(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Pan X");
        }
        [Modifies("a", 0)]
        void PanY(object sender, GestureEventArgs t)
        {
            Debug.WriteLine("Pan Y");
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
    }
}


