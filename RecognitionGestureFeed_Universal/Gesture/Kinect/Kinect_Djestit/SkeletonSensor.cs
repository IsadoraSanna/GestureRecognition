using System.Collections.Generic;
using System.Linq;
using Unica.Djestit.Recognition.Kinect2;

namespace Unica.Djestit.Kinect2
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
                    // Genero l'evento onSkeletonMove
                    _onTokenMove(e);
                    // Copio la lista di vecchi token
                    List<SkeletonToken> listToken;
                    sequence.moves.TryGetValue(token.identifier, out listToken);
                    token.precSkeletons = listToken.Select(item =>(Skeleton)item.skeleton.Clone()).ToList();
                    int index;
                    sequence.m_index.TryGetValue(token.identifier, out index);
                    token.indexBuffer = index;
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
