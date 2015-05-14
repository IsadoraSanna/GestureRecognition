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
            public List<JointInformation> jointInformationList { get; set; }
            public GestureXML() { }
        }

        public AddNewGesture(String nameGesture, List<JointType> jointReg, Skeleton[] skeletonList)
        {
            // La nuova GestureXML che verrà inserita nel database
            GestureXML newGesture = new GestureXML();
            newGesture.jointInformationList = new List<JointInformation>();
            bool scrittura = false;
            /// Elemento che verrà usato per accedere ai JointInformation di Skeleton
            JointInformation jointI;

            foreach (Skeleton skeleton in skeletonList)
            {
                if (skeleton.getStatus())
                {
                    foreach (JointType jointType in jointReg)
                    {
                        // Prendo dallo scheletro il joint che mi serve
                        jointI = (JointInformation) skeleton.getJointInformation(jointType).Clone();
                        // Processo i dati

                        // Inserisco il Joint e i dati appena calcolati nella lista di newGesture
                        newGesture.jointInformationList.Add(jointI);
                        // La gesture è stata registrata
                        scrittura = true;
                    }
                }
            }
            if (scrittura)
                SerializeToXML(newGesture);
        }

        private void SerializeToXML(GestureXML newGesture)
        {
            // Creo il serializer per poter scrivere sul file xml i dati della gesture appena registrata
            XmlSerializer writer = new XmlSerializer(typeof(GestureXML));
            // Apro il database in input in modalità append
            FileStream file = new FileStream("C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml", FileMode.Append);
            // Scrittura
            writer.Serialize(file, newGesture);
            // Chiudo il flusso del file
            file.Close();
        }
    }
}
