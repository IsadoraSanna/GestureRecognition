using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Wrapper
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree
{
    public class FeedbackComposite : FeedbackGroup
    {
        /* Attributi */
        // Lista dei GroundTerm
        public List<FeedbackLeaf> groundTermChildren = new List<FeedbackLeaf>();
        
        /* Costruttore */
        public FeedbackComposite(Term term) : base(term)
        {
            // Associo all'evento TokenFire il suo handler
            this.term.TokenFire += updateChild;
            // Associo all'evento ChangeState il suo handler
            this.term.ChangeState += updateTerm;
        }

        /// <summary>
        /// Controlla lo stato dei figli, dopo un ChangeState. Il FeedbackComposite viene posto
        /// in uno stato di Continue, se almeno uno dei suoi figli è in uno stato di Complete.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        private void updateChild(object obj, TokenFireArgs sender)
        {
            if (this.groundTermChildren.Where(this.IsComplete).Count() > 0 && this.state != StateGroup.Complete && this.state != StateGroup.Error)
            {
                this.state = StateGroup.Continue;
                OnFeedbackGroupContinue();
            }
        }
    }
}
