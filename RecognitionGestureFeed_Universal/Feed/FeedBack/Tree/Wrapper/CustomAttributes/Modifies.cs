using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Reflection
using System.Reflection;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes
{
    // 
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class Modifies : Attribute, ICloneable
    {
        /* Attributi */
        // Nome dell'oggetto
        internal string name { get; private set; }
        // Oggetto in questione
        internal object obj { get; private set; }
        // Rappresenta il nuovo valore che verrà assegnato all'elemento in seguito all'esecuzione di una gesture
        internal object newValue { get; private set; }
        // Rappresenta il valore assegnato all'oggetto in precedenza (serve per la gestione concorrenziale)
        internal object oldValue { get; private set; }
        // Rispettivamente nuovo e vecchio valore. Prova.
        internal float newv { get; private set; }
        internal float oldv { get; private set; }

        /* Costruttore */
        public Modifies(string name, float value)
        {
            this.name = name;
            this.newv = value;
        }
        // Creazione di un Modifies
        public Modifies(string name, object obj)
        {
            this.name = name;
            this.obj = obj;
        }
        // In seguito all'esecuzione di una gesture
        public Modifies(string name, object obj, object newValue)
        {
            this.name = name; // Nome
            this.obj = obj; // Object 
            this.newValue = newValue; // Nuovo Valore
        }

        /* Metodi */
        /// <summary>
        /// Clona l'oggeto di tipo Modifies
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        // Aggiorna il valore di un obj
        public void setAttr()
        {
            if (this.newValue != null)
            {
                this.oldValue = this.obj;// Conservo il vecchio valore
                this.obj = this.newValue;// Setto il nuovo valore
            }
        }
        public void setAttr(object newValue)
        {
            if (this.newValue != null)
            {
                this.newValue = newValue;
                setAttr();
            }
            else
            {
                // Comunico all'utente che non ha inserito un nuovo valore
                throw new InvalidModifiesException("Non è stato inserito nessun nuovo valore");
            }
        }
        // Ripristina il vecchio valore 
        public void riprAttr()
        {
            if (this.oldValue != null)
                this.obj = this.oldValue;
            else
            {    
                // Comunico all'utente che non ha inserito un nuovo valore
                throw new InvalidModifiesException("Non è presente alcun valore di ripristino");
            }
        }
    }

    // Comparer per la classe Modifies
    class ModifiesComparer : IEqualityComparer<Modifies>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Modifies x, Modifies y)
        {
            // Controlla se i due Modifies puntano alla stesso oggetto, restituisce true.
            if (Object.ReferenceEquals(x, y)) 
                return true;
            
            // Se uno dei due Modifies puntano ad un oggetto nullo, restituisce false.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            // Controlla se i due Modifies modificano lo stesso oggetto
            if ((x.obj != null && y.obj != null) && (x.obj == y.obj))
                return true;
            // O se hanno lo stesso nome
            if ((x.obj == null) || (y.obj == null) && (x.name == y.name))
                return true;

            return false;
        }

        // Calcola l'hashCode di un oggetto di tipo Modifies
        public int GetHashCode(Modifies element)
        {
            // Controlla se l'oggetto è nullo
            if (Object.ReferenceEquals(element, null)) return 0;

            // Se l'oggetto non è nullo allora calcolo l'hashcode dei singoli attributi dell'oggetto stesso
            int hashName = element.name == null ? 0 : element.name.GetHashCode();
            // Calcola l'hashcode dell'oggetto
            int hashObj = element.obj == null ? 0 : element.obj.GetHashCode();
            
            //Calculate the hash code for the product.
            return hashName ^ hashObj;
        }
    }

}