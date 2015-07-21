using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Likelihood
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood;
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
            // Probabilità del GroundTerm
            this.likelihood = new Likelihood(term.likelihood.likelihood);
            // Handler che aggiorna lo stato del term quando questo viene aggiornato
            this.term.ChangeState += updateTerm;
        }

        /* Metodi */
    }
}
