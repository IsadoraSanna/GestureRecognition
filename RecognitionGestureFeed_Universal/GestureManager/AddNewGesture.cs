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
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.GestureManager
{

    public class AddNewGesture
    {
        public class GestureXML
        {

            [XmlArray("JointInformationList"), XmlArrayItem(typeof(JointInformation), ElementName = "Joint")]
            public List<JointInformation> jointInformationList { get; set; }

        }

        public AddNewGesture(String nameGesture, List<JointType> jointReg, Skeleton[] skeletonList)
        {
            // La nuova GestureXML che verrà inserita nel database
            GestureXML newGesture = new GestureXML();
            newGesture.jointInformationList = new List<JointInformation>();
            bool scrittura = false;
            // Variabile che verrà usata per accedere alle joint da registrare
            JointInformation joint;


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
                        //newGesture.jointInformationList.Add(joint);
                        newGesture.jointInformationList.Add(joint);
                        
                        scrittura = true;
                    }
                }
            }
            if(scrittura)
                SerializeToXML(newGesture);
            //

        }
       
        private void SerializeToXML(GestureXML newGesture)
        {
            Debug.WriteLine("tipo joint" + newGesture.jointInformationList[0].getPosition().X);

            XmlSerializer writer = new XmlSerializer(typeof(GestureXML));
            FileStream file = new FileStream("C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml", FileMode.Append);
            // Scrittura
            writer.Serialize(file, newGesture);
            file.Close();
        }
    }
}
