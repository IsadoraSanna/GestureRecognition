using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ErrorTolerance.ErrorTolerance errorTolerance
        {
            // Get: se il Composite Term rappresenta la gesture completa, allora ritorna il suo ErrorTolerance, altrimenti
            // ritorna l'ErrorTolerance del padre.
            get
            {
                if (flagErrTolerance)
                {
                    return errorTolerance;
                }
                else
                    return pointFather.errorTolerance;
            }
            // Set: Nel momento in cui imposta l'ErrorTolerance, setta il flag a true
            set
            {
                this.flagErrTolerance = true;
            }
        }
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
        /// Restituisce l'oggetto di tipo il ErrorTolerance del padre se il flag non è settato oppure il proprio, se il composite
        /// term descrive la gesture.
        /// </summary>
        /// <returns></returns>
        public ErrorTolerance.ErrorTolerance getErrorTolerance()
        {
            if (flagErrTolerance)
                return this.errorTolerance;
            else
                return pointFather.getErrorTolerance();
        }
    }
}
