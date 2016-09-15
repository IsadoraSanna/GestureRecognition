using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// GroundTerm
using Unica.Djestit;

namespace Unica.Djestit.Kinect2
{
    public class SkeletonJointTerm : GroundTerm
    {
        public int? SkeletonId { get; set; }

        public JointType JointType { get; internal set; }

        public SkeletonJointTerm(JointType type)
        {
            this.JointType = type;
        }

        public override bool _accepts(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken sk = token as SkeletonToken;
                if(this.SkeletonId != null && sk.identifier == this.SkeletonId)
                {
                    return false;
                }

                if(Accepts != null)
                {
                    return Accepts(token);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }

    public enum JointType
    {
        SpineBase = 0,
        SpineMid = 1,
        Neck = 2,
        Head = 3,
        ShoulderLeft = 4,
        ElbowLeft = 5,
        WristLeft = 6,
        HandLeft = 7,
        ShoulderRight = 8,
        ElbowRight = 9,
        WristRight = 10,
        HandRight = 11,
        HipLeft = 12,
        KneeLeft = 13,
        AnkleLeft = 14,
        FootLeft = 15,
        HipRight = 16,
        KneeRight = 17,
        AnkleRight = 18,
        FootRight = 19,
        SpineShoulder = 20,
        HandTipLeft = 21,
        ThumbLeft = 22,
        HandTipRight = 23,
        ThumbRight = 24,
        None = -1
    }
}
