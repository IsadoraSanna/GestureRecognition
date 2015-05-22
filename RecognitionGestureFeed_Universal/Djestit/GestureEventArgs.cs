using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class GestureEventArgs : EventArgs
    {
        private readonly Term t;

        public GestureEventArgs(Term t)
        {
            this.t = t;
        }
    }
}
