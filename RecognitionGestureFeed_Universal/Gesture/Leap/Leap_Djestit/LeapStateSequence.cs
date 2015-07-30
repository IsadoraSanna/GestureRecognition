using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.Gesture.Leap.Leap_Djestit
{
    class LeapStateSequence: StateSequence
    {
        /* Attributi */
        internal Dictionary<int, List<LeapToken>> moves;
        internal Dictionary<int, int> m_index;

        /* Costruttore */
        public LeapStateSequence(int capacity) : base(capacity)
        {
            this.moves = new Dictionary<int, List<LeapToken>>();
            this.m_index = new Dictionary<int, int>();
        }

        /* Metodi */
        public void push(LeapToken token)
        {
            this._push(token);
            switch (token.type)
            {
                case TypeToken.Start:
                    this.moves.Add(token.identifier, new List<LeapToken>());
                    this.m_index.Add(token.identifier, 0);
                    goto case TypeToken.Move;
                case TypeToken.Move:
                    goto case TypeToken.End;
                case TypeToken.End:
                    List<LeapToken> t;
                    this.moves.TryGetValue(token.identifier, out t);
                    int index;
                    this.m_index.TryGetValue(token.identifier, out index);

                    if (t.Count < this.capacity)
                    {
                        t.Add(token);
                        index++;
                    }
                    else
                        t[index] = token;


                    index = (index + 1) % this.capacity;
                    m_index[token.identifier] = index;
                    break;
            }
        }

        public LeapToken getById(int delay, int id)
        {
            int pos = 0;
            List<LeapToken> t;
            this.moves.TryGetValue(id, out t);
            int m_index_id;
            this.m_index.TryGetValue(id, out m_index_id);

            if (t.Count < this.capacity)
                pos = m_index_id - delay - 1;
            else
                pos = (m_index_id - delay - 1 + this.capacity) % this.capacity;

            return t[pos];
        }

        /// <summary>
        /// Cancella tutti i riferimenti in moves e m_index dello scheletro con quel token
        /// </summary>
        /// <param name="id"></param>
        public void removeById(int id)
        {
            this.moves.Remove(id);
            this.m_index.Remove(id);
        }
    }
}
