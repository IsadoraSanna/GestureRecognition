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

namespace UnitTestProject1
{
    [TestClass]
    [Modifies("a", 0), Modifies("b", 0, 11)]
    public class Test_Transaction : RecognitionGestureFeed_Universal.Concurrency.TransactionsManager
    {
        /* Attributi */
        // Gesture
        GroundTerm termA, termB;

        [TestMethod]
        public void Test_Transaction_1()
        {
            this.listModifies = ((Modifies[]) Attribute.GetCustomAttributes(typeof(Test_Transaction), typeof(Modifies))).ToList();

            // Creo due gesture
            termA = new GroundTerm();
            termA.type = "Start";
            termA.accepts = closeA;
            termA.name = "GroundTerm CloseX";
            termA.likelihood = 0.5f;
            termA.Complete += ccc;
            termA.handler = new Handler(ccc);
            termB = new GroundTerm();
            termB.type = "Start";
            termB.accepts = closeB;
            termB.name = "GroundTerm CloseX";
            termB.Complete += completeB;
            termB.handler = new Handler(this.completeB);

            termA.complete(new Token());
        }

        //
        [Modifies("a", 0, 10)]
        private void ccc(object obj, GestureEventArgs sender)
        {
            BackgroundWorker worker1 = new BackgroundWorker();
            BackgroundWorker worker2 = new BackgroundWorker();

            worker2.DoWork += exec2;
            worker1.DoWork += exec1;
            

            worker2.RunWorkerAsync(sender);
            worker1.RunWorkerAsync(sender);
        }

        private void exec1(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GestureEventArgs args = (GestureEventArgs)e.Argument;

            Debug.WriteLine("Eseguito A");
            this.OnTransactionExcute(args.term.handler, new List<Modifies>(), args.term.handler.elementList);   
        }
        private void exec2(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GestureEventArgs args = (GestureEventArgs)e.Argument;

            // creo lista di modifies fasulla
            List<Modifies> list = new List<Modifies>();
            list.Add(new Modifies("a", 0, 12));
            list.Add(new Modifies("b", 0, 11));

            Debug.WriteLine("Eseguito B");
            this.OnTransactionExcute(args.term.handler, new List<Modifies>(), list);
        }

        // Funzione eseguita al riconoscimento di A
        [Modifies("a", 0, 10), Modifies("b", 1)]
        private void completeA(object obj, GestureEventArgs sender)
        {
            Debug.WriteLine("Eseguito A");
            this.OnTransactionExcute(sender.term.handler, new List<Modifies>(), sender.term.handler.elementList);
        }
        // Funzione eseguita al riconoscimento di B
        [Modifies("a", 0, 12), Modifies("c", 1)]
        private void completeB(object obj, GestureEventArgs sender)
        {
            Debug.WriteLine("Eseguito B");
            this.OnTransactionExcute(sender.term.handler, new List<Modifies>(), sender.term.handler.elementList);
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
