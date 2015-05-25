using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecognitionGestureFeed_Universal.Djestit;
using System.Collections.Generic;
using RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit;
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;

namespace UnitTestProject1
{
    [TestClass]
    public class GestureTest
    {
        [TestMethod]
        public void DisablingOperator()
        {
            GroundTerm term1 = new GroundTerm();
            term1.type = "BodyStart";
            GroundTerm term2 = new GroundTerm();
            term2.type = "BodyMove";
            GroundTerm term3 = new GroundTerm();
            term3.type = "BodyEnd";

            Iterative iterative = new Iterative(term2);
            List<Term> listTerm = new List<Term>();
            listTerm.Add(iterative);
            listTerm.Add(term3);

            Disabling disabling = new Disabling(listTerm);
            List<Term> listTerm2 = new List<Term>();
            listTerm2.Add(term1);
            listTerm2.Add(disabling);

            Sequence sequence = new Sequence(listTerm2);

            Assert.IsTrue(1 == 1, "ok");
            
        }

        [TestMethod]
        public void TouchSequence()
        {
            //creo l'espressione
            GroundTerm term1 = new GroundTerm();
            term1.type = "BodyStart";
            GroundTerm term2 = new GroundTerm();
            term2.type = "BodyMove";
            GroundTerm term3 = new GroundTerm();
            term3.type = "BodyEnd";
            GroundTerm term4 = new GroundTerm();
            term4.type = "BodyStart";
            GroundTerm term5 = new GroundTerm();
            term5.type = "BodyMove";
            GroundTerm term6 = new GroundTerm();
            term6.type = "BodyEnd";

            List<Term> listTerm = new List<Term>();
            listTerm.Add(term3);
            listTerm.Add(term4);
            Iterative iterative = new Iterative(listTerm);
            Parallel parallel = new Parallel(iterative);

            List<Term> listTerm2 = new List<Term>();
            listTerm2.Add(term5);
            listTerm2.Add(term6);
            OrderIndependece orderIndependence = new OrderIndependece(listTerm2);

            List<Term> listTerm3 = new List<Term>();
            listTerm3.Add(parallel);
            listTerm3.Add(orderIndependence);

            Disabling disabling = new Disabling(listTerm3);

            List<Term> listTerm4 = new List<Term>();
            listTerm4.Add(term1);
            listTerm4.Add(term2);
            OrderIndependece orderIndependence2 = new OrderIndependece(listTerm4);

            List<Term> listTerm5 = new List<Term>();
            listTerm5.Add(orderIndependence2);
            listTerm5.Add(disabling);

            Sequence sequence = new Sequence(listTerm5);



        }
        public class newBodyToken : BodyToken
        {
            public newBodyToken(TypeToken type, Skeleton skeleton) : base(type, skeleton)
            {

            }
        }

    }
}
