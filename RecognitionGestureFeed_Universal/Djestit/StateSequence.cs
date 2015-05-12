using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    class StateSequence
    {
        //attributi
        private int capacity;
        private List<Token> tokens;
        private int index;

        //metodi

        //nel JS si chiamava init ma credo si possa utilizzare come un costruttore
        public StateSequence(int capacity)
        {
            if(this.capacity == capacity) 
                this.capacity = capacity;
            else 
                capacity = 2;

	        this.tokens = new List<Token>();
	        this.index = -1;
        }

        public void _push(Token token)
        {
            if (this.tokens.Count > this.capacity)
            {
                this.tokens.Add(token);
                this.index++;
            }
            else
            {
                this.index = (this.index + 1) % this.capacity;
                this.tokens[this.index] = token;
            }
        }

        public void push(Token token)
        {
            this._push(token);
        }

        public Token get(int delay)
        { 
            int pos = Math.Abs(this.index - delay) % this.capacity;
            return this.tokens[pos];
        }

        //su JS c'è this.init(capacity) ma credo che serva solo per inizializzare questa classe
    }
}
