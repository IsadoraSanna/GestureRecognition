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
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}