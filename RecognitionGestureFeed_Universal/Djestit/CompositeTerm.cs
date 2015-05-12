using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class CompositeTerm : Term
    {
        protected expressionState state;
        protected List<Term> children;

        public void reset()
        {
	        this.state = expressionState.Default;
	        foreach(var child in this.children)
	        {
		        child.reset();
	        }
        } 
    }
}
