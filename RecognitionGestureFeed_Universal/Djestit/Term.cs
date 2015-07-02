using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Debug
using System.Diagnostics;

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
    public delegate void GestureEventHandler(object sender, GestureEventArgs t);

    public class Term
    {
        /* Eventi */
        public event GestureEventHandler Complete;
        public event GestureEventHandler Error;  
        /* Attributi */
        public expressionState state = expressionState.Default;
        public bool excluded;
        public bool once;
        // Indica quante volte è stato eseguito il Term in questione, da quando il programma è stato avviato
        public int num_total {get; private set;}
        // Indica il numero di volte consecutive con cui è stato eseguito il Term in questione
        public int num_discrete { get; private set; }

        /* Metodi */
        public virtual void fire(Token token)
        {
            this.complete(token);
        }

        // Reinizializzo il termine dell'espressione
        public virtual void reset()
        {
            this.num_total = this.num_discrete = 0;
            this.state = expressionState.Default;
        }

        // Imposta lo stato dell'espressione come completo
        public void complete(Token token){
            // Aggiorna i contatori e genera l'evento Complete
            this.num_total++;
            this.num_discrete++;
		    this.state = expressionState.Complete;
            GestureEventArgs e = new GestureEventArgs(this, token);
            onComplete(e);
        }

        // Imposta lo stato dell'espressione come errore
        public void error(Token token){
            this.num_discrete = 0;
		    this.state = expressionState.Error;
            GestureEventArgs e = new GestureEventArgs(this, token);            
            onError(e);
        }

        // Verifica se l'imput puo' essere accettato o no
        public virtual bool lookahead(Token token)
        {
	        if(token != null){
		        return true;
	        }
            return false;
        }

        public virtual void onComplete(GestureEventArgs t)
        {
            if (Complete != null)
            {
                Complete(this, t);
            }
        }

        public virtual void onError(GestureEventArgs t)
        {            
            if (Error != null)
            {
                Error(this, t);
            }
        }
 
    }
}
