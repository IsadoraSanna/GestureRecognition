using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Handler
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper
{
    public class FeedbackGroupEventArgs
    {
        /* Attributi */
        // Term relativo al wrapper inviato
        public Term term { get; private set; }
        // Group che descrive la gesture
        public FeedbackGroup group { get; private set; }
        // Handler associata alla gesture
        public RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler.Handler handler { get; private set; }

        /* Costruttore */
        public FeedbackGroupEventArgs(Term term, FeedbackGroup group, Handler.Handler handler)
        {
            this.term = term;
            this.group = group;
            this.handler = handler;
        }
    }
}
