using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    public class JointStateSequence : StateSequence
    {
        /* Attributi */
        internal Dictionary<int,List<JointToken>> moves;
        internal Dictionary<int, int> m_index;
        //List<StateSequence> moves;
        //internal Dictionary<int,List<NewJointToken>> moves;
        //internal Dictionary<int, int> m_index;

        /* costruttore */
        public JointStateSequence(int capacity) : base(capacity)
        {
            this.moves = new Dictionary<int, List<JointToken>>();
            this.m_index = new Dictionary<int, int>();
            //this.moves = new Dictionary<int, List<NewJointToken>>();
            //this.m_index = new List<int>();
        }

        /* Metodi */
        public void push(JointToken token)
        {
            this._push(token);
            switch(token.type)
            {
                case TypeToken.Start:
                    this.moves.Add(token.identifier, new List<JointToken>());
                    this.m_index.Add(token.identifier, 0);
                    goto case TypeToken.Move;
                case TypeToken.Move:
                    goto case TypeToken.End;
                case TypeToken.End:
                    List<JointToken> t;
                     this.moves.TryGetValue((int)token.identifier, out t);

                    if (t.Count < this.capacity)//this.moves[(int)token.identifier].tokens.Count() < this.capacity)
                        t.Add(token);
                        //this.moves[(int)token.identifier].tokens.Add(token);
                    else
                    {
                        t.Insert(this.m_index[token.identifier], token);/******************************** Modificare */
                        //this.moves[(int)token.identifier].tokens.Insert(this.m_index[token.identifier], token);
                    }

                    int c;
                    this.m_index.TryGetValue(token.identifier, out c);
                    c = (c+1) % this.capacity;
                    m_index[token.identifier] = c;
                    break;
                
            }
        }
        /*
        public void push(NewJointToken token)
        {
            this._push(token);
            switch (token.type)
            {
                case TypeToken.Start:
                    this.moves.Add(token.identifier, new List<NewJointToken>());
                    this.m_index.Add(token.identifier, 0);
                    //this.m_index.Insert(token.identifier, 0);
                    goto case TypeToken.Move;
                case TypeToken.Move:
                    goto case TypeToken.End;
                case TypeToken.End:
                    List<NewJointToken> t;
                     this.moves.TryGetValue((int)token.identifier, out t);

                    if (t.Count < this.capacity)//this.moves[(int)token.identifier].tokens.Count() < this.capacity)
                        t.Add(token);
                        //this.moves[(int)token.identifier].tokens.Add(token);
                    else
                    {
                        t.Insert(this.m_index[token.identifier], token);/******************************** Modificare *
                        //this.moves[(int)token.identifier].tokens.Insert(this.m_index[token.identifier], token);
                    }

                    int c;
                    this.m_index.TryGetValue(token.identifier, out c);
                    c = (c+1) % this.capacity;
                    m_index[token.identifier] = c;
                    break;

            }
        }         
        */
        public Token getById(int delay, int id)
        {

            int pos = 0;
            List<JointToken> t;
            this.moves.TryGetValue(id, out t);
            int m_index_id;
            this.m_index.TryGetValue(id, out m_index_id);

            if (t.Count < this.capacity)
                pos = m_index_id - delay - 1;
            //pos = this.m_index[id] - delay -1;
            else
                pos = (m_index_id - delay - 1 + this.capacity) % this.capacity;
                //pos = (this.m_index[id] - delay - 1 + this.capacity) % this.capacity;

            return t[pos];

            /*
            int size = this.moves.Count();
            
            // Crasha in questo punto
            if(this.moves[id].tokens.Count < this.capacity )
            {
                if(this.moves[id].tokens.Count < this.capacity)
                    pos = this.m_index[id] - delay -1;
                else
                    pos = (this.m_index[id] - delay - 1 + this.capacity) % this.capacity;
            }
            else
            {
                pos = (this.m_index[id] - delay - 1 + this.capacity) % this.capacity;
            }
            return this.moves[id].tokens[pos];// */
        }
    }
}
