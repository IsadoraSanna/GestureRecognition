using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    public class NewJointToken : Token
    {
        public TypeToken type;
        public JointType jointType;
        public float x;
        public float y;
        public float z;
        public int identifier;

        public NewJointToken(JointType jointType, float x, float y, float z, int ID)
        {
            this.jointType = jointType;
            this.x = x;
            this.y = y;
            this.z = z;
            this.identifier = ID;
        }
    }
}
