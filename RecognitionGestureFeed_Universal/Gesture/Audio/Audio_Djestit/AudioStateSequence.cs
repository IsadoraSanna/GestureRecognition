using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using Unica.Djestit;

namespace Unica.Djestit.Audio
{
    public class AudioStateSequence : StateSequence
    {
        /* Attributi */
        internal Dictionary<int, List<AudioToken>> speechs;
        internal Dictionary<int, int> s_index;

        /* Costruttore */
        public AudioStateSequence(int capacity) : base(capacity)
        {
            this.speechs = new Dictionary<int, List<AudioToken>>();
            this.s_index = new Dictionary<int, int>();
        }

        /* Metodi */
        public void push(AudioToken token)
        {
            this._push(token);
            switch (token.type)
            {
                case TypeToken.Start:
                    this.speechs.Add(token.identifier, new List<AudioToken>());
                    this.s_index.Add(token.identifier, 0);
                    goto case TypeToken.Move;
                case TypeToken.Move:
                    goto case TypeToken.End;
                case TypeToken.End:
                    List<AudioToken> t;
                    this.speechs.TryGetValue(token.identifier, out t);
                    int index;
                    this.s_index.TryGetValue(token.identifier, out index);

                    if (t.Count < this.capacity)
                    {
                        t.Add(token);
                        index++;
                    }
                    else
                        t[index] = token;

                    index = (index + 1) % this.capacity;
                    s_index[token.identifier] = index;
                    break;
            }
        }

        public AudioToken getById(int delay, int id)
        {
            int pos = 0;
            List<AudioToken> t;
            this.speechs.TryGetValue(id, out t);
            int m_index_id;
            this.s_index.TryGetValue(id, out m_index_id);

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
            this.speechs.Remove(id);
            this.s_index.Remove(id);
        }
    }
}
