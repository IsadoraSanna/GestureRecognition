using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect 
using Microsoft.Kinect;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
// Xml
using System.Xml;
using System.Xml.XmlConfiguration;

namespace RecognitionGestureFeed_Universal.GestureManager
{
    public class AddNewGesture
    {
        public class GestureXML
        {
            public List<JointInformation> jointInformationList = new List<JointInformation>();
        }

        public AddNewGesture(String nameGesture, List<JointType> jointReg, Skeleton[] skeletonList)
        {
            // La nuova GestureXML che verrà inserita nel database
            GestureXML newGesture = new GestureXML();
            bool scrittura = false;
            // Variabile che verrà usata per accedere alle joint da registrare
            JointInformation joint;
            //
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(GestureXML));
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:/Users/Alessandro/Copy/Tesi/DatabaseGesture_1.xml");

            foreach(Skeleton skeleton in skeletonList)
            {
                if(skeleton.getStatus())
                {
                    foreach (JointType jointType in jointReg)
                    {
                        // Prendo dallo scheletro il joint che mi serve
                        joint = skeleton.getJointInformation(jointType);
                        // Processo i dati

                        // Inserisco il Joint e i dati appena calcolati nella lista di newGesture
                        newGesture.jointInformationList.Add(joint);

                        // Scrittura
                        writer.Serialize(file, newGesture);
                    }
                }
            }
            file.Close();
        }
    }
}
