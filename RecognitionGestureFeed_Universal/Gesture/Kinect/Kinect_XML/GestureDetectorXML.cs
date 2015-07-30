using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// XML
using System.Xml.Serialization;
// Skeleton
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect_XML
{
    public class GestureDetectorXML
    {
        /**** Attributi ****/
        // Valore di confidenza entro cui deve ricadere il jointInformation
        //double confidenceMin = 0.7;        
        // Lista di gesture lette dal database
        private List<GestureXML> gestureList = new List<GestureXML>();
        //string path2 = "C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml";

        /**** Costruttore ****/
        public GestureDetectorXML(string path){
            /// Read
            XmlSerializer serializer = new XmlSerializer(typeof(List<GestureXML>));
            // Leggo dal database tutte le gesture disponibili
            this.gestureList = readXML(path, serializer);
        }

        /**** Metodi ****/
        public void GestureDetectorHandle(Skeleton skeleton)
        {
            // Controllo se la posizione dello scheletro combacia con quella descritta nella gesture
            if(this.gestureList.Count > 0)// Se nella lista è presente almeno una gesture
            {
                foreach(GestureXML gx in this.gestureList)// Per ogni GestureXML presente nella lista
                {
                    // Calcolo la vicinanza tra la gesture in questione e lo scheletro
                    calculateConfidence(gx, skeleton);
                    /// come si può comunicare all'utente che la gesture è stata rilevata?
                }
            }
        }

        // Cosa potrebbe restituire? Un bool? Una lista di double? Una classe?
        /// <summary>
        /// Presa una gesture e uno scheletro, calcola per ogni joint la confidenza con la posizione rilevata dalla kinect
        /// </summary>
        /// <param name="gx"></param>
        /// <param name="skeleton"></param>
        private void calculateConfidence(GestureXML gx, Skeleton skeleton)
        {
            // Valore di confidenza, una per ogni joint che costituiscono la gesture in esame
            List<double> confidenceList = new List<double>();

            // Verifico se le jointInformation presentano le stesse condizioni delle jointInformation che formano la gesture
            foreach(JointInformation jiGesture in gx.jointInformationList)
            {
                // Prelevo dallo scheletro lo stesso jointinformation di quello in esame
                JointInformation jiSkeleton = skeleton.getJointInformation(jiGesture.getType());

                // Calcolo la confidenza tra jiSkeleton e jiGesture
                //double confidence = blablabla
                // Inserisco il valore appena calcolato nella lista
                //confidenceList.Add(confidence);
            }

            // Calcolo la confidenza media
            double averageConfidence = confidenceList.Average();
            // Verifico se la confidenza globale ricade nell'intervallo richiesto
            //if(averageConfidence > this.confidenceMin)            
        }

        public void printXML()
        {
            foreach (GestureXML gesture in this.gestureList)
            {
                Debug.WriteLine(gesture.jointInformationList[0].getType());
            }
        }

        /// <summary>
        /// Leggo dal database tutte le gestureXML in esso presenti.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static List<GestureXML> readXML(string path, XmlSerializer serializer)
        {
            /// Gestione file non esistente
            /// Read
            // Creo la lista di GestureXML che userò per leggere dal file
            List<GestureXML> listGesture = new List<GestureXML>();
            // Apro il database
            StreamReader reader = new StreamReader(path);
            // Se il file non è vuoto allora deserializzo gli elementi presenti e li metto nella lista
            if (reader.Peek() > -1)
                listGesture = (List<GestureXML>)serializer.Deserialize(reader);
            // Chiudo il database
            reader.Close();
            // Restituisco la lista
            return listGesture;
        }
    }
}
