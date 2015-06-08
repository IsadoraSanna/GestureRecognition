using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Djestit
{
    //da capire perchè non mi vede la precedente definizione in Term

    public class Sequence : CompositeTerm
    {
        //public List<Term> children;
        private int index = 0;

        //COSTRUTTORI
        //creo 2 costruttori invece che solo uno come nel JS dato che è troppo tipato
        public Sequence(Term terms)
        {
            this.children = new List<Term>();
        }

        public Sequence(List<Term> terms)
        {
            this.children = terms;
        }

        //METODI
        public override void reset()
        {
            this.state = expressionState.Default;
            this.index = 0;
            foreach (Term t in children)
            {
                t.reset();
            }
        }

        public override bool lookahead(Token token)
        {
            if (this.state == expressionState.Complete || this.state == expressionState.Error)
            {
                return false;
            }
            //terzo argomento dell'if . nel codice JS non c'è il parametro token
            if ((this.children != null) && (this.children[index] != null))// && (this.children[index].lookahead(token) != null))
            {
                //if (this.children[index] != null)
                return this.children[index].lookahead(token);
            }

            return false;
        }

        public override void fire(Token token)
        {
            if (this.lookahead(token))
            {
                this.children[index].fire(token);
            }
            else
            {
                this.error(token);
                return;
            }

            if (index >= this.children.Count)
            {
                this.error(token);
            }

            switch (this.children[index].state)
            {
                case expressionState.Complete:
                    this.index++;
                    if(index >= this.children.Count)
                        this.complete(token);
                    break;
                case expressionState.Error:
                    this.error(token);
                    break;
            }  
        }
    }
}
