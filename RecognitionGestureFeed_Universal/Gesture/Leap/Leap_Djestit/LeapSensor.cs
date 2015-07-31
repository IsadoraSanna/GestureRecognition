using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Copy
using RecognitionGestureFeed_Universal.Utilities;
// Leap
using Leap;

namespace RecognitionGestureFeed_Universal.Gesture.Leap.Leap_Djestit
{
    class LeapSensor : Sensor
    {
        /* Attributi  */
        public LeapStateSequence sequence;

        public LeapSensor(Term root, int capacity) : base(root, capacity)
        {
            this.sequence = new LeapStateSequence(this.capacity);
        }

        public Token generateToken(TypeToken type, Hand data)
        {
            // Creo uno SkeletonToken a partire dallo Skeleton ricevuto in input
            LeapToken token = new LeapToken(type, data);
            LeapEventArgs e = new LeapEventArgs(this);
            switch (type)
            {
                case TypeToken.Start:
                    // Genero l'evento onSkeletonStart
                    this._onTokenStart(e);
                    break;
                case TypeToken.Move:
                    // Genero l'evento onSkeletonStart
                    this._onTokenMove(e);
                    // Copio la lista di vecchi token
                    List<LeapToken> listToken;
                    sequence.moves.TryGetValue(token.identifier, out listToken);
                    token.precHands = listToken.Select(item => (Hand)item.hand.CloneObject()).ToList();
                    break;
                case TypeToken.End:
                    // Genero l'evento onSkeletonStart
                    this._onTokenEnd(e);
                    // Rimuovo lo scheletro in questione dalla mappa
                    sequence.removeById(token.identifier);
                    break;
            }
            // Se lo scheletro gestito non è di tipo end, allora si provvede ad inserirlo nel buffer
            if (type != TypeToken.End)
                this.sequence.push(token);
            return token;
        }

        /// <summary>
        /// Verifica se in state sequence è già presente quello scheletro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool checkId(int id)
        {
            if (this.sequence.moves.ContainsKey(id))
                return true;
            else
                return false;
        }
    }
}
