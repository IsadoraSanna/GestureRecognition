using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ErrorTolerance
using RecognitionGestureFeed_Universal.Djestit.ErrorToleranceManager;
// Handler
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;

namespace RecognitionGestureFeed_Universal.Djestit
{
    // Quando viene rilevato e tollerato un movimento errato dell'utente
    public delegate void GestureErrorTolerance();

    public class CompositeTerm : Term
    {
        /* Attributi */
        // Evento che descrive quando viene rilevato e gestito un errore nei movimento dell'utente
        public event GestureErrorTolerance ErrorDetect;
        // Contiene la lista di operandi da gestire
        public List<Term> children = new List<Term>();
        // Indica il massimo numero di errori tollerabili
        protected const int deltaError = 1;
        // Variabile che indica il numero di errori commessi
        private ErrorToleranceManager.ErrorTolerance error_tolerance = null;

        // Flag che indica se il CompositeTerm ha l'oggetto ErrorTolerance settato
        public bool flagErrTolerance { private set; get; }

        /* Costruttore */
        public CompositeTerm(List<Term> terms)
        {
            // Ad ogni figlio in input provvede a settare il puntatore al padre.
            foreach(Term term in terms)
            {
                term.pointFather = this;
            }
        }
        public CompositeTerm(Term term)
        {
            // Associa al figlio in input il puntatore al padre
            term.pointFather = this;
        }

        /* Metodi */
        /// <summary>
        /// Reset composite term
        /// </summary>
        public override void reset()
        {
	        this.state = expressionState.Default;
	        foreach(var child in this.children)
	        {
		        child.reset();
	        }
        }

        /// <summary>
        /// On error
        /// </summary>
        /// <param name="token"></param>
        public override void error(Token token)
        {
            foreach (var child in this.children)
            {
                child.error(token);
            }
            base.error(token);
        }

        /// <summary>
        /// Permette di settare la variabile ErrorTolerance
        /// </summary>
        /// <param name="err"></param>
        public void setErrorTolerance(ErrorTolerance err)
        {
            this.error_tolerance = err;
            this.flagErrTolerance = true;
        }
        /// <summary>
        /// Restituisce la variabile ErrorTolerance. Se il flag flagErrTolerance è settato a true restituisce il proprio 
        /// errorTolerance, altrimenti restituisce l'errorTolerance del padre. Se non ha un puntatore al padre, allora
        /// restituisce null.
        /// </summary>
        /// <returns></returns>
        public ErrorTolerance getErrorTolerance()
        {
            if (flagErrTolerance)
                return error_tolerance;
            else
            {
                if (pointFather != null)
                    return pointFather.getErrorTolerance();
                else
                    return null;
            }
        }
    }
}
