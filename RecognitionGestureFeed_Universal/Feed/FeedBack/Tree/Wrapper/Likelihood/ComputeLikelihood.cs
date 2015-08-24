using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood
{
    public static class ComputeLikelihood
    {
        /* Attributi */
        private static int n = 10;

        /* Metodi */
        /// <summary>
        /// Aggiorna la probabilità
        /// </summary>
        /// <param name="value"></param>
        public static float updateLikelihood(float value)
        {
            // La probabilità di una gesture è ovviamente sempre compresa tra 0 e 1
            if ((value > 0) && (value < 1))
                return value;
            else if (value > 1)
                return 1f;
            else
                return 0;
        }

        //// <summary>
        /// Probabilità composta calcolata come il prodotto della probabilità dei singoli GroundTerm che 
        /// compongono la gesture
        /// </summary>
        /// <param name="node"></param>  
        public static float indipendentEvents(CompositeTerm term)
        {
            float tempLikelihood = 1;

            if ((term.GetType() == typeof(Sequence)) || (term.GetType() == typeof(RecognitionGestureFeed_Universal.Djestit.Parallel)) || (term.GetType() == typeof(Disabling)))
            {
                foreach (var child in term.children)
                    tempLikelihood *= child.likelihood;
            }
            else if(term.GetType() == typeof(OrderIndependece))
            {
                foreach (var child in term.children)
                    tempLikelihood *= child.likelihood;
                tempLikelihood *= term.children.Count;
            }
            else if(term.GetType() == typeof(Iterative))
            {
                tempLikelihood = (float)Math.Pow(term.children[0].likelihood, n);
            }
            else if(term.GetType() == typeof(Choice))
            {
                List<float> list= new List<float>();

                foreach (var child in term.children) 
                    list.Add(child.likelihood);

                tempLikelihood = list.Max();
            }

            return tempLikelihood;
        }
    }
}
