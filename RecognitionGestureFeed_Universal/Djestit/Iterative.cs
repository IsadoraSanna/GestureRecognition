using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class Iterative : CompositeTerm
    {
        public Term child;

        //COSTRUTTORI
        //creo 2 costruttori invece che solo uno come nel JS dato che è troppo tipato
        public Iterative(Term term)
        {
            this.child = term;
        }

        public Iterative(List<Term> terms)
        {
            this.child = terms.First();
        }

        public override void reset()
        {
            this.state = expressionState.Default;
            if(this.child != null)
            {
                child.reset();
            }
        }

        public override bool lookahead(Token token)
        {

            if(this.child != null && this.child.lookahead(token))
                return this.child.lookahead(token);
            else
                return false;
        }

        public override void fire(Token token)
        {
            if(this.lookahead(token))// if (this.lookahead(token) && this.children.fire)
            {
                this.child.fire(token);
                switch (this.child.state)
                {
                    case expressionState.Complete:
                        this.complete(token);
                        this.child.reset();
                        break;
                    case expressionState.Error:
                        this.error(token);
                        this.child.reset();
                        break;
                }
            }
        }
    }
}
