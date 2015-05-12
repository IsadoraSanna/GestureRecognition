using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Recognition.BodyStructure
{
    /// <summary>
    /// La classe bone rappresenta un osso della classe scheletro
    /// </summary>
    public class Bone : Tuple<JointType, JointType>
    {
        /* Attributi */
        // Le joint dell'osso (i due estremi)
        private JointType joint1;
        private JointType joint2;
        // Dimensioni dell'osso
        //private double size;

        // Costruttore
        public Bone(JointType jointType1, JointType jointType2) : base(jointType1, jointType2) 
        {
            // Assegno i due valori presi in input
            this.joint1 = jointType1;
            this.joint2 = jointType2;
        }
    }
}