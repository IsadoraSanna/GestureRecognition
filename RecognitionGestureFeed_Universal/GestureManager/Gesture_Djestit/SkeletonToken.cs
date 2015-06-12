using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// Kinect - Prova
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    public enum TypeToken
    {
        Start,
        Move,
        End
    }

    public class SkeletonToken : Token
    {
        public Skeleton skeleton;
        public TypeToken type;
        public List<Skeleton> precSkeletons;
        public int identifier;

        /* Costruttore */
        public SkeletonToken(TypeToken type, Skeleton sklt)
        {
            this.skeleton = (Skeleton)sklt.Clone();
            this.type = type;
            this.precSkeletons = new List<Skeleton>();
            this.identifier = sklt.getIdSkeleton();
        }
    }
}
