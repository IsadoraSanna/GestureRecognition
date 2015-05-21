using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    // Enum expressionState
    public enum expressionState
    {
        Complete = 1,
        Default = 0,
        Error = -1
    }
    // Delegate per i GestureEventHandler
    public delegate void GestureEventHandler(Term t);

    public class Term
    {
        public expressionState state = expressionState.Default;
        public event GestureEventHandler Complete;
        public event GestureEventHandler Error;  

        public bool excluded;
        public bool once;

        public void fire(Token token)
        {
            this.complete(token);
        }

        //reinizializzo il termine dell'espressione
        public void reset()
        {
            this.state = expressionState.Default;
        }

        //imposto lo stato dell'espressione come completo
        public void complete(Token token){
		    this.state = expressionState.Complete;
            //onComplete.trigger(“completed”, token);
        }

        //imposto lo stato dell'espressione come errore
        public void error(Token token){
		    this.state = expressionState.Error;
		    //onError.trigger(“error”, token);
        }

        //verifica se l'imput puo' essere accettato o no
        public bool lookahead(Token token){
	        if(token != null){
		        return true;
	        }
            return false;
        }

        public virtual void onComplete(Term t)
        {
            t.state = expressionState.Complete;
            GestureEventHandler handler = Complete;
            if (handler != null)
            {  
                handler(t);
            }
        }

        public virtual void onError(Term t)
        {
            t.state = expressionState.Error;
            GestureEventHandler handler = Error;
            if (handler != null)
            {
                handler(t);
            }
        }
 
    }
}
