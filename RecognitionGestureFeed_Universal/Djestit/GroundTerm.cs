using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace Unica.Djestit
{
    //
    public delegate bool Accepts<T>(T token) where T : Token;

    public class GroundTerm : Term
    {
        /* Attributi */
        public String Type { get; set; }// Tipo di ground term
        public Accepts<Token> Accepts { get; set; }
        public Accepts<Token> _Accepts { get; set; }

        /* Metodi */
        public virtual bool _accepts(Token token)
        {
            return true;
        }

        public override bool lookahead(Token token)
        {
            if(Accepts != null)
            {
                return _Accepts(token) && Accepts(token);
            }
            if(_Accepts != null)
            {
                return _Accepts(token);
            }
            return false;
        }
    }
    
}
