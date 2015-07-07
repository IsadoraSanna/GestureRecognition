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
        True = 2,
        Continue = 1,
        Default = 0,
        False = -1
    }

    //
    public delegate void FeedbackGroupUpdate(Term term);
    //
    public delegate void FeedbackGroupTrue(FeedbackWrapper wrapper);

    public class FeedbackGroup : FeedbackNode
    {
        /* Eventi */
        FeedbackGroupTrue FeedbackGroupTrue;

        /* Attributi */
        // Term associato al Group in questione
        public Term term { get; private set; }
        // Wrapper associato al Group in questione
        public FeedbackWrapper wrapper;
        // Probabilità associata al Term
        public float likelihood { get; private set; }
        /// Flag per indicare lo stato del term:
        /// Se assume il valore True vuol dire che è ancora valido e quindi può essere visualizzato;
        /// Se assume il valore Default vuol dire che il sistema sta aspettando che l'utente esegua una gesture;
        /// Se assume il valore False vuol dire che l'utente sta eseguendo una gesture totalmente dissimile, e quindi non dev'essere visualizzato.
        public StateGroup state { get; private set; }
        //
        int index = 0;

        /* Costruttore */
        public FeedbackGroup(Term term)
            : base(term)
        {
            // Term associato al FeedbackGroup
            this.term = term;
            // Inizializzo il Wrapper
            //this.wrapper = new FeedbackWrapper();
            // Probabilità dell'evento
            //this.likelihood
            // Inizialmente si pone state sempre a Default
            this.state = StateGroup.Default;
        }

        /* Metodi */
        public virtual void update(object obj, TokenFireArgs sender)
        {
            if (this.children.Count > 0)
            {
                foreach (FeedbackGroup child in this.children)
                {
                    child.update(obj, sender);
                    if (child.state == StateGroup.False)
                    {
                        this.state = StateGroup.False;
                        return;
                    }
                }
                if (this.children.Count == this.children.Where(this.ChildTrue).Count())
                {
                    this.state = StateGroup.True;
                    OnFeedbackGroupTrue();
                }
                /*else if(numberChildTrue > 0)
                {
                    this.state = StateGroup.Continue;
                }*/
            }
            else
            {
                if (this.term.state == expressionState.Error)
                    this.state = StateGroup.False;
                else if (this.term.state == expressionState.Complete)
                {
                    this.state = StateGroup.True;
                    OnFeedbackGroupTrue();
                }
            }            
        }

        //
        private void OnFeedbackGroupTrue()
        {
            if (this.FeedbackGroupTrue != null)
                this.FeedbackGroupTrue(this.wrapper);
        }

        public void visitingTree()
        {
            this.term.GetType().ToString();
            foreach (var child in this.children)
            {
                Debug.WriteLine(child.term.GetType().ToString());
            }
        }
    }
}
