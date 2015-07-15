using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class CompositeTerm : Term
    {
        /* Attributi */
        public List<Term> children = new List<Term>();

        /* Metodi */
        public override void reset()
        {
	        this.state = expressionState.Default;
	        foreach(var child in this.children)
	        {
		        child.reset();
	        }
        }

        public override void error(Token token)
        {
            foreach (var child in this.children)
            {
                child.error(token);
            }
            base.error(token);
        }
    }
}
