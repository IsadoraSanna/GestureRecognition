using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    
    public class GroundTerm : Term
    {
        private String type = "ground";
        //private qualcosa modality = null; per JS this.modality = undefined;
        //non ho minimamente capito l'utilità di questa classe

        public virtual bool _accepts(Token token)
        {
            //if(token != null) return true; else return false;
            return true;
        }

        public virtual bool accepts(Token token)
        {
            //if(token != null) return true; else return false;
            return true;
        }

        public bool lookahead(Token token)
        {
            return (this._accepts(token) && this.accepts(token));
        }
    }
    
}
