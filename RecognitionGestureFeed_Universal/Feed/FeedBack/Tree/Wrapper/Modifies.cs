using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper
{
    //
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct,
                       AllowMultiple = true)
    ]

    public class Modifies : System.Attribute, ICloneable
    {
        /* Attributi */
        // Nome dell'oggetto
        internal string name { get; private set; }
        // Nome del suo nuovo valore
        internal float value { get; private set; }

        /* Costruttore */
        public Modifies(string name, float value)
        {
            this.name = name; // Nome
            this.value = value; // Valore
        }

        /* Metodi */

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    // Custom comparer for the Product class
    class ModifiesComparer : IEqualityComparer<Modifies>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Modifies x, Modifies y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.value == y.value && x.name == y.name;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Modifies product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = product.name == null ? 0 : product.name.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = product.value.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }

}