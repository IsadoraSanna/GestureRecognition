using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_Djestit
{
    public class SkeletonToken : Token
    {
        /* Attributi */
        // Scheletro associato al token
        public Skeleton skeleton;
        // Tipo di token (Start, Move o End)
        public TypeToken type;
        // Buffer che contiene gli n scheletri precedentemente rilevati
        public List<Skeleton> precSkeletons;
        // Indice per il buffer
        public int indexBuffer;
        // Identificativo associato allo scheletro
        public int identifier;

        /* Costruttore */
        public SkeletonToken(TypeToken type, Skeleton skeleton)
        {
            this.skeleton = (Skeleton)skeleton.Clone();
            this.type = type;
            this.precSkeletons = new List<Skeleton>();
            this.identifier = skeleton.idSkeleton;
        }
        public SkeletonToken()
        {

        }
    }
}
