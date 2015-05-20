using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class GestureStateSequence : StateSequence
    {
        /* Attributi */
        int capacity;
        List<StateSequence> moves;
        List<int> m_index;

        /* costruttore */
        public GestureStateSequence(int capacity)
        {
            this.capacity = capacity;
            this.moves = new List<StateSequence>();
            this.m_index = new List<int>();
        }

        /* Metodi */
        public void push(GestureToken token)
        {
            this._push(token);
            switch(token.type)
            {
                case TypeToken.Start:
                    StateSequence i = new StateSequence(0);
                    this.moves.Add(i);
                    break;
                case TypeToken.Move:
                    break;
                case TypeToken.End:
                    if(this.moves[(int)token.identifier].tokens.Count() < this.capacity)
                        this.moves[(int)token.identifier].push(token);
                    else
                        this.moves[(int)token.identifier].tokens[(int)token.identifier] = token;
                    this.m_index.Add(this.m_index[(int)token.identifier+1] % this.capacity);
                    break;
                
            }
        }

        public Token getById(int delay, int id)
        {
            int pos = 0;

            if(this.moves[id].tokens.Count < this.capacity)
                pos = this.m_index[id] - delay -1;
            else
                pos = (this.m_index[id] - delay -1 + this.capacity) % this.capacity;

            return this.moves[id].tokens[pos];
        }
    }
}
