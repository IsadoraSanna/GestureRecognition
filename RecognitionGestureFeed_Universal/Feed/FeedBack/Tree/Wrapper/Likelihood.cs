using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper
{
    // Enum che indica i diversi tipi di probabilità
    public enum ProbabilityType
    {
        composite = 1,
        simple = 0
    }

    public static class Likelihood
    {
        /* Attributi */

        /* Metodi */
        public static void determineLikelihood(this FeedbackGesture node, ProbabilityType type)
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

        private static void composite(FeedbackGesture node)
        {
            float tempLikelihood = 1;

            foreach (FeedbackLeaf child in node.groundTermChildren)
            {
                tempLikelihood = tempLikelihood * child.likelihood;
            }

            node.likelihood = tempLikelihood;
        }

        private static void simple(FeedbackGesture node)
        {
            node.likelihood = node.term.likelihood;
        }
    }
}
