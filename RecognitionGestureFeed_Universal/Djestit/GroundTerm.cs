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

        /* Metodi 
        public virtual bool _accepts2(Token token)
        {
            if(this._accepts != null) 
                return this._accepts(token); 
            else 
                return true;
        }

        public virtual bool accepts2(Token token)
        {
            if (this.Accepts != null)
                return this.Accepts(token);
            else
                return true;
        }*/

        protected virtual bool _Accepts(Token token)
        {
            return true;
        }
       
        public override bool lookahead(Token token)
        {
            if(Accepts != null)
            {
                return _Accepts(token) && Accepts(token);
            }
            else
            {
                return _Accepts(token);
            }
        }
    }
    
}
