using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
// Thread
using System.Threading;
using System.ComponentModel;
// Reflection
using System.Reflection;
// Debug
using System.Diagnostics;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
// Handler
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;
// Conflict
using RecognitionGestureFeed_Universal.Feed.FeedBack.Conflict;
// Gestione Transazioni
using RecognitionGestureFeed_Universal.Concurrency;
// database
using System.Transactions;

namespace UnitTestProject1
{
    [TestClass]
    [Modifies("a", 0), Modifies("b", 0), Modifies("c", 0)]
    public class Test_Transaction
    {
        /* Attributi */
        // Gesture
        GroundTerm termA, termB;
        TransactionsManager transactionManager = new TransactionsManager();
        ConflictManager conflictManager = null;
        
        [TestMethod]
        public void Test_Transaction_1()
        {
            // Creo due gesture
            termA = new GroundTerm();
            termA.type = "Start";
            termA.accepts = closeA;
            termA.name = "GroundTerm CloseA";
            termA.likelihood = 0.5f;
            termA.Complete += completeA;
            termA.handler = new Handler(completeA, termA, this.GetType().GetCustomAttributes(true).OfType<Modifies>().ToList());
            termB = new GroundTerm();
            termB.type = "Start";
            termB.accepts = closeB;
            termB.likelihood = 0.6f;
            termB.name = "GroundTerm CloseB";
            termB.Complete += completeB;
            termB.handler = new Handler(this.completeB, termB, this.GetType().GetCustomAttributes(true).OfType<Modifies>().ToList());
            // Choice
            List<Term> list = new List<Term>();
            list.Add(termA);
            list.Add(termB);
            Choice choice = new Choice(list);

            conflictManager = new ConflictManager(this, choice);

            termA.complete(new Token());
            termB.complete(new Token());

            foreach(Modifies elem in this.conflictManager.listModifies)
            {
                Debug.WriteLine(elem.name + " - " + elem.value + " - " + elem.newValue);
            }
        }

        // Funzione eseguita al riconoscimento di A
        [Modifies("a", 0, 10), Modifies("b", 0, 1)]
        private void completeA(object obj, GestureEventArgs sender)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                // Cerca la corrispondenza con un modifies del programma
                transactionManager.onTransactionExcute(conflictManager.listModifies, sender.term.handler.elementList);
                scope.Complete();
            }
        }

        // Funzione eseguita al riconoscimento di B
        [Modifies("a", 0, 12), Modifies("c", 0, 1)]
        private void completeB(object obj, GestureEventArgs sender)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                transactionManager.onTransactionExcute(conflictManager.listModifies, sender.term.handler.elementList);
                scope.Complete();
            }
        }

        /* Funzioni Accettazione Metodi */
        private bool closeA(Token token)
        {
            return true;
        }
        private bool closeB(Token token)
        {
            return true;
        }
    }
}