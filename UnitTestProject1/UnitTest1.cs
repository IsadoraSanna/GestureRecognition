using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecognitionGestureFeed_Universal.Djestit;
using System.Collections.Generic;

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

            sequence.onComplete += 
            /////////sequence1.onComplete.add(function() {console.log(sequence1.state);});
            //sequence1.

            sequence.fire(new Token());
            sequence.fire(new Token());

            Assert.is
        }


    sequence1.onComplete.add(function() {console.log(sequence1.state);});


    assert.ok(djestit.COMPLETE === sequence1.state, "Sequence completed");

    sequence1.fire(new djestit.Token());
    assert.ok(djestit.ERROR === sequence1.state, "No more token accepted");
});
    }
}
