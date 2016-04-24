using System.Collections.Generic;
using Unica.Djestit.Kinect2;

namespace Unica.Djestit.JSON.Kinect2
{
    public class KinectObjectEmitter : IGroundTermEmitter
    {
        ITermReference IGroundTermEmitter.EmitGroundTerm(string[] declaration, Dictionary<string, object> args)
        {
            ITermReference reference = null;
            string suffix = "";
            if (args.ContainsKey("_right"))
            {
                suffix = "Right";
            }
            if (args.ContainsKey("_left"))
            {
                suffix = "Left";
            }

            
            if (declaration[0].Equals("kinect2"))
            {
                switch (declaration.Length)
                {
                    case 4:
                        if (declaration[1].Equals("skeleton"))
                        {
                            JointType type = JointFromString(declaration[2]);
                            if(type == JointType.None)
                            {
                                type = JointFromString(declaration[2] + suffix);
                            }
                            if(type == JointType.HandLeft || type == JointType.HandRight)
                            {
                                HandState state = HandStateFromString(declaration[3]);
                                HandStateTerm term = new HandStateTerm(type, state);
                                reference = new ObjectTerm(term);
                            }
                        }
                        break;
                    case 3:
                        // generating terms for the kinect2
                        if (declaration[1].Equals("skeleton"))
                        {
                            JointType type = JointFromString(declaration[2]);
                            if (type == JointType.None)
                            {
                                type = JointFromString(declaration[2] + suffix);
                            }
                            SkeletonJointTerm term = new SkeletonJointTerm(type);
                            reference = new ObjectTerm(term);
                        }
                        break;
                }
               
            }

            return reference;
        }

        private HandState HandStateFromString(string s)
        {
            if (s.Equals("open")) return HandState.Open;
            if (s.Equals("closed")) return HandState.Closed;
            if (s.Equals("lasso")) return HandState.Lasso;
            if (s.Equals("notTracked")) return HandState.NotTracked;
            return HandState.Unknown;
        }

        private JointType JointFromString(string s)
        {
            if (s.Equals("spineBase")) return JointType.SpineBase;
            if (s.Equals("spineMid")) return JointType.SpineMid;
            if (s.Equals("neck")) return JointType.Neck;
            if (s.Equals("head")) return JointType.Head;
            if (s.Equals("shoulderLeft")) return JointType.ShoulderLeft;
            if (s.Equals("elbowLeft")) return JointType.ElbowLeft;
            if (s.Equals("wristLeft")) return JointType.WristLeft;
            if (s.Equals("handLeft")) return JointType.HandLeft;
            if (s.Equals("shoulderRight")) return JointType.ShoulderRight;
            if (s.Equals("elbowRight")) return JointType.ElbowRight;
            if (s.Equals("wristRight")) return JointType.WristRight;
            if (s.Equals("handRight")) return JointType.HandRight;
            if (s.Equals("hipLeft")) return JointType.HipLeft;
            if (s.Equals("kneeLeft")) return JointType.KneeLeft;
            if (s.Equals("ankleLeft")) return JointType.AnkleLeft;
            if (s.Equals("footLeft")) return JointType.FootLeft;
            if (s.Equals("hipRight")) return JointType.HipRight;
            if (s.Equals("kneeRight")) return JointType.KneeRight;
            if (s.Equals("ankleRight")) return JointType.AnkleRight;
            if (s.Equals("footRight")) return JointType.FootRight;
            if (s.Equals("spineShoulder")) return JointType.SpineShoulder;
            if (s.Equals("handTipLeft")) return JointType.HandTipLeft;
            if (s.Equals("thumbLeft")) return JointType.ThumbLeft;
            if (s.Equals("handTipRight")) return JointType.HandTipRight;
            if (s.Equals("thumbRight")) return JointType.ThumbRight;
            return JointType.None;
        }
    }
}
