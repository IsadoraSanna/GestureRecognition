using System.Collections.Generic;
using Unica.Djestit.Recognition.Kinect2;

namespace Unica.Djestit.Kinect2
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
