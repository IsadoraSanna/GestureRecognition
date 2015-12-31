using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    public class Iterative : CompositeTerm
    {

        //COSTRUTTORI
        //creo 2 costruttori invece che solo uno come nel JS dato che è troppo tipato
        public Iterative(Term term) : base(term)
        {
            this.children = new List<Term>();
            this.children.Add(term);
        }

        public Iterative(List<Term> terms) : base(terms)
        {
            this.children = new List<Term>();
            this.children.Add(terms.First());
        }

        public override void reset()
        {
            this.state = expressionState.Default;
            this.children[0].reset();
        }

        public override bool lookahead(Token token)
        {

            if (this.children != null && this.children[0].lookahead(token))
                return this.children[0].lookahead(token);
            else
                return false;
        }

        /*
        public override void fire(Token token)
        {
            if(this.lookahead(token))
            {
                this.children[0].fire(token);
                switch (this.children[0].state)
                {
                    case expressionState.Complete:
                        this.complete(token);
                        this.children[0].reset();
                        break;
                    case expressionState.Error:
                        this.error(token);
                        this.children[0].reset();
                        break;
                }
            }
            //
            TokenFireArgs args = new TokenFireArgs(token, this);
            IsTokenFire(args);
        }*/

        /* Error Tolerance Manager */
        public override void fire(Token token)
        {
            if (this.lookahead(token))
            {
                this.children[0].fire(token);
                switch (this.children[0].state)
                {
                    case expressionState.Error:
                        // Se viene rilevato un errore si aumenta il contatore dell'apposito gestore, e si verifica se è stata 
                        // superata la massima soglia "tollerabile".
                        this.errorTolerance.errorDetect();
                        if (this.errorTolerance.numError > deltaError)
                        {
                            this.error(token);
                            break;
                        }
                        this.state = expressionState.Likely;
                        break;
                    case expressionState.Complete:
                        this.complete(token);                        
                        break;
                }
            }
            this.children[0].reset();// Resetta lo stato del figlio
            // Comunica che il token è stato gestito (genera l'evento TokenFire)
            TokenFireArgs args = new TokenFireArgs(token, this);
            IsTokenFire(args);
        }
    }
}
