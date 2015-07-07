using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack
{
    public class FeedbackRoot : FeedbackNode
    {
        /* Attributi */
        // children conterrà tutte le gesture presenti nell'espressione passata in input
        public List<FeedbackGroup> children { get; private set; }

        /* Costruttore */
        public FeedbackRoot(Term term) : base(term)
        {
            /// Se il term ha degli altri elementi al suo interno (e quindi è un compositeTerm) allora 
            /// provvede a creare i nuovi rami dell'albero. Altrimenti se al suo interno ha un'unica gesture,
            /// allora provvede a creare un unico figlio.
            if (this.children.Count == 0)
            {
                this.children.Add(new FeedbackGroup(term));
            }
            
            term.TokenFire += update;
        }
        
        /* Metodi */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        protected override void update(object obj, TokenFireArgs sender)
        {
            foreach(FeedbackGroup child in this.children.Where(ChildFalse))
            {
                child.update(obj, sender);
            }
            foreach(FeedbackGroup child in this.children.Where(ChildFalse))
            {
                TokenFireArgs args = new TokenFireArgs();
                Debug.WriteLine("Porcamadonna");
            }
        }

        /* Metodi */
        public void visitingTree()
        {
            foreach(FeedbackGroup child in this.children)
            {
                child.visitingTree();
            }
        }
    }
}
