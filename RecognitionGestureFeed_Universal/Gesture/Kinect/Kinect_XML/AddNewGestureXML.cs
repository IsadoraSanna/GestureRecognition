using System;
using System.Collections.Generic;
// Kinect 
using Microsoft.Kinect;
// Skeleton
// Xml
using System.Xml.Serialization;
using System.IO;
using Unica.Djestit.Recognition.Kinect2;

namespace Unica.Djestit.Kinect2.XML
{
    public class AddNewGestureXML
    {
        /* Attributi */

        /* Costruttore */
        public AddNewGestureXML(String nameGesture, List<Microsoft.Kinect.JointType> jointReg, Skeleton[] skeletonList, string path)
        {
            // Inizializzo la nuova GestureXML che verrà inserita nel database
            GestureXML newGesture = new GestureXML();
            newGesture.jointInformationList = new List<JointInformation>();
            newGesture.name = nameGesture;
            // Variabile che indicherà se la gesture è stata effettivamente registrata o meno
            bool boolYesWriting = false;
            // Elemento che verrà usato per accedere ai JointInformation di Skeleton
            JointInformation jointI;

            foreach (Skeleton skeleton in skeletonList)
            {
                if (skeleton.getStatus())
                {
                    foreach (Microsoft.Kinect.JointType jointType in jointReg)
                    {
                        // Prendo dallo scheletro il joint che mi serve
                        jointI = (JointInformation) skeleton.getJointInformation(jointType).Clone();
                        // Processo i dati

                        // Inserisco il Joint e i dati appena calcolati nella lista di newGesture
                        newGesture.jointInformationList.Add(jointI);
                        // La gesture è stata registrata
                        boolYesWriting = true;
                    }
                }
            }
            // Se una gesture è stata registrata, allora provvedo alla serializzazione
            if (boolYesWriting)
            {
                // Richiamo la funzione per serializzare il file
                SerializeToXML(newGesture, path);
            }
        }

        /* Metodi */
        private void SerializeToXML(GestureXML newGesture, string path)
        {
            // Serializer usato per leggere/scrivere dal file
            XmlSerializer serializer = new XmlSerializer(typeof(List<GestureXML>));
            /// Read
            List<GestureXML> listGesture = GestureDetectorXML.readXML(path, serializer);
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
