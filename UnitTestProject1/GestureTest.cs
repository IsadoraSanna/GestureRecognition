using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecognitionGestureFeed_Universal.Djestit;
using System.Collections.Generic;
using RecognitionGestureFeed_Universal.Gesture.Kinect_Djestit;
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// Kinect
using Microsoft.Kinect;

namespace UnitTestProject1
{
    //[TestClass]
    //public class GestureTest
    //{
    //    [TestMethod]
    //    public void DisablingOperator()
    //    {
    //        GroundTerm term1 = new GroundTerm();
    //        term1.type = "BodyStart";
    //        GroundTerm term2 = new GroundTerm();
    //        term2.type = "BodyMove";
    //        GroundTerm term3 = new GroundTerm();
    //        term3.type = "BodyEnd";

    //        Iterative iterative = new Iterative(term2);
    //        List<Term> listTerm = new List<Term>();
    //        listTerm.Add(iterative);
    //        listTerm.Add(term3);

    //        Disabling disabling = new Disabling(listTerm);
    //        List<Term> listTerm2 = new List<Term>();
    //        listTerm2.Add(term1);
    //        listTerm2.Add(disabling);

    //        Sequence sequence = new Sequence(listTerm2);

    //        Assert.IsTrue(1 == 1, "ok");
            
    //    }

    //    [TestMethod]
    //    public void TouchSequencePan()
    //    {
    //        /************************ Gesture Pinch
    //        GroundTerm term1 = new GroundTerm();
    //        term1.type = "BodyStart";
    //        GroundTerm term2 = new GroundTerm();
    //        term2.type = "BodyMove";
    //        GroundTerm term3 = new GroundTerm();
    //        term3.type = "BodyEnd";
    //        GroundTerm term4 = new GroundTerm();
    //        term4.type = "BodyStart";
    //        GroundTerm term5 = new GroundTerm();
    //        term5.type = "BodyMove";
    //        GroundTerm term6 = new GroundTerm();
    //        term6.type = "BodyEnd";

    //        List<Term> listTerm = new List<Term>();
    //        listTerm.Add(term3);
    //        listTerm.Add(term4);
    //        Iterative iterative = new Iterative(listTerm);
    //        Parallel parallel = new Parallel(iterative);

    //        List<Term> listTerm2 = new List<Term>();
    //        listTerm2.Add(term5);
    //        listTerm2.Add(term6);
    //        OrderIndependece orderIndependence = new OrderIndependece(listTerm2);

    //        List<Term> listTerm3 = new List<Term>();
    //        listTerm3.Add(parallel);
    //        listTerm3.Add(orderIndependence);

    //        Disabling disabling = new Disabling(listTerm3);

    //        List<Term> listTerm4 = new List<Term>();
    //        listTerm4.Add(term1);
    //        listTerm4.Add(term2);
    //        OrderIndependece orderIndependence2 = new OrderIndependece(listTerm4);

    //        List<Term> listTerm5 = new List<Term>();
    //        listTerm5.Add(orderIndependence2);
    //        listTerm5.Add(disabling);

    //        Sequence sequence = new Sequence(listTerm5);
    //        */

    //        /* Gesture Pan */
    //        GroundTerm term1 = new GroundTerm();
    //        term1.type = "BodyStart";
    //        GroundTerm term2 = new GroundTerm();
    //        term2.type = "BodyMove";
    //        GroundTerm term3 = new GroundTerm();
    //        term3.type = "BodyEnd";
    //        Iterative iterative = new Iterative(term2);
    //        List<Term> listTerm = new List<Term>();
    //        listTerm.Add(iterative);
    //        listTerm.Add(term3);
    //        Disabling disabling = new Disabling(listTerm);
    //        List<Term> listTerm2 = new List<Term>();
    //        listTerm2.Add(term1);
    //        listTerm2.Add(disabling);
    //        Sequence sequence = new Sequence(listTerm2);

    //        /* Definizione dell'espressione */
    //        // JointSensor
    //        JointSensor jointSensor = new JointSensor(sequence, 3);
    //        // NewJointToken creati ad cazzum
    //        NewJointToken a1 = new NewJointToken(TypeToken.Start, JointType.HandRight, 50, 50, 50, 0);// Start
    //        NewJointToken a2 = new NewJointToken(TypeToken.Move, JointType.HandRight, 51, 51, 51, 0);// Move
    //        NewJointToken a3 = new NewJointToken(TypeToken.End, JointType.HandRight, 52, 52, 52, 0);// End
    //        // NewJointToken creati con il JointSensor
    //        NewJointToken ta1 = jointSensor.generateToken(TypeToken.Start, a1);
    //        NewJointToken sa1 = (NewJointToken)jointSensor.sequence.getById(0, 1);
    //        Assert.IsTrue(ta1 == sa1, "Passo 1: Tocco 1 delay 0");

    //        //2
    //        NewJointToken ta2 = jointSensor.generateToken(TypeToken.Move, a2);
    //        sa1 = (NewJointToken)jointSensor.sequence.getById(1, 1);
    //        NewJointToken sa2 = (NewJointToken)jointSensor.sequence.getById(0, 1);
    //        Assert.IsTrue(ta1 == sa1, "Passo 2: Tocco 1 delay 1");
    //        Assert.IsTrue(ta2 == sa2, "Passo 2: Tocco 1 delay 0");

    //        //2
    //        NewJointToken ta3 = jointSensor.generateToken(TypeToken.Move, a3);
    //        sa1 = (NewJointToken)jointSensor.sequence.getById(2, 1);
    //        sa2 = (NewJointToken)jointSensor.sequence.getById(1, 1);
    //        NewJointToken sa3 = (NewJointToken)jointSensor.sequence.getById(0, 1);
    //        Assert.IsTrue(ta1 == sa1, "Passo 3: Tocco 1 delay 2");
    //        Assert.IsTrue(ta2 == sa2, "Passo 3: Tocco 1 delay 1");
    //        Assert.IsTrue(ta3 == sa3, "Passo 3: Tocco 1 delay 0");

    //    }

    //    [TestMethod]
    //    public void TouchSequencePinch()
    //    {
    //        /* Gesture Pinch */
    //        GroundTerm term1 = new GroundTerm();
    //        term1.type = "BodyStart";
    //        GroundTerm term2 = new GroundTerm();
    //        term2.type = "BodyMove";
    //        GroundTerm term3 = new GroundTerm();
    //        term3.type = "BodyEnd";
    //        GroundTerm term4 = new GroundTerm();
    //        term4.type = "BodyStart";
    //        GroundTerm term5 = new GroundTerm();
    //        term5.type = "BodyMove";
    //        GroundTerm term6 = new GroundTerm();
    //        term6.type = "BodyEnd";
    //        List<Term> listTerm = new List<Term>();
    //        listTerm.Add(term3);
    //        listTerm.Add(term4);
    //        Iterative iterative = new Iterative(listTerm);
    //        Parallel parallel = new Parallel(iterative);
    //        List<Term> listTerm2 = new List<Term>();
    //        listTerm2.Add(term5);
    //        listTerm2.Add(term6);
    //        OrderIndependece orderIndependence = new OrderIndependece(listTerm2);
    //        List<Term> listTerm3 = new List<Term>();
    //        listTerm3.Add(parallel);
    //        listTerm3.Add(orderIndependence);
    //        Disabling disabling = new Disabling(listTerm3);
    //        List<Term> listTerm4 = new List<Term>();
    //        listTerm4.Add(term1);
    //        listTerm4.Add(term2);
    //        OrderIndependece orderIndependence2 = new OrderIndependece(listTerm4);
    //        List<Term> listTerm5 = new List<Term>();
    //        listTerm5.Add(orderIndependence2);
    //        listTerm5.Add(disabling);
    //        Sequence pinch = new Sequence(listTerm5);



    //        /* 1 */
    //        JointSensor tsensor = new JointSensor(pinch, 3);
    //        NewJointToken r1 = new NewJointToken(TypeToken.Start, JointType.HandRight, 100, 100, 100, 0);
    //        NewJointToken l1 = new NewJointToken(TypeToken.Start, JointType.HandLeft, 200, 200, 200, 1);
    //        NewJointToken r2 = new NewJointToken(TypeToken.Start, JointType.HandRight, 101, 101, 101, 0);
    //        NewJointToken l2 = new NewJointToken(TypeToken.Start, JointType.HandLeft, 201, 201, 201, 1);
    //        NewJointToken r3 = new NewJointToken(TypeToken.Start, JointType.HandRight, 102, 102, 102, 0);
    //        NewJointToken tr1 = tsensor.generateToken(TypeToken.Start, r1);
    //        NewJointToken sr1 = (NewJointToken)tsensor.sequence.getById(0, 1);
    //        Assert.IsTrue(tr1 == sr1, "Passo 1: Tocco 1 delay 0");

    //        /* 2 */
    //        NewJointToken tl1 = tsensor.generateToken(TypeToken.Start, l1);
    //        sr1 = (NewJointToken)tsensor.sequence.getById(0,1);
    //        NewJointToken sl1 = (NewJointToken)tsensor.sequence.getById(0,2);
    //        Assert.IsTrue(tr1 == sr1, "Passo 2: Tocco 1 delay 0");
    //        Assert.IsTrue(tl1 == sl1, "Passo 2: Tocco 2 delay 0");

    //        /* 3 */
    //        NewJointToken tr2 = tsensor.generateToken(TypeToken.Move, r2);
    //        NewJointToken sr2 = (NewJointToken)tsensor.sequence.getById(0, 1);
    //        sr1 = (NewJointToken)tsensor.sequence.getById(1, 1);
    //        sl1 = (NewJointToken)tsensor.sequence.getById(0, 2);
    //        Assert.IsTrue(tr2 == sr2, "Passo 3: Tocco 1 delay 0");
    //        Assert.IsTrue(tr1 == sr1, "Passo 3: Tocco 1 delay 1");
    //        Assert.IsTrue(tl1 == sl1, "Passo 3: Tocco 2 delay 0");

    //        /* 4 */
    //        NewJointToken tl2 = tsensor.generateToken(TypeToken.Move, l2);
    //        sr2 = (NewJointToken)tsensor.sequence.getById(0, 1);
    //        NewJointToken sl2 = (NewJointToken)tsensor.sequence.getById(0, 2);
    //        sr1 = (NewJointToken)tsensor.sequence.getById(1, 1);
    //        sl1 = (NewJointToken)tsensor.sequence.getById(1, 2);
    //        Assert.IsTrue(tr2 == sr2, "Passo 4: Tocco 1 delay 0");
    //        Assert.IsTrue(tl2 == sl2, "Passo 4: Tocco 2 delay 0");
    //        Assert.IsTrue(tr1 == sr1, "Passo 4: Tocco 1 delay 1");
    //        Assert.IsTrue(tl1 == sl1, "Passo 4: Tocco 2 delay 1");

    //        /* 5 */
    //        NewJointToken tr3 = tsensor.generateToken(TypeToken.Move, r3);
    //        NewJointToken sr3 = (NewJointToken)tsensor.sequence.getById(0, 1);
    //        sr2 = (NewJointToken)tsensor.sequence.getById(1, 1);
    //        sl2 = (NewJointToken)tsensor.sequence.getById(0, 2);
    //        sl1 = (NewJointToken)tsensor.sequence.getById(1, 2);
    //        Assert.IsTrue(tr3 == sr3, "Passo 5: Tocco 1 delay 0");
    //        Assert.IsTrue(tr2 == sr2, "Passo 5: Tocco 2 delay 0");
    //        Assert.IsTrue(tl2 == sl2, "Passo 5: Tocco 1 delay 1");
    //        Assert.IsTrue(tl1 == sl1, "Passo 5: Tocco 2 delay 1");
    //    }
    //}
}
