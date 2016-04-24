
namespace Unica.Djestit.Kinect2
{
    public class HandStateTerm : GroundTerm
    {
        public int? SkeletonId { get; set; }

        public JointType JointType { get; internal set; }
        public HandState State { get; set; }

        public HandStateTerm(JointType type, HandState state)
        {
            if (type == JointType.HandLeft|| type == JointType.HandRight)
            {
                this.JointType = type;
                this.State = state;
            }
            else
            {
                this.JointType = JointType.None;
            }
            
        }

        protected override bool _Accepts(Token token)
        {
            SkeletonToken sk = token as SkeletonToken;
            if (this.SkeletonId != null && sk.identifier == this.SkeletonId)
            {
                return false;
            }

            switch (JointType)
            {
                case JointType.HandLeft:
                    if((int) sk.skeleton.leftHandStatus != (int)state)
                    {
                        return false;
                    }
                    break;

                case JointType.HandRight:
                    if ((int)sk.skeleton.rightHandStatus != (int)state)
                    {
                        return false;
                    }
                    break;
            }
            if (Accepts != null)
            {
                return Accepts(token);
            }
            else
            {
                return true;
            }
        }

   }

    public enum HandState
    {
        Unknown = 0,
        NotTracked = 1,
        Open = 2,
        Closed = 3,
        Lasso = 4
    }
}
