using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Djestit Kinect
using RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// Kinect
using Microsoft.Kinect;
// Debug
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class Djestit_Skeleton
    {

        void PanX(object sender, GestureEventArgs t)
        {
            Assert.IsTrue(true, "PorcamadonnaX");
        }
        void PanY(object sender, GestureEventArgs t)
        {
            Assert.IsTrue(true, "PorcoddioY");
        }
    }
}
