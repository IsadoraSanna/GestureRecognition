using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

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

        /* Costruttore */
        public Handler(String name, List<Modifies> list, GestureEventHandler function)
        {
            this.name = (string)name.Clone();
            this.elementList = list;
            this.function = (GestureEventHandler)function.Clone();
        }
        /* Metodi */
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
