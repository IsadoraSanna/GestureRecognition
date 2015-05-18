using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.GestureManager
{
    

    public class GestureDetectorXML
    {
        //attributi
        public AddNewGestureXML.GestureXML gesture = new AddNewGestureXML.GestureXML();

        //costruttore
        public GestureDetectorXML(){
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(AddNewGestureXML.GestureXML));
            //System.IO.StreamReader file = new System.IO.StreamReader("C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml");
            FileStream file = new FileStream("C:/Users/BatCave/Copy/Tesi/DatabaseGesture/DatabaseGesture_1.xml", FileMode.Open);

            if(file != null)
                gesture = (AddNewGestureXML.GestureXML)reader.Deserialize(file);
        }

        //metodi
        public void printXML(AddNewGestureXML.GestureXML file)
        {
            Debug.WriteLine(file.jointInformationList[0].getType());

        }
        
    }
}
