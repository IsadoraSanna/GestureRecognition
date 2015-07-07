using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Acquisition
using RecognitionGestureFeed_Universal.Recognition;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack
{
    /* Dichiarazione Eventi */
    public delegate void TermUpdate(List<Term> GestureActive);

    public class Feedback
    {
        /* Eventi */
        public event TermUpdate TermUpdate;

        /* Attributi */
        // Albero dei Feedback costruito a partire dalle gesture
        public FeedbackRoot tree { get; private set; }

        List<Term> listExpressions = new List<Term>();
        List<Term> GestureActive = new List<Term>();
        //List<Term> listTermEvent = new List<Term>();

        /* Costruttore */
        public Feedback(Term expr)
        {
            //
            this.tree = new FeedbackRoot(expr);
            //
            //aquisitionManager.SkeletonFrameManaged += updateFeedback;
        }

        private void createList(CompositeTerm expression)
        {
            // Raccolgo tutte le espressioni in una lista
            foreach(var child in expression.children)
                listExpressions.Add(child);
        }

        private void updateFeedback(Skeleton sender)
        {
            if (listExpressions.Count == 0)
            {
                // Aggiorna Start
                foreach (Sequence sequence in listExpressions)
                {
                    if (sequence.children[0].state == expressionState.Complete)
                        GestureActive.Add(sequence);
                }
            }
            else
            {
                // Aggiorna Move
                foreach (Sequence sequence in GestureActive)
                {
                    if (sequence.children[1].state == expressionState.Error)
                        GestureActive.Remove(sequence);
                }
                // Comunica aggiornamento
                termUpdate();
            }
        }

        private void termUpdate()
        {
            if (this.TermUpdate != null)
                TermUpdate(this.GestureActive);
        }

        public void visitingTree()
        {
            this.tree.visitingTree();
        }
    }
}
