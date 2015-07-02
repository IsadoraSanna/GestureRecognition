using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack
{
    public class FeedbackRoot
    {
        /* Attributi */
        // children conterrà tutte le gesture presenti nell'espressione passata in input
        public List<FeedbackGroup> children { get; private set; }

        /* Costruttore */
        public FeedbackRoot(CompositeTerm compositeTerm)
        {
            // Inizializzo la lista di children
            children = new List<FeedbackGroup>();

            /// Se il term ha degli altri elementi al suo interno (e quindi è un compositeTerm) allora 
            /// provvede a creare i nuovi rami dell'albero. Altrimenti se al suo interno ha un'unica gesture,
            /// allora provvede a creare un unico figlio.
            //if (term.GetType() != typeof(Term))
            //{
                //CompositeTerm compositeTerm = (CompositeTerm)term;
                // Creo un FeedbackGroup per ogni sottocomponente di term e lo metto nella lista children
                foreach (var child in compositeTerm.children)
                {
                    this.children.Add(new FeedbackGroup(child));
                }
            //}
            //else
            //{
            //    this.children.Add(new FeedbackGroup(term));
            //}
        }

        /* Metodi */
        public void visitingTree()
        {
            foreach (FeedbackGroup child in this.children)
            {
                //Debug.WriteLine(child.term.GetType().ToString());
                child.visitingTree();
            }
        }
    }
}
