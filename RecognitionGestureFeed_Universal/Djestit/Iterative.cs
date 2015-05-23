using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class Iterative : CompositeTerm
    {
        public Term children;

        //COSTRUTTORI
        //creo 2 costruttori invece che solo uno come nel JS dato che è troppo tipato
        public Iterative(Term term)
        {
            this.children = term;
        }

        public Iterative(List<Term> terms)
        {
            this.children = terms.First();
        }

        public override void reset()
        {
            this.state = expressionState.Default;
            if(this.children != null)
            {
                children.reset();
            }
        }

        public override bool lookahead(Token token)
        {

            if(this.children != null && this.children.lookahead(token))
                return this.children.lookahead(token);
            else
                return false;
        }

        public override void fire(Token token)
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
                    case expressionState.Error:
                        this.error(token);
                        this.children.reset();
                        break;
                }
            }
        }
    }
}
