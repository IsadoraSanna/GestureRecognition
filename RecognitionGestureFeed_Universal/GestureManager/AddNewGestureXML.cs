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
    public class AddNewGestureXML
    {
        // Attributi
        string path = "C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml";
        public class GestureXML
        {
            public List<JointInformation> jointInformationList { get; set; }
            public GestureXML() { }
        }

        // Metodi
        public AddNewGestureXML(String nameGesture, List<JointType> jointReg, Skeleton[] skeletonList)
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
            // Se una gesture è stata registrata, allora provvedo alla serializzazione
            if (scrittura)
            {
                // Richiamo la funzione per serializzare il file
                SerializeToXML(newGesture);
            }
        }

        private void SerializeToXML(GestureXML newGesture)
        {
            // Serializer usato per leggere/scrivere dal file
            XmlSerializer serializer = new XmlSerializer(typeof(List<AddNewGestureXML.GestureXML>));
            /// Read
            List<AddNewGestureXML.GestureXML> listGesture = GestureDetectorXML.readXML(path, serializer);
            /// Write
            // Aggiungo nella lista la GestureXML
            listGesture.Add(newGesture);
            // Apro il file in modalità open
            StreamWriter writer = new StreamWriter(path);
            // Serializzo la lista di gesture sul file
            serializer.Serialize(writer, listGesture);
            // Chiudo il file
            writer.Close();
        }
    }
}
