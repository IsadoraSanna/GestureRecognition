using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class TokenFireArgs : EventArgs
    {
        /* Attributi */
        Token token;
        Term term;

        /* Costruttore */
        public TokenFireArgs()
        {

        }
    }
}
