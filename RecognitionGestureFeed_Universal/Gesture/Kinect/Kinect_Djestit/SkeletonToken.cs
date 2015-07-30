using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;
// Kinect - Prova
using Microsoft.Kinect;

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
        // Identificativo associato allo scheletro
        public int identifier;

        /* Costruttore */
        public SkeletonToken(TypeToken type, Skeleton sklt)
        {
            this.skeleton = (Skeleton)sklt.Clone();
            this.type = type;
            this.precSkeletons = new List<Skeleton>();
            this.identifier = sklt.getIdSkeleton();
        }

        /* Prova */
        public float positionX;
        public float positionY;
    }
}
