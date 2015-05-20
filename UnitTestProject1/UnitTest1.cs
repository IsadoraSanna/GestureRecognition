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
            
            Iterative iterative = new Iterative();

            
            /*
            iterative.fire(new djestit.Token());
            assert.ok(djestit.COMPLETE === iterative.state, "First iteration completed");

            // execute the term more than once
            iterative.fire(new djestit.Token());
            assert.ok(djestit.COMPLETE === iterative.state, "Second iteration completed");*/
        }
    }
}
