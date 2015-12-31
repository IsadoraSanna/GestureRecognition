using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit.ErrorTolerance
{
    /// <summary>
    /// Classe che descrive come e in che modo vengono tollerati gli errori nei movimenti dell'utente.
    /// </summary>
    public class ErrorTolerance
    {
        /* Attributi */
        // Variabile che indica il numero di errori commessi
        public int numError { get; private set; }

        /* Costruttore */
        public ErrorTolerance()
        {
            // Inizializza Variabili
            this.numError = 0;
        }

        /* Metodi */
        /// <summary>
        /// E' stato rilevato un errore, si aggiorna la variabile che li conta.
        /// </summary>
        public void errorDetect()
        {
            this.numError++;
        }
        /// <summary>
        /// Resetta a zero la variabile che conta gli errori.
        /// </summary>
        public void reset()
        {
            this.numError = 0;
        }
    }
}
