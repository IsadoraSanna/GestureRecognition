using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// GroundTerm
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect_Djestit
{
    class SkeletonEnd : GroundTerm
    {
        // Attributi
        public int id { get; private set; }

        /* Costruttore */
        public SkeletonEnd(int id) : base()
        {
            this.id = id;
            this.accepts = func_accepts;
        }

        public bool func_accepts(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken skeletonToken = (SkeletonToken)token;
                if (skeletonToken.type != TypeToken.End)
                    return false;
                if (this.id != skeletonToken.identifier)
                    return false;
                return true;
            }
            else
                return false;
        }
    }
}