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
        Eerror = -1
    }
    // Delegate per i GestureEventHandler
    public delegate void GestureEventHandler(Skeleton skeleton);

    public class Term
    {
        public expressionState state = expressionState.Default;
        public event GestureEventHandler onComplete;
        public event GestureEventHandler onError;  

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
		    this.state = expressionState.Eerror;
		    //onError.trigger(“error”, token);
        }

        //verifica se l'imput puo' essere accettato o no
        public bool lookahead(Token token){
	        if(token != null){
		        return true;
	        }
            return false;
        }
    }
}
