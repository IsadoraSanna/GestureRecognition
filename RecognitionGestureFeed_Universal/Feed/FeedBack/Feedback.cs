using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
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
        public Feedback(List<Modifies> listModifies, Choice expr)
        {
            // Creo l'albero dei Feedback
            this.tree = new FeedbackRoot(listModifies, expr);
        }
    }
}
