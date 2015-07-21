using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood
{
    // Enum che indica i diversi tipi di probabilità
    public enum ProbabilityType
    {
        composite = 1,
        simple = 0
    }

    public class Likelihood
    {
        /* Attributi */
        public float likelihood { get; private set; }

        /* Costruttore */
        public Likelihood()
        {
            this.likelihood = 0.0f;
        }
        public Likelihood(float value)
        {
            this.likelihood = value;
        }
        public Likelihood(FeedbackGesture node, ProbabilityType type)
        {
            switch (type)
            {
                case ProbabilityType.composite:
                    // Probabilità composta
                    composite(node);
                    break;
                case ProbabilityType.simple:
                    // Probabilità semplice
                    simple(node);
                    break;
            }
        }

        /* Metodi */
        /// <summary>
        /// Aggiorna la probabilità
        /// </summary>
        /// <param name="value"></param>
        public void updateLikelihood(float value)
        {
            // La probabilità di una gesture è ovviamente sempre compresa tra 0 e 1
            if ((value > 0) && (value < 1))
                this.likelihood = value;
            else if (value > 1)
                this.likelihood = 1f;
            else
                this.likelihood = 0;
        }

        /// <summary>
        /// Probabilità composta calcolata come il prodotto della probabilità dei singoli GroundTerm che 
        /// compongono la gesture
        /// </summary>
        /// <param name="node"></param>
        private void composite(FeedbackGesture node)
        {
            float tempLikelihood = 1;

            foreach (FeedbackLeaf child in node.groundTermChildren)
            {
                tempLikelihood = tempLikelihood * child.likelihood.likelihood;
            }

            // Inserisce la probabilità nel nodo
            this.likelihood = tempLikelihood;
        }

        /// <summary>
        /// Probabilità semplice, ovvero la probabilità data a priori alla gesture
        /// </summary>
        /// <param name="node"></param>
        private void simple(FeedbackGesture node)
        {
            // Inserisce la probabilità nel nodo
            this.likelihood = node.term.likelihood.likelihood;
        }
    }
}
