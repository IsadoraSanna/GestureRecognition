using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Handler 
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;
// Likelihood
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood;
// Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Concurrency (Transazioni)
using RecognitionGestureFeed_Universal.Concurrency;
// Transazioni
using System.Transactions;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Djestit
{
    // Enum expressionState
    public enum expressionState
    {
        Complete = 1,
        Default = 0,
        Likely = -1,
        Error = -2
    }
    // Delegate per i GestureEventHandler
    public delegate void GestureEventHandler(object obj, GestureEventArgs sender);
    public delegate void GestureChangeStateHandler();
    // Delegate per il TokenFire
    public delegate void TokenFire(object obj, TokenFireArgs sender);

    public abstract class Term
    {
        /* Eventi */
        public event GestureEventHandler Complete;
        public event GestureEventHandler Likely;
        public event GestureEventHandler Error;
        public event GestureChangeStateHandler ChangeState;
        public event TokenFire TokenFire;
        /* Attributi */
        public expressionState state = expressionState.Default;
        public bool excluded;
        public bool once;
        // Indica quante volte è stato eseguito il Term in questione, da quando il programma è stato avviato
        public int num_total { get; private set; }
        // Indica il numero di volte consecutive con cui è stato eseguito il Term in questione
        public int num_discrete { get; private set; }
        // Probabilità dell'evento
        //public Likelihood likelihood;
        public float likelihood;
        // Handler dell'evento
        //public List<Handler> handlers = new List<Handler>();
        public Handler handler;
        // Puntatore al padre
        internal CompositeTerm pointFather = null;
        // Conflict Manager (per eseguire in un ambiente sicuro le gesture)
        public TransactionsManager transactionsManager = new TransactionsManager();
        // prova
        public String name;

        /* Metodi */
        /// <summary>
        /// Spara il token, se si è arrivati a questo caso allora il movimento dell'utente non rispetta completamente
        /// le condizioni previste dal term; tuttavia si mantiene ancora in vita la gesture.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="numError"></param>
        public virtual void fire(Token token, int numError)
        {
            this.likely(token);
        }

        /// <summary>
        /// Spara il token, se si è arrivati a questo punto allora il movimento rispetta le condizioni previste dal term.
        /// </summary>
        /// <param name="token"></param>
        public virtual void fire(Token token)
        {
            this.complete(token);
        }

        /// <summary>
        /// Reinizializzo il termine dell'espressione
        /// </summary>
        public virtual void reset()
        {
            this.num_total = this.num_discrete = 0;
            this.state = expressionState.Default;
        }

        /// <summary>
        /// Imposta lo stato dell'espressione come completo
        /// </summary>
        /// <param name="token"></param>
        public virtual void complete(Token token)
        {
            // Aggiorna i contatori e verifica se deve generare l'evento Complete
            this.num_total++;
            this.num_discrete++;
            // Aggiorna lo stato
            this.state = expressionState.Complete;
            // Genera gli eventi OnComplete e OnChangeState
            GestureEventArgs e = new GestureEventArgs(this, token);
            onComplete(e);
            onChangeState();
        }

        /// <summary>
        /// Imposta lo stato dell'espressione come "probabilmente". Per ora limitiamoci a comunicare che lo stato del term vien 
        /// modificato.
        /// </summary>
        /// <param name="token"></param>
        public virtual void likely(Token token)
        {
            // Aggiorna lo stato
            this.state = expressionState.Likely;
            // Genera gli eventi OnLikely, OnComplete e OnChangeState
            GestureEventArgs e = new GestureEventArgs(this, token);
            onLikely(e);
            onComplete(e);
            onChangeState();
        }

        /// <summary>
        /// Imposta lo stato dell'espressione come errore
        /// </summary>
        /// <param name="token"></param>
        public virtual void error(Token token)
        {
            // Resetta il contatore e verifica se deve generare l'evento Error
            this.num_discrete = 0;
            // Modifica lo stato
            this.state = expressionState.Error;
            // Genera gli eventi Error e ChangeState
            GestureEventArgs e = new GestureEventArgs(this, token);
            onError(e);
            onChangeState();
        }

        /// <summary>
        /// Verifica se il movimento dell'utente rispetta le condizioni del term o meno. Di default viene restituito true.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual bool lookahead(Token token)
        {
            if (token != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Genera l'evento Complete; viene richiamato quando lo stato del term va in complete. Provvede anche ad eseguire
        /// le modifiche alle variabili di stato in un ambiento protetto.
        /// </summary>
        /// <param name="t"></param>
        protected virtual void onComplete(GestureEventArgs t)
        {
            if (Complete != null)
            {
                // Genera l'evento Complete
                Complete(this, t);
                // Esegui in sicurezza l'eventuali modifiche alle variabili di stato.
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    //transactionsManager.onTransactionExcute(handler.elementList);
                    transactionsManager.onTransactionExcute(handler.listModifies, handler.elementList);
                    // Da l'ok per completare la transizione.
                    scope.Complete();
                }
                /*foreach (Handler h in handlers)
                {
                    if (h.elementList.Count > 0)
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            //transactionsManager.onTransactionExcute(handler.elementList);
                            transactionsManager.onTransactionExcute(h.listModifies, h.elementList);
                            // Da l'ok per completare la transizione.
                            scope.Complete();
                        }
                    }
                }*/
            }
        }

        /// <summary>
        /// Genera l'evento Likely; viene richiamato quando lo stato del term va in likely.
        /// </summary>
        /// <param name="t"></param>
        protected virtual void onLikely(GestureEventArgs t)
        {
            if(Likely != null)
            {
                Likely(this, t);
            }
        }

        /// <summary>
        /// Genera l'evento Error; viene richiamato quando lo stato del term va in error.
        /// </summary>
        /// <param name="t"></param>
        protected virtual void onError(GestureEventArgs t)
        {
            if (Error != null)
            {
                Error(this, t);
            }
        }

        /// <summary>
        /// Genera l'evento ChangeState, viene richiamato quando viene modificato lo stato del term.
        /// </summary>
        public void onChangeState()
        {
            if (this.ChangeState != null)
                this.ChangeState();
        }

        // IsTokenFire
        public virtual void IsTokenFire(TokenFireArgs sender)
        {
            if (this.TokenFire != null)
                this.TokenFire(this, sender);
        }

        /// <summary>
        /// Controlla se al term è stato associato una funzione di complete
        /// </summary>
        /// <returns></returns>
        public virtual bool hasComplete()
        {
            if (this.Complete != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Setta la funzione Complete del term e provvede a crearne l'handler
        /// </summary>
        /// <param name="func"></param>
        public virtual void setHandler(GestureEventHandler func, List<Modifies> listModifies)
        {
            this.Complete += func;
            this.handler = new Handler(func, this, listModifies);
            //this.handlers.Add(new Handler(func, this, listModifies));
        }
    }
}
