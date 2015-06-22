using RecognitionGestureFeed_Universal.Djestit;
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect_Djestit
{
    public delegate void SkeletonEventHandler(object sender, SkeletonEventArgs s);

    public class SkeletonSensor
    {
        // Eventi generati all'arrivo di un scheletro
        public event SkeletonEventHandler onSkeletonStart;
        public event SkeletonEventHandler onSkeletonMove;
        public event SkeletonEventHandler onSkeletonEnd;
        /* Attributi  */
        private int capacity;
        public Term root;
        public SkeletonStateSequence sequence;

        public SkeletonSensor(Term root, int capacity)
        {
            this.capacity = capacity;
            this.root = root;
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
                    _onSkeletonStart(e);
                    break;
                case TypeToken.Move:
                    // Genero l'evento onSkeletonStart
                    _onSkeletonMove(e);
                    // Copio la lista di vecchi token
                    List<SkeletonToken> listToken;
                    sequence.moves.TryGetValue(token.identifier, out listToken);
                    token.precSkeletons = listToken.Select(item =>(Skeleton)item.skeleton.Clone()).ToList();
                    break;
                case TypeToken.End:
                    // Genero l'evento onSkeletonStart
                    _onSkeletonEnd(e);
                    // Rimuovo lo scheletro in questione dalla mappa
                    sequence.removeById(token.identifier);

                    break;
            }
            // Se lo scheletro gestito non è di tipo end, allora si provvede ad inserirlo nel buffer
            if(type != TypeToken.End)
                this.sequence.push(token);
            return token;
        }

        /* Handler eventi di start, move ed end */
        /// <summary>
        /// Handler per l'evento start
        /// </summary>
        /// <param name="sender"></param>
        public void _onSkeletonStart(SkeletonEventArgs sender)
        {
            if (onSkeletonStart != null)
                onSkeletonStart(this, sender);
        }
        /// <summary>
        /// Handler per l'evento move
        /// </summary>
        /// <param name="sender"></param>
        public void _onSkeletonMove(SkeletonEventArgs sender)
        {
            if (onSkeletonMove != null)
                onSkeletonMove(this, sender);
        }
        /// <summary>
        /// Handler per l'evento end
        /// </summary>
        /// <param name="sender"></param>
        public void _onSkeletonEnd(SkeletonEventArgs sender)
        {
            if (onSkeletonEnd != null)
                onSkeletonEnd(this, sender);
        }

        /// <summary>
        /// Verifica se in state sequence è già presente quello scheletro.
        /// </summary>
        /// <param name="skeletonId"></param>
        /// <returns></returns>
        public bool checkSkeleton(int skeletonId)
        {
            if (this.sequence.moves.ContainsKey(skeletonId))
                return true;
            else
                return false;
        }
    }
}
