using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// XML
using System.Xml.Serialization;

namespace RecognitionGestureFeed_Universal.GestureManager
{
    

    public class GestureDetectorXML
    {
        /**** Attributi ****/
        //
        private List<AddNewGestureXML.GestureXML> gestureList = new List<AddNewGestureXML.GestureXML>();
        string path = "C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml";

        //costruttore
        public GestureDetectorXML(){
            /// Read
            XmlSerializer serializer = new XmlSerializer(typeof(List<AddNewGestureXML.GestureXML>));
            StreamReader reader = new StreamReader(path);
            if (reader.Peek() > -1)
                gestureList = (List<AddNewGestureXML.GestureXML>)serializer.Deserialize(reader);
            reader.Close();
        }

        //metodi
        public void printXML()
        {
            foreach (AddNewGestureXML.GestureXML gesture in this.gestureList)
            {
                Debug.WriteLine(gesture.jointInformationList[0].getType());
            }
        }

        public static List<AddNewGestureXML.GestureXML> readXML(string path, XmlSerializer serializer)
        {
            /// Read
            // Creo la lista di GestureXML che userò per leggere dal file
            List<AddNewGestureXML.GestureXML> listGesture = new List<AddNewGestureXML.GestureXML>();
            // Apro il database
            StreamReader reader = new StreamReader(path);
            // Se il file non è vuoto allora deserializzo gli elementi presenti e li metto nella lista
            if (reader.Peek() > -1)
                listGesture = (List<AddNewGestureXML.GestureXML>)serializer.Deserialize(reader);
            // Chiudo il database
            reader.Close();
            // Restituisco la lista
            return listGesture;
        }
    }
}
