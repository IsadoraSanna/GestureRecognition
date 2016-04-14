using RecognitionGestureFeed_Universal.Djestit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_Djestit
{
    class SkeletonMove : GroundTerm
    {
        public int id { get; private set; }

        public SkeletonMove(int id) : base()
        {
            this.id = id;
            this.accepts = func_accepts;
        }

        public bool func_accepts(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken skeletonToken = (SkeletonToken)token;
                if (skeletonToken.type != TypeToken.Move)
                    return false;
                if (this.id != skeletonToken.identifier)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}
