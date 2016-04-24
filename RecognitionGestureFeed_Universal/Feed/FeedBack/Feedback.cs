using System.Collections.Generic;


namespace Unica.Djestit.Feed
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
