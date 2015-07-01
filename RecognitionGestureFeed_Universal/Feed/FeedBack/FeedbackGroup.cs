using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack
{
    public delegate void FeedGroupUpdate(Term term);

    class FeedbackGroup
    {
        /* Eventi */

        /* Attributi */
        // Term associato al Group in questione
        Term term;
        // Se il Term ha a sua volta dei figli, li metto nella lista
        List<FeedbackGroup> children {get; private set;}
        // Wrapper associato al Group in questione
        FeedbackWrapper wrapper;
        // Probabilità associata al Term
        float likelihood {get; private set;}

        /* Costruttore */
        public FeedbackGroup(Term term)
        {
            // Term associato al FeedbackGroup
            this.term = term;
            // Inizializzo il Wrapper
            //this.wrapper = new FeedbackWrapper();
            // Inizializzo la lista di children
            this.children = new List<FeedbackGroup>();

            /// Se il term ha degli altri elementi al suo interno (e quindi è un compositeTerm) allora 
            /// provvede a creare i nuovi rami dell'albero
            if(term.GetType() == typeof(CompositeTerm))
            {
                CompositeTerm compositeTerm = (CompositeTerm)term;
                // Creo un FeedbackGroup per ogni sottocomponente di term e lo metto nella lista children
                foreach(var child in compositeTerm.children)
                {
                    this.children.Add(new FeedbackGroup(child));
                }
            }
        }

        /* Metodi */

    }
}
