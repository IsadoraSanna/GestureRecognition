using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    public class SkeletonToken : Token
    {
        internal Dictionary<int, List<JointToken>> joints;
        internal Skeleton skeleton;
        internal TypeToken type;
        internal int identifier;

        /* Costruttore */
        public SkeletonToken(TypeToken type, Skeleton sklt)
        {
            this.skeleton = (Skeleton)sklt.Clone();
            joints = new Dictionary<int,List<JointToken>>();
            this.type = type;
            this.identifier = skeleton.getIdSkeleton();//(int)skeleton.getId();
        }
    }
}
