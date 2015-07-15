using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper
{
    //
    public delegate void Handler<T, S>(T obj, S sender) where S : GestureEventArgs;

    public class Handler
    {
        /* Attributi */
        //
        public string nome { get; private set; }
        //
        public List<Modifies> elementList = new List<Modifies>();
        //
        public Handler function { get; private set; }

        /* Costruttore */
        public Handler(String nome, List<Modifies> list, Handler function)
        {
            this.nome = nome;
            this.elementList = list;
            this.function = function;
        }

        /* Metodi */

    }
}
