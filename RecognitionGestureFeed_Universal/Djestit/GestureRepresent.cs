using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    /// <summary>
    /// Questa classe rappresenta una singola gesture, contiene quindi al suo interno tutti i movimenti che la
    /// definiscono. A differenza della classe Choice, GestureRepresent contiene al suo interno le funzioni necessarie
    /// relative alla gestione degli errori dell'utente.
    /// </summary>
    public class GestureRepresent : Choice
    {
        /* Costruttori */
        /// <summary>
        /// Prende in ingresso un singolo parametro.
        /// </summary>
        /// <param name="terms"></param>
        public GestureRepresent(Term term) : base(term)
        {
            setErrorTollerance();
        }
        /// <summary>
        /// Prende in ingresso un'intera lista.
        /// </summary>
        /// <param name="terms"></param>
        public GestureRepresent(List<Term> terms) : base(terms)
        {
            setErrorTollerance();
        }

        /* Metodi */
        /// <summary>
        /// Per evitare modifiche ridondanti ogni variabile di tipo ErrorTollerance, contenuto in un composite term, viene associato
        /// a quello del composite term più in alto (solitamente un choice).
        /// </summary>
        public void setErrorTollerance()
        {
            foreach (CompositeTerm child in this.children.OfType<CompositeTerm>())
            {
                child.errorTolerance = this.errorTolerance;
            }
        }

        public override void reset()
        {
            this.errorTolerance.reset();
            this.state = expressionState.Default;
            foreach (var child in this.children)
            {
                child.reset();
            }
        }
    }
}
