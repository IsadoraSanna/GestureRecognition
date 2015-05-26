﻿using System;
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
        List<StateSequence> moves;
        List<int> m_index;

        /* costruttore */
        public JointStateSequence(int capacity) : base(capacity)
        {
            this.moves = new List<StateSequence>();
            this.m_index = new List<int>();
        }

        /* Metodi *
        public void push(JointToken token)
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
        }*/
        public void push(NewJointToken token)
        {
            this._push(token);
            switch (token.type)
            {
                case TypeToken.Start:
                    StateSequence s = new StateSequence(0);
                    s.tokens = new List<Token>();
                    this.moves.Insert(token.identifier, s);
                    this.m_index.Insert(token.identifier, 0);
                    //this.m_index.Add(0);
                    //StateSequence i = new StateSequence(0);
                    //this.moves.Add(i);
                    //this.m_index.Add(0);
                    break;
                case TypeToken.Move:
                    break;
                case TypeToken.End:
                    if (this.moves[(int)token.identifier].tokens.Count() < this.capacity)
                        this.moves[(int)token.identifier].tokens.Add(token);//this.moves[(int)token.identifier].push(token);
                    else
                    {
                        this.moves[(int)token.identifier].tokens.Insert(this.m_index[token.identifier], token);
                    }
                    this.m_index.Add(this.m_index[(int)token.identifier + 1] % this.capacity);
                    break;

            }
        }         

        public Token getById(int delay, int id)
        {

            int pos = 0;
            int size = this.moves.Count();

            if(size > id) //if(this.moves[id].tokens.Count < this.capacity )
            {
                if(this.moves[id].tokens.Count < this.capacity)
                    pos = this.m_index[id] - delay -1;
                else
                    pos = (this.m_index[id] - delay - 1 + this.capacity) % this.capacity;
            }
            else
            {
                pos = (this.m_index[id] - delay - 1 + this.capacity) % this.capacity; //pos = (this.m_index[id] - delay - 1 + this.capacity) % this.capacity;
            }
            return this.moves[id].tokens[pos];

        }
    }
}