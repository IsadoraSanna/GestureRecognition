using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree
{
    public class FeedbackLeaf : FeedbackGroup
    {
        /* Attributi */

        /* Costruttore */
        public FeedbackLeaf(Term term) : base(term)
        {
            this.likelihood = term.likelihood;
            this.term.ChangeState += updateTerm;
        }

        /* Metodi */
    }
}
