using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect_Djestit
{
    public class SkeletonStateSequence : StateSequence
    {
        /* Attributi */
        internal Dictionary<int, List<SkeletonToken>> moves;
        internal Dictionary<int, int> m_index;

        /* costruttore */
        public SkeletonStateSequence(int capacity) : base(capacity)
        {
            this.moves = new Dictionary<int, List<SkeletonToken>>();
            this.m_index = new Dictionary<int, int>();
        }

        /* Metodi */
        public void push(SkeletonToken token)
        {
            this._push(token);
            switch (token.type)
            {
                case TypeToken.Start:
                    this.moves.Add(token.identifier, new List<SkeletonToken>());
                    this.m_index.Add(token.identifier, 0);
                    goto case TypeToken.Move;
                case TypeToken.Move:
                    goto case TypeToken.End;
                case TypeToken.End:
                    List<SkeletonToken> t;
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

        public SkeletonToken getById(int delay, int id)
        {
            int pos = 0;
            List<SkeletonToken> t;
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
