﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Wrapper
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper;
// CustomAttributes
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Handler
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;
// Likelihood
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Likelihood;
// Conflict Manager
using RecognitionGestureFeed_Universal.Feed.FeedBack.Conflict;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree
{
    // Delegate per gli eventi legati al cambiamento di stato di una gesture
    public delegate void FeedbackRootEvent();

    public class FeedbackRoot
    {
        /* Eventi */
        public event FeedbackRootEvent feedbackRootEvent;
        /* Attributi */
        // Lista delle Gesture presenti nel term passato in input
        public List<FeedbackGesture> children = new List<FeedbackGesture>();
        // Term associato alla radice
        public Term term;
        // Conflict Manager associato alla radice
        public ConflictManager conflictManager = null;

        // SortedDictionary che contiene le informazioni relative alle gesture in stato di Continue
        //public SortedDictionary<Handler, List<Modifies>> mapHandler { get; private set; }

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

            // Inizializzo la mappa degli handler, inserendovi la relativa classe che si occupa del Compare tra i vari handler
            //this.mapHandler = new SortedDictionary<Handler, List<Modifies>>();
            this.conflictManager = new ConflictManager(term);

            // Associa al cambiamento di stato del term l'handler resetTree
            this.term.ChangeState += resetTree;
        }

        /* Metodi */
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
        /// Quando una Gesture va in uno stato di Continue, si provvede ad inserire il suo Handler dentro
        /// la map. 
        /// </summary>
        /// <param name="sender"></param>
        private void updateContinue(FeedbackGroupEventArgs sender)
        {
            // Se l'handler non è ancora presente nella mappa, allora lo inserisco, e aggiorno l'albero
            if (!this.conflictManager.mapConflictExec.ContainsKey(sender.handler))
            {
                // Aggiorna l'albero inserendo l'handler.
                this.conflictManager.addHandler(sender.handler);
            }
        }

        /// <summary>
        /// Quando una Gesture va in uno stato di Error, e se il suo handler si trova nella map, allora
        /// si aggiorna la map.
        /// </summary>
        /// <param name="sender"></param>
        private void updateError(FeedbackGroupEventArgs sender)
        {
            // Controlla se l'handler si trova nella mappa
            if (this.conflictManager.mapConflictExec.ContainsKey(sender.handler))
            {
                // Se si, lo si rimuove dalla mappa e si aggiornano di conseguenza anche tutti gli 
                // elementi dei vari handler presenti nella mappa.
                //this.mapHandler.Remove(sender.handler);
                //this.removeNode();
                this.conflictManager.removeHandler(sender.handler);
            }
        }

        /*/// <summary>
        /// Inserisce un handler nella map.
        /// </summary>
        /// <param name="newHandler"></param>
        private void addNode(Handler newHandler)
        {
            // Lista di Modifies temporanea, costruita a partire dai CustomAttributes del handler che andrà
            // inserita nella mappa
            List<Modifies> listModifies = new List<Modifies>(newHandler.elementList);

            // Quindi per ogni handler presente nella mappa, si prende la sua lista di CustomAttribute,
            // e si controlla se va ad agire su attributi presenti anche negli altri handler
            foreach (var i in this.mapHandler)
            {
                // Toglie dalla lista di Modifies associato all'Handler gli eventuali elementi in conflitto
                // ovvero quelli contenuti nell'handler della nuova gesture
                foreach (Modifies element in i.Value.Intersect(newHandler.elementList).ToList())
                    i.Value.Remove(element);

                // Contemporaneamente aggiorna anche la lista di Modifies del nuovo handler che dovrà essere
                // inserito
                listModifies = listModifies.Except(i.Key.elementList).ToList();
            }

            // Inserisce l'handler nella mappa
            this.mapHandler.Add(newHandler, listModifies);

            OnFeedbackRootEvent();
        }

        /// <summary>
        /// Rimuove il nodo dalla mappa, e aggiorna gli elementi degli eventuali handler ancora rimasti 
        /// nella mappa.
        /// </summary>
        private void removeNode()
        {
            List<Modifies> listModifies = new List<Modifies>();
            List<Modifies> listTemp = new List<Modifies>();

            // Si determina quali sono i nuovi attributi in conflitto
            foreach (var elem in this.mapHandler)
            {
                if (listTemp.Count() > 0)
                {
                    // Memorizza temporaneamente il vecchio contenuto della lista
                    List<Modifies> temp = listTemp.ToList();
                    // Unione con gli attributi della funione in oggetto.
                    listTemp = listTemp.Union(elem.Value).ToList();
                    // Intersezione con la vecchia lista
                    temp = temp.Intersect(elem.Value).ToList();
                    // Differenza tra le due liste
                    listModifies = temp.Except(listTemp).ToList();
                }
                else// Inizializzazione mappa
                {
                    listTemp.Union(elem.Value);
                }
            }

            // Prima si determina quali sono i nuovi attributi in conflitto
            //foreach (var i in this.mapHandler)
            //{
                //if (listModifies.Count > 0)
                    //listModifies.Union(i.Key.elementList);
                //else
                    //listModifies.Intersect(i.Key.elementList);
            //}

            // Quindi si fa l'union con la lista degli elementi di partenza e poi se ne fa la differenza
            foreach (var i in this.mapHandler)
            {
                foreach (Modifies element in i.Key.elementList.Except(listModifies).Except(i.Value).ToList())
                {
                    i.Value.Add(element);
                }
            }

            OnFeedbackRootEvent();
        }*/

        /// <summary>
        /// Funzione che resetta l'albero (e di conseguenza tutti i suoi figli) e la mappa degli handler
        /// </summary>
        private void reset()
        {
            foreach (FeedbackGesture feedbackGesture in this.children)
            {
                feedbackGesture.reset();// Resetta il figlio
            }
            this.conflictManager.mapConflictExec.Clear();// Resetta la mappa degli Handler
            //this.mapHandler.Clear();// Resetta la mappa degli Handler
            OnFeedbackRootEvent();
        }

        /// <summary>
        /// Funzione che richiama la reset
        /// </summary>
        private void resetTree()
        {
            this.reset();
        }

        public void OnFeedbackRootEvent()
        {
            if (this.feedbackRootEvent != null)
                this.feedbackRootEvent();
        }
    }

    public class ComparerHandler : IComparer<Handler>
    {
        public int Compare(Handler x, Handler y)
        {
            //return (y.likelihood.probability.CompareTo(x.likelihood.probability));
            return (y.likelihood.CompareTo(x.likelihood));
        }
    }
}
