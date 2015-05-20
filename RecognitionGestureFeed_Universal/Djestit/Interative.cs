using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class Interative : CompositeTerm
    {
        private Term children;

        //COSTRUTTORI
        //creo 2 costruttori invece che solo uno come nel JS dato che è troppo tipato
        public Interative(Term term)
        {
            this.children = term;
        }

        public Interative(List<Term> terms)
        {
            this.children = terms.First();
        }

        public void reset()
        {
            this.state = expressionState.Default;
            if(this.children != null)
            {
                children.reset();
            }
        }
        
        public bool lookahead(Token token)
        {

            if(this.children != null && this.children.lookahead(token))
                return this.children.lookahead(token);
            else
                return false;
        }

        public void fire(Token token)
        {
            if(this.lookahead(token))// if (this.lookahead(token) && this.children.fire)
            {
                this.children.fire(token);
                switch (this.children.state)
                {
                    case expressionState.Complete:
                        this.complete(token);
                        this.children.reset();
                        break;
                    case expressionState.Eerror:
                        this.error(token);
                        this.children.reset();
                        break;
                }
            }
        }
    }
}
