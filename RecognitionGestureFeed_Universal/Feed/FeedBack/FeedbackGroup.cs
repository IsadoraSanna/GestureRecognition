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
    // Enum state
    public enum StateGroup
    {
        True = 1,
        Default = 0,
        False = -1
    }

    public delegate void FeedGroupUpdate(Term term);

    public class FeedbackGroup
    {
        /* Eventi */

        /* Attributi */
        // Term associato al Group in questione
        public Term term { get; private set; }
        // Se il Term ha a sua volta dei figli, li metto nella lista
        public List<FeedbackGroup> children {get; private set;}
        // Wrapper associato al Group in questione
        public FeedbackWrapper wrapper;
        // Probabilità associata al Term
        public float likelihood {get; private set;}
        /// Flag per indicare lo stato del term:
        /// Se assume il valore True vuol dire che è ancora valido e quindi può essere visualizzato;
        /// Se assume il valore Default vuol dire che il sistema sta aspettando che l'utente esegua una gesture;
        /// Se assume il valore False vuol dire che l'utente sta eseguendo una gesture totalmente dissimile, e quindi non dev'essere visualizzato.
        public StateGroup state { get; private set; }

        /* Costruttore */
        public FeedbackGroup(Term term)
        {
            // Term associato al FeedbackGroup
            this.term = term;
            // Inizializzo il Wrapper
            //this.wrapper = new FeedbackWrapper();
            // Inizializzo la lista di children
            this.children = new List<FeedbackGroup>();
            // Probabilità dell'evento
            //this.likelihood
            // Inizialmente si pone state sempre a Default
            this.state = StateGroup.Default;

            /// Se il term ha degli altri elementi al suo interno (e quindi è un compositeTerm) allora 
            /// provvede a creare i nuovi rami del sottalbero
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
        public void update()
        {
            if (term.state == expressionState.Error)
                state = StateGroup.False;
        }

        public void visitingTree()
        {
            foreach (var child in this.children)
            {
                Debug.WriteLine(term.GetType().ToString());
            }
        }
    }
}
