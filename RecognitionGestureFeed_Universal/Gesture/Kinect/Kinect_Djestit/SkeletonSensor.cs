using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_Djestit
{
    public class SkeletonSensor : Sensor
    {
        /* Attributi  */
        public SkeletonStateSequence sequence;

        public SkeletonSensor(Term root, int capacity) : base(root, capacity)
        {
            this.sequence = new SkeletonStateSequence(this.capacity);
        }

        public Token generateToken(TypeToken type, Skeleton skeleton)
        {
            // Creo uno SkeletonToken a partire dallo Skeleton ricevuto in input
            SkeletonToken token = new SkeletonToken(type, skeleton);
            SkeletonEventArgs e = new SkeletonEventArgs(this);
            switch(type)
            {
                case TypeToken.Start:
                    // Genero l'evento onSkeletonStart
                    _onTokenStart(e);
                    break;
                case TypeToken.Move:
                    // Genero l'evento onSkeletonStart
                    _onTokenMove(e);
                    // Copio la lista di vecchi token
                    List<SkeletonToken> listToken;
                    sequence.moves.TryGetValue(token.identifier, out listToken);
                    token.precSkeletons = listToken.Select(item =>(Skeleton)item.skeleton.Clone()).ToList();
                    break;
                case TypeToken.End:
                    // Genero l'evento onSkeletonStart
                    _onTokenEnd(e);
                    // Rimuovo lo scheletro in questione dalla mappa
                    sequence.removeById(token.identifier);

                    break;
            }
            // Se lo scheletro gestito non è di tipo end, allora si provvede ad inserirlo nel buffer
            if(type != TypeToken.End)
                this.sequence.push(token);
            return token;
        }

        /// <summary>
        /// Verifica se in state sequence è già presente quello scheletro.
        /// </summary>
        /// <param name="skeletonId"></param>
        /// <returns></returns>
        public override bool checkId(int skeletonId)
        {
            if (this.sequence.moves.ContainsKey(skeletonId))
                return true;
            else
                return false;
        }
    }
}
