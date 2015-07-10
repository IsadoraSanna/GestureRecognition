using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Acquisition
using RecognitionGestureFeed_Universal.Recognition;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// FeedbackTree
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack
{
    public class Feedback
    {
        /* Attributi */
        // Albero dei Feedback costruito a partire dalle gesture
        public FeedbackRoot tree { get; private set; }

        /* Costruttore */
        public Feedback(Choice expr)
        {
            // Creo l'albero dei Feedback
            this.tree = new FeedbackRoot(expr);
        }
    }
}
