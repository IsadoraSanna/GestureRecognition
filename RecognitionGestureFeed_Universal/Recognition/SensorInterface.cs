﻿using System;
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
        public Sensor jointSensor;

        internal bool close(SkeletonToken token)
        {
            if (token.skeleton.leftHandStatus == HandState.Closed)
                return true;
            else
                return false;
        }
        internal bool Move(SkeletonToken token)
        {
            if (token.skeleton.leftHandStatus == HandState.Closed)
                return true;
            else
                return false;
        }
        internal bool Open(SkeletonToken token)
        {
            if (token.skeleton.leftHandStatus == HandState.Closed)
                return true;
            else
                return false;
        }

        public SensorInterface(AcquisitionManager am)
        {
            /* Pan Asse x */
            // Close
            GroundTerm termx1 = new GroundTerm();
            termx1.type = "BodyStart";
            termx1.accepts = close;
            // Move
            GroundTerm termx2 = new GroundTerm();
            termx2.type = "BodyMove";
            // Open
            GroundTerm termx3 = new GroundTerm();
            termx3.type = "BodyEnd";
            Iterative iterativex = new Iterative(termx2);
            List<Term> listTermx = new List<Term>();
            listTermx.Add(iterativex);
            listTermx.Add(termx3);
            Disabling disablingx = new Disabling(listTermx);
            List<Term> listTerm2 = new List<Term>();
            listTerm2.Add(termx1);
            listTerm2.Add(disablingx);
            Sequence panX = new Sequence(listTerm2);

            /* Pan Asse Y */
            // Close
            GroundTerm termy1 = new GroundTerm();
            termy1.type = "BodyStart";
            // Move
            GroundTerm termy2 = new GroundTerm();
            termy2.type = "BodyMove";
            // Open
            GroundTerm termy3 = new GroundTerm();
            termy3.type = "BodyEnd";
            Iterative iterativey = new Iterative(termy2);
            List<Term> listTermy = new List<Term>();
            listTermy.Add(iterativey);
            listTermy.Add(termy3);
            Disabling disablingy = new Disabling(listTermy);
            List<Term> listTermy2 = new List<Term>();
            listTerm2.Add(termy1);
            listTerm2.Add(disablingy);
            Sequence panY = new Sequence(listTerm2);

            List<Term> listaPan = new List<Term>();
            listaPan.Add(panX);
            listaPan.Add(panY);

            //gesture contenente i 2 tipi di pan
            Choice choicePan = new Choice(listaPan);




            // Dichiarazione Joint
            jointSensor = new Sensor(3);
            //am.OnSkeletonFrameManaged+= updateJoint;
        }

        private bool close(Token token)
        {
            throw new NotImplementedException();
        }

        public void updateSkeleton(AcquisitionManager am)
        {
            foreach(Skeleton skeleton in am.skeletonList)
            {
                if (skeleton.getStatus())
                {
                    if (jointSensor.checkSkeleton(skeleton.getIdSkeleton()))
                        jointSensor.generateToken(TypeToken.Move, skeleton);
                    else
                        jointSensor.generateToken(TypeToken.Start, skeleton);
                }
                else if (jointSensor.checkSkeleton(skeleton.getIdSkeleton()))
                    jointSensor.generateToken(TypeToken.End, skeleton);
                //else if(jointSensor.checkJoint(ji.getType()))
                //rimuovi token
            }
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
                                jointSensor.generateToken(TypeToken.Move, ji);
                            else
                                jointSensor.generateToken(TypeToken.Start, ji);
                        }
                        //else if(jointSensor.checkJoint(ji.getType()))
                        //rimuovi token
                    }
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
