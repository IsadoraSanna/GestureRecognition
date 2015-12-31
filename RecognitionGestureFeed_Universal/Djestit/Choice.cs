using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Djestit
{
    /**
     * A composite expression of terms connected with the choice operator.
     * The sequence operator expresses that it is possible to select one among 
     * the terms in order to complete the whole expression.
     * The implementation exploits a best effort approach for dealing with the 
     * selection ambiguity problem (see [1])
     */
    public class Choice : CompositeTerm
    {
        /* Costruttori */
        /// <summary>
        /// Prende in ingresso un singolo parametro.
        /// </summary>
        /// <param name="term"></param>
        public Choice(Term term) : base(term)
        {
            this.children = new List<Term>();
            this.children.Add(term);
        }
        /// <summary>
        /// Prende in ingresso un'intera lista.
        /// </summary>
        /// <param name="terms"></param>
        public Choice(List<Term> terms) : base(terms)
        {
            this.children = new List<Term>();
            this.children = terms;
        }

        /* Metodi */
        public override void reset()
        {
            this.state = expressionState.Default;
            foreach (Term child in this.children)
            {
                child.reset();
                child.excluded = false;
            }
        }

        public override bool lookahead(Token token)
        {
            
            if (this.state == expressionState.Complete|| this.state == expressionState.Error)
                return false;
            if (this.children != null && this.children.GetType() == typeof(List<Term>))
            {
                for (int index = 0; index < this.children.Count; index++)
                {
                    if (!this.children[index].excluded && this.children[index].lookahead(token) == true)
                        return true;
                }
            }
            return false;

        }

        public virtual void feedToken(Token token)
        {
            if (this.state == expressionState.Complete || this.state == expressionState.Error)
                return;

            if (this.children != null && this.children.GetType() == typeof(List<Term>))
            {
                for (int index = 0; index < this.children.Count; index++)
                {
                    if (!this.children[index].excluded)
                    {
                        if (this.children[index].lookahead(token))
                        {
                            this.children[index].fire(token);
                        }
                        else
                        {
                            // E' stato rilevato un errore; si verifica se può essere "tollerato" oppure no
                            errorTolerance.errorDetect();//getErrorTolerance().errorDetect();
                            // Se è stato già tollerato un numero massimo di errori, allora si pone excluded a true e in error
                            // il term in questione; altrimenti quest'ultimo viene posto in likely e si va avanti.
                            if(!(getErrorTolerance().numError > deltaError))
                            {
                                // The current sub-term is not able to handle the input sequence
                                this.children[index].excluded = true;
                                this.children[index].error(token);
                            }
                            this.children[index].likely(token);                            
                        }
                    }
                }
            }
        }

        public override void fire(Token token)
        {
            this.feedToken(token);
            bool allExcluded = true;

            for (int index = 0; index < this.children.Count; index++)
            {
                if (!this.children[index].excluded)
                {
                    allExcluded = false;
                    switch (this.children[index].state)
                    {
                        // one of the subterms is completed or likely, then the entire expression is completed
                        case expressionState.Likely:
                        case expressionState.Complete:
                            this.complete(token);
                            return;
                        case expressionState.Error:
                            // this case is never executed, since
                            // feedToken excludes the subterms in error state
                            return;
                    }
                }
            }
            if (allExcluded)
            {
                this.error(token);
                this.reset();
            }
            
            // Comunica che è stato sparato un token
            TokenFireArgs args = new TokenFireArgs(token, this);
            IsTokenFire(args);
        }/*
        public void feedToken(Token token)
        {
            if (this.state == expressionState.Complete || this.state == expressionState.Error)
                return;

            if (this.children != null && this.children.GetType() == typeof(List<Term>))
            {
                for (int index = 0; index < this.children.Count; index++)
                {
                    if (!this.children[index].excluded)
                    {
                        if (this.children[index].lookahead(token))
                        {
                            this.children[index].fire(token);
                        }
                        else
                        {
                            // the current sub-term is not able to handle the input
                            // sequence
                            this.children[index].excluded = true;
                            this.children[index].error(token);
                        }
                    }
                }
            }
        }

        public override void fire(Token token)
        {
            this.feedToken(token);
            bool allExcluded = true;

            for (int index = 0; index < this.children.Count; index++)
            {
                if (!this.children[index].excluded)
                {
                    allExcluded = false;
                    switch (this.children[index].state)
                    {
                        case expressionState.Complete:
                            // one of the subterms is completed, then the entire expression is completed
                            this.complete(token);
                            return;
                        case expressionState.Error:
                            // this case is never executed, since
                            // feedToken excludes the subterms in error state
                            return;
                    }
                }
            }
            if (allExcluded)
            {
                this.error(token);
                this.reset();
            }

            // Comunica che è stato sparato un token
            TokenFireArgs args = new TokenFireArgs(token, this);
            IsTokenFire(args);
        }*/
    }
}


/* 
        public void feedToken(Token token)
        {
            if (this.state == expressionState.Complete || this.state == expressionState.Error)
                return;

            if (this.children != null && this.children.GetType() == typeof(List<Term>))
            {
                for (int index = 0; index < this.children.Count; index++)
                {
                    if (!this.children[index].excluded)
                    {
                        if (this.children[index].lookahead(token))
                        {
                            this.children[index].fire(token);
                        }
                        else
                        {
                            // Tollera un altro elemento

                            // the current sub-term is not able to handle the input
                            // sequence
                            this.children[index].excluded = true;
                            this.children[index].error(token);
                        }
                    }
                }
            }
        }

        public override void fire(Token token)
        {
            this.feedToken(token);
            bool allExcluded = true;

            for (int index = 0; index < this.children.Count; index++)
            {
                if (!this.children[index].excluded)
                {
                    allExcluded = false;
                    switch (this.children[index].state)
                    {
                        case expressionState.Complete:
                            // one of the subterms is completed, then the entire expression is completed
                            this.complete(token);
                            return;
                        case expressionState.Error:
                            // this case is never executed, since
                            // feedToken excludes the subterms in error state
                            return;
                    }
                }
            }
            if (allExcluded)
            {
                this.error(token);
            }
            
            // Comunica che è stato sparato un token
            TokenFireArgs args = new TokenFireArgs(token, this);
            IsTokenFire(args);
        }
*/
