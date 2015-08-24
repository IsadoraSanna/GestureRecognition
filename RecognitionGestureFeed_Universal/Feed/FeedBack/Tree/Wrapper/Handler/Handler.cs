using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Reflection
using System.Reflection;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Likelihood
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler
{
    public class Handler : ICloneable, IComparable<Handler>
    {
        /* Attributi */
        // Nome della funzione associata a questo Handler
        public string name { get; private set; }
        // Lista dei custom attributes di tipo Modifies dichiarati nella funzione
        public List<Modifies> elementList = new List<Modifies>();
        // Funzione che gestisce la gesture
        public GestureEventHandler function;
        // Probabilità associata alla gesture
        //public Likelihood.Likelihood likelihood { get; internal set; }
        public float likelihood;

        /* Costruttore */
        public Handler(GestureEventHandler function)
        {
            // Probabilità
            this.likelihood = 0.0f;
            // Funzione 
            this.function = (GestureEventHandler)function.Clone();
            // Lista dei modifies modificati dalla funzione della gesture
            this.elementList = this.getModifiesAttribute();
            // Nome associata alla funzione
            this.name = function.Method.Name;
        }
        /// <summary>
        /// Costruttore che inizializza a zero (o a null), tutti gli elementi dell'handler
        /// </summary>
        public Handler()
        {
            this.function = null;
            //this.likelihood = new Likelihood.Likelihood(); 
        }

        /* Metodi */
        /// <summary>
        /// Clona l'Handler
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            // Clona i suoi membri
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Prende gli attributi della funzione, sfruttando la funzione GetCustomAttributes, 
        /// e li trasforma in una lista.
        /// </summary>
        /// <returns></returns>
        public List<Modifies> getModifiesAttribute()
        {
            return this.function.GetMethodInfo().GetCustomAttributes().OfType<Modifies>().ToList();
        }

        /// <summary>
        /// Funzione che compara due Handler tra loro.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Handler other)
        {
            return (other.likelihood.CompareTo(this.likelihood));//(other.likelihood.probability.CompareTo(this.likelihood.probability));
        }
    }

    /// <summary>
    /// Comparer della classe Handler
    /// </summary>
    public class HandlerComparer : IEqualityComparer<Handler>
    {
        // Funzione che verfica l'equalità tra due Handler
        public bool Equals(Handler x, Handler y)
        {
            // Controlla per prima cosa se i due handler puntano allo stesso oggetto
            // in tal caso restituisce vero.
            if (Object.ReferenceEquals(x, y)) return true;

            // Controlla se uno dei due oggetti è null, in tal caso
            // restituisce false.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            // Controlla se le due funzioni sono equivalenti, in tal caso restituisce true
            return x.function.Equals(y.function);
        }

        // Restituisce l'HashCode di un Handler
        public int GetHashCode(Handler handler)
        {
            // Se l'handler non punta a nulla restituisce zero
            if (Object.ReferenceEquals(handler, null)) return 0;

            // Calcola l'hashcode del nome
            int hashName = handler.name == null ? 0 : handler.name.GetHashCode();

            // Calcola l'hashcode della lista di elementi
            int hashList = handler.elementList.GetHashCode();

            // Calcola l'hashcode dell'handler associata alla gesture
            int hashFunction = handler.function.GetHashCode();

            // Calcola l'hashcode della probabilità associata alla gesture
            int hashLikelihood = handler.likelihood.GetHashCode();

            // Calcola l'hashcode di tutto l'oggetto
            return hashName ^ hashList ^ hashFunction ^ hashLikelihood;
        }
    }
}
