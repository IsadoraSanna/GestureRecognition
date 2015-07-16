using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Wrapper
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper;
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree
{
    public class FeedbackRoot
    {
        /* Attributi */
        // Lista delle Gesture presenti nel term passato in input
        public List<FeedbackGesture> children = new List<FeedbackGesture>();
        // Term assoicato alla radice
        public Term term;
        // Map 
        public Dictionary<Handler, List<Modifies>> mapHandler { get; private set; }

        /* Costruttore */
        public FeedbackRoot(Term term)
        {
            // Associo il Term alla radice
            this.term = term;

            if (term.GetType() != typeof(GroundTerm))
            {
                CompositeTerm compositeTerm = (CompositeTerm)term;
                // Creo un FeedbackGroup per ogni sottocomponente di term e lo metto nella lista children
                foreach (var child in compositeTerm.children)
                {
                    // Lo aggiungo nella lista dei children
                    this.children.Add(createFeedbackGesture(child));
                }
            }
            else
            {
                // Se è invece un Ground Term allora lo aggiungo semplicemente nella lista
                this.children.Add(createFeedbackGesture(term));
            }

            // Inizializzo la mappa degli handler
            this.mapHandler = new Dictionary<Handler, List<Modifies>>();
            // Associa al cambiamento di stato del term l'handler resetTree
            this.term.ChangeState += resetTree;
        }

        /* Metodi */
        private void reset()
        {
            foreach (FeedbackGesture feedbackGesture in this.children)
            {
                feedbackGesture.reset();
                this.mapHandler.Clear();
            }
        }

        private void resetTree()
        {
            this.reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private FeedbackGesture createFeedbackGesture(Term term)
        {
            // Crea il nuovo FeedbackGesture
            FeedbackGesture feedbackGesture = new FeedbackGesture(term);
            // Gestisce gli eventi del FeedbackGesture
            feedbackGesture.FeedbackGroupComplete += updateComplete;
            feedbackGesture.FeedbackGroupContinue += updateContinue;
            feedbackGesture.FeedbackGroupError += updateError;

            return feedbackGesture;
        }

        /// <summary>
        /// Se una Gesture è stata completata, allora resetto l'albero e cancello la mappa
        /// </summary>
        /// <param name="sender"></param>
        private void updateComplete(FeedbackGroupEventArgs sender)
        {
            this.reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void updateContinue(FeedbackGroupEventArgs sender)
        {
            // Se l'handler non è ancora presente nella mappa, allora lo inserisco, e aggiorno l'albero
            if (!this.mapHandler.ContainsKey(sender.handler))
            {
                // Aggiorna l'albero
                this.addNode(sender.handler);//(Handler)sender.handler.Clone());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void updateError(FeedbackGroupEventArgs sender)
        {
            // Se una gesture va in errore, e se il suo handler era nella mappa, aggiorno la mappa.
            if (this.mapHandler.ContainsKey(sender.handler))
            {
                this.mapHandler.Remove(sender.handler);
                this.removeNode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newHandler"></param>
        private void addNode(Handler newHandler)
        {
            List<Modifies> listModifies = new List<Modifies>(newHandler.elementList);

            foreach (var i in this.mapHandler)
            {
                //
                foreach (Modifies element in i.Value.Intersect(newHandler.elementList).ToList())
                    i.Value.Remove(element);

                //
                listModifies = listModifies.Except(i.Key.elementList).ToList();
            }

            //
            this.mapHandler.Add(newHandler, listModifies);
            //
            this.mapHandler.OrderBy(key => key.Key.likelihood);
        }

        /// <summary>
        /// 
        /// </summary>
        private void removeNode()
        {
            List<Modifies> listModifies = new List<Modifies>();

            // Prima si determina quali sono i nuovi attributi in conflitto
            foreach (var i in this.mapHandler)
            {
                if (listModifies.Count > 0)
                    listModifies.Union(i.Key.elementList);
                else
                    listModifies.Intersect(i.Key.elementList);
            }

            // Quindi si fa l'union con la lista degli elementi di partenza e poi se ne fa la differenza
            foreach (var i in this.mapHandler)
            {
                foreach (Modifies element in i.Key.elementList.Except(listModifies).Except(i.Value).ToList())
                {
                    i.Value.Add(element);
                }
            }

            //
            this.mapHandler.OrderBy(key => key.Key.likelihood);
        }

        // Prova
        public void print()
        {
            Debug.WriteLine("It's Time of Porcoddio");
            foreach (var child in this.mapHandler)
            {
                Debug.WriteLine("Porcamadonna a Dio: - " + child.Key.name);
            }
        }
    }
}
