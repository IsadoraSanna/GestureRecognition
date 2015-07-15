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
        // Handler associata alla gesture
        public Handler handler { get; private set; }

        /* Metodi */
        public FeedbackGroupEventArgs(Term term, FeedbackWrapper wrapper, FeedbackGroup group, Handler handler)
        {
            this.term = term;
            this.wrapper = wrapper;
            this.group = group;
            this.handler = handler;
        }
    }
}
