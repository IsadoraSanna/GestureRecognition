using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    /**
     * A composite expression of terms connected with the choice operator.
     * The sequence operator expresses that it is possible to select one among 
     * the terms in order to complete the whole expression.
     * The implementation exploits a best effort approach for dealing with the 
     * selection ambiguity problem (see [1])
     */
    public class Choice : CompositeTerm
    {
        // Costruttori
        public Choice(Term terms)
        {
            this.children = new List<Term>();
        }
        public Choice(List<Term> terms)
        {
            this.children = terms;
        }

        // Metodi
        public override void reset()
        {
            this.state = expressionState.Default;
            foreach (Term child in this.children)
            {
                child.reset();
                child.excluded = false;
            }
        }

        public override bool lookahead(Token token)
        {
            
            if (this.state == expressionState.Complete|| this.state == expressionState.Error)
                return false;
            if (this.children != null && this.children.GetType() == typeof(List<Term>))
            {
                for (int index = 0; index < this.children.Count; index++)
                {
                    if (!this.children[index].excluded && this.children[index].lookahead(token) == true)
                        return true;
                }
            }
            return false;

        }

        public void feedToken(Token token)
        {
            if (this.state == expressionState.Complete|| this.state == expressionState.Error)
                return;

            if (this.children != null && this.children.GetType() == typeof(List<Term>))
            {
                for (int index = 0; index < this.children.Count; index++)
                {
                    if (!this.children[index].excluded)
                    {
                        if (this.children[index].lookahead(token))
                        {
                            this.children[index].fire(token);
                        }
                        else
                        {
                            // the current sub-term is not able to handle the input
                            // sequence
                            this.children[index].excluded = true;
                            this.children[index].error(token);
                        }
                    }
                }
            }
        }

        public override void fire(Token token)
        {
            this.feedToken(token);
            bool allExcluded = true;

            for (int index = 0; index < this.children.Count; index++)
            {
                if (!this.children[index].excluded)
                {
                    allExcluded = false;
                    switch (this.children[index].state)
                    {
                        case expressionState.Complete:
                            // one of the subterms is completed, then the
                            // entire expression is completed
                            this.complete(token);
                            return;

                        case expressionState.Error:
                            // this case is never executed, since
                            // feedToken excludes the subterms in error state
                            return;
                    }
                }
            }
            if (allExcluded)
            {
                this.error(token);
            }
        }
    }
}
