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
        public List<Term> children;
        private int index = 0;
        private expressionState state;

        //COSTRUTTORI
        //creo 2 costruttori invece che solo uno come nel JS dato che è troppo tipato
        public Sequence(Term terms)
        {
            this.children = new List<Term>();
            //se il termine che viene passato è di tipo terms
            //if (terms.GetType() == typeof(List<Term>))
        }

        public Sequence(List<Term> terms)
        {
            this.children = terms;
        }

        //METODI
        public void reset()
        {
            this.state = expressionState.Default;
            this.index = 0;
            foreach (Term t in children)
            {
                t.reset();
            }
        }

        public bool lookahead(Token token)
        {
            if (this.state == expressionState.Complete || this.state == expressionState.Error)
            {
                return false;
            }
            //terzo argomento dell'if . nel codice JS non c'è il parametro token
            if ((this.children != null) && (index < this.children.Count) )// && (this.children[index].lookahead(token) != null))
            {
                if (this.children[index] != null)
                    return this.children[index].lookahead(token);
            }

            return false;
        }

        public void fire(Token token)
        {
           // Debug.Assert(true, "Index: " + this.index);
            //come nell'if precedente per il fire
            if (this.lookahead(token))
            {
                this.children[index].fire(token);
            }
            else
            {
                this.error(token);
            }

            if (index >= this.children.Count)
            {
                this.error(token);
            }
            else if (this.children[index].state == expressionState.Complete)
            {
                this.index++;
                Debug.WriteLine(index);
                if (index >= this.children.Count)
                    this.complete(token);
            }
            /*
            switch (this.children[index].state)
            {
                case expressionState.Complete:
                    this.index++;
                    Debug.WriteLine(index);
                    if(index >= this.children.Count)
                        this.complete(token);
                    break;
                case expressionState.Eerror:
                    this.error(token);
                    break;

            }
             * */
        }
    }
}
