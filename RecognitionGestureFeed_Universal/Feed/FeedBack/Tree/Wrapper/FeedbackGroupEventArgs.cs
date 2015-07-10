using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper
{
    public class FeedbackGroupEventArgs
    {
        /* Attributi */
        // Term relativo al wrapper inviato
        public Term term { get; private set; }
        // Wrapper relativo al group
        public FeedbackWrapper wrapper { get; private set; }
        // Group che descrive la gesture
        public FeedbackGroup group { get; private set; }

        /* Metodi */
        public FeedbackGroupEventArgs(Term term, FeedbackWrapper wrapper, FeedbackGroup group)
        {
            this.term = term;
            this.wrapper = wrapper;
            this.group = group;
        }
    }
}
