using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using Unica.Djestit;
using System.Transactions;
using Unica.Djestit.Feed;
using Unica.Djestit.Concurrency;

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
            termA.Type = "Start";
            termA.Accepts = closeA;
            termA.Name = "GroundTerm CloseA";
            termA.likelihood = 0.5f;
            termA.Complete += completeA;
            //termA.handler = new Handler(completeA, termA);
            termB = new GroundTerm();
            termB.Type = "Start";
            termB.Accepts = closeB;
            termB.likelihood = 0.6f;
            termB.Name = "GroundTerm CloseB";
            termB.Complete += completeB;
            //termB.handler = new Handler(this.completeB, termB);
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
                //transactionManager.onTransactionExcute(sender.term.handler.elementList);
                scope.Complete();
            }
        }

        // Funzione eseguita al riconoscimento di B
        [Modifies("a", 0, 12), Modifies("c", 0, 1)]
        private void completeB(object obj, GestureEventArgs sender)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                //transactionManager.onTransactionExcute(sender.term.handler.elementList);
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