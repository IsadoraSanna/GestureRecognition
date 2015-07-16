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

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper
{
    public class Handler : ICloneable
    {
        /* Attributi */
        //
        public string name { get; private set; }
        //
        public List<Modifies> elementList = new List<Modifies>();
        //
        public GestureEventHandler function;
        // Probabilità associata alla gesture
        public float likelihood { get; private set; }

        /* Costruttore */
        public Handler(string name, GestureEventHandler function, float likelihood)
        {
            this.name = (string)name.Clone();
            this.function = (GestureEventHandler)function.Clone();
            this.likelihood = likelihood;
            this.elementList = this.getModifiesAttribute();
        }

        /* Metodi */
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public List<Modifies> getModifiesAttribute()
        {
            return this.function.GetMethodInfo().GetCustomAttributes().OfType<Modifies>().ToList();
        }
    }

    public class HandlerComparer : IEqualityComparer<Handler>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Handler x, Handler y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.name == y.name;// && x.elementList.Equals(y.elementList) && x.function.Equals(y.function);
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Handler handler)
        {
            //
            if (Object.ReferenceEquals(handler, null)) return 0;

            // 
            int hashName = handler.name == null ? 0 : handler.name.GetHashCode();

            // 
            int hashList = handler.elementList.GetHashCode();

            //
            int hashFunction = handler.function.GetHashCode();

            //
            return hashName ^ hashList ^ hashFunction;
        }
    }
}
