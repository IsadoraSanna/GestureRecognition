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
        public Skeleton skeleton;
        public TypeToken type;
        public List<JointInformation> jointsOld;
        public Skeleton sOld;
        public int identifier;

        /* Costruttore */
        public SkeletonToken(TypeToken type, Skeleton sklt)
        {
            this.skeleton = (Skeleton)sklt.Clone();
            jointsOld = new List<JointInformation>();
            this.type = type;
            this.identifier = skeleton.getIdSkeleton();//(int)skeleton.getId();
        }
    }
}
