using System;
// Test
using Microsoft.VisualStudio.TestTools.UnitTesting;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
using System.Collections.Generic;
// Debug
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Composition()
        {
            GroundTerm term1 = new GroundTerm();
            GroundTerm term2 = new GroundTerm();
            GroundTerm term3 = new GroundTerm();
            GroundTerm term4 = new GroundTerm();

            List<Term> lista1 = new List<Term>();
            lista1.Add(term1);
            lista1.Add(term2);
            List<Term> lista2 = new List<Term>();
            lista2.Add(term3);
            lista2.Add(term4);
            Sequence sequence = new Sequence(lista1);
            Parallel parallel = new Parallel(lista2);

            Assert.IsTrue( sequence.children != parallel.children , "Passed");
        }

        [TestMethod]
        public void IterativeOperator()
        {

            GroundTerm term1 = new GroundTerm();
            Iterative iterative = new Iterative(term1);

            Token token1 = new Token();
            iterative.fire(token1);
            Assert.IsTrue(expressionState.Complete == iterative.state, "First iteration completed");

            Token token2 = new Token();
            iterative.fire(token2);
            Assert.IsTrue(expressionState.Complete == iterative.state, "Second iteration completed");
        }

        [TestMethod]
        public void SequenceOperator()
        {
            GroundTerm term1 = new GroundTerm();
            GroundTerm term2 = new GroundTerm();

            List<Term> lista1 = new List<Term>();
            lista1.Add(term1);
            lista1.Add(term2);

            Sequence sequence = new Sequence(lista1);

            sequence.Complete += onComplete1;

            sequence.fire(new Token());
            sequence.fire(new Token());
            
            Assert.IsTrue(expressionState.Complete == sequence.state, "Sequence completed");

            
            sequence.fire(new Token());
            Assert.IsTrue(expressionState.Error == sequence.state, "No more token accepted");
        }

        [TestMethod]
        public void IterativeOperator2()
        {
            GroundTerm term1 = new GroundTerm();
            Iterative iterative = new Iterative(term1);

            iterative.Complete += onComplete2;

            iterative.fire(new Token());
            iterative.fire(new Token());

        }

        [TestMethod]
        public void ParallelOperator()
        {

            GroundTerm term1 = new GroundTerm();
            GroundTerm term2 = new GroundTerm();
            List<Term> lista = new List<Term>();
            lista.Add(term1);
            lista.Add(term2);

            Parallel parallel = new Parallel(lista);

            //parallel.Complete += onComplete2;
            term1.onComplete(term1);

            //parallel.Complete += onComplete2;
            term2.onComplete(term2);

            parallel.fire(new Token());
            Assert.IsTrue(expressionState.Complete == parallel.state, "Passed!");

        }

        [TestMethod]
        public void ParallelOperator1()
        {
            GroundTerm term1 = new GroundTerm();
            GroundTerm term2 = new GroundTerm();
            Iterative iterative1 = new Iterative(term1);
            Iterative iterative2 = new Iterative(term1);
            List<Term> listTerm = new List<Term>();
            listTerm.Add(iterative1);
            listTerm.Add(iterative2);

            term1.Complete += onComplete2;
            term2.Complete += onComplete2;

            Parallel parallel = new Parallel(listTerm);
            Token tokenA = new Token();
            tokenA.type = "A";
            Token tokenB = new Token();
            tokenB.type = "B";

            parallel.fire(tokenA);
            parallel.fire(tokenA);
            parallel.fire(tokenB);

            Assert.IsTrue(expressionState.Complete == parallel.state, "Passed!");
           
        }

        [TestMethod]
        public void ChoiceOperator()
        {
           GroundTerm term1 = new GroundTerm();
           GroundTerm term2 = new GroundTerm();
           GroundTerm term3 = new GroundTerm();
           GroundTerm term4 = new GroundTerm();

           List<Term> listTerm1 = new List<Term>();
           List<Term> listTerm2 = new List<Term>();
           listTerm1.Add(term1);
           listTerm1.Add(term2);
           listTerm2.Add(term3);
           listTerm2.Add(term4);

           Sequence sequence = new Sequence(listTerm1);
           Parallel parallel = new Parallel(listTerm2);

           List<Term> listTerm3 = new List<Term>();
           listTerm3.Add(sequence);
           listTerm3.Add(parallel);
           Choice choice = new Choice(listTerm3);
           Token tokenA = new Token();
           tokenA.type = "A";
           Token tokenB = new Token();
           tokenB.type = "B";

           choice.fire(tokenA);
           choice.fire(tokenA);

           Assert.IsTrue(sequence.state == expressionState.Complete, "First operand (sequence) completed");
           Assert.IsTrue(choice.state == expressionState.Complete, "Choice completed");

           choice.reset();
           choice.fire(tokenB);
           Assert.IsTrue(parallel.state == expressionState.Complete, "Second operand (parallel) completed");
           Assert.IsTrue(choice.state == expressionState.Complete, "Choice completed");
        }


        [TestMethod]
        public void OrderIndependenceOperator()
        {
            GroundTerm term1 = new GroundTerm();
            GroundTerm term2 = new GroundTerm();
            GroundTerm term3 = new GroundTerm();
            GroundTerm term4 = new GroundTerm();

            List<Term> listTerm1 = new List<Term>();
            List<Term> listTerm2 = new List<Term>();
            listTerm1.Add(term1);
            listTerm1.Add(term2);
            listTerm2.Add(term3);
            listTerm2.Add(term4);

            Sequence sequence = new Sequence(listTerm1);
            Parallel parallel = new Parallel(listTerm2);


            
            List<Term> listTerm3 = new List<Term>();
            listTerm3.Add(sequence);
            listTerm3.Add(parallel);
            OrderIndependece order = new OrderIndependece(listTerm3);

            Token tokenA = new Token();
            tokenA.type = "A";
            Token tokenB = new Token();
            tokenB.type = "B";

            order.fire(tokenA);
            order.fire(tokenA);

            Assert.IsTrue(sequence.state == expressionState.Complete, "First operand (sequence) completed");

            order.fire(tokenB);
            order.fire(tokenA);

            Assert.IsTrue(parallel.state == expressionState.Complete, "Second operand (parallel) completed");
            Assert.IsTrue(order.state == expressionState.Complete, "OrderIndependence completed");

            order.reset();

            order.fire(tokenB);
            order.fire(tokenA);

            Assert.IsTrue(parallel.state == expressionState.Complete, "Second operand (parallel) completed");

            order.fire(tokenA);
            order.fire(tokenA);

            Assert.IsTrue(sequence.state == expressionState.Complete, "First operand (sequence) completed");
            Assert.IsTrue(order.state == expressionState.Complete, "OrderIndependence completed");
        }

        [TestMethod]
        public void DisablingOperator()
        {
            GroundTerm term1 = new GroundTerm();
            Iterative iterative1 = new Iterative(term1);
            GroundTerm term2 = new GroundTerm();
            Iterative iterative2 = new Iterative(term2);
            GroundTerm term3 = new GroundTerm();

            List<Term> listTerm = new List<Term>();
            listTerm.Add(iterative1);
            listTerm.Add(iterative2);
            listTerm.Add(term3);
            Disabling disabling = new Disabling(listTerm);

            Token tokenA = new Token();
            tokenA.type = "A";
            Token tokenB = new Token();
            tokenB.type = "B";
            Token tokenC = new Token();
            tokenB.type = "C";

            // a sequence of A tokens
            disabling.fire(tokenA);
            disabling.fire(tokenA);

            // send a C token
            disabling.fire(tokenC);


            Assert.IsTrue(disabling.state == expressionState.Complete, "C token accepted");

            disabling.reset();

            disabling.fire(tokenA);
            disabling.fire(tokenB);
            disabling.fire(tokenA);

            Assert.IsTrue(disabling.state == expressionState.Error, "A token after B not accepted");

            disabling.reset();

            // a sequence of A tokens
            disabling.fire(tokenA);
            disabling.fire(tokenA);
            disabling.fire(tokenA);
            disabling.fire(tokenA);

            // stop with a B token
            disabling.fire(tokenB);
            Assert.IsTrue(iterative2.state == expressionState.Complete, "B tokens accepted");

            disabling.fire(tokenC);
            Assert.IsTrue(iterative2.state == expressionState.Complete, "C tokens accepted");
        }

        //metodi
        public bool accepts(Token token)
        {
            return token.type != null && token.type == "A";
        }
        public bool _accepts(Token token)
        {
            return token.type != null && token.type == "B";
        }

        // Funzioni per OnComplete
        private void onComplete1(Term e)
        {
            Debug.WriteLine(e.state);
        }

        private void onComplete2(Term term)
        {
            Assert.IsTrue(expressionState.Complete == term.state, "Sequence completed");
        }
    }
}
