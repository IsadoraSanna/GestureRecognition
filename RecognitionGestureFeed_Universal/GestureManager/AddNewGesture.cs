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

            foreach(Skeleton skeleton in skeletonList)
            {
                if(skeleton.getStatus())
                {
                    foreach (JointType jointType in jointReg)
                    {
                        if (skeleton.getStatus())
                        {
                            // Prendo dallo scheletro il joint che mi serve
                            joint = skeleton.getJointInformation(jointType);
                            // Processo i dati

                            // Inserisco il Joint e i dati appena calcolati nella lista di newGesture
                            newGesture.jointInformationList.Add(joint);

                            scrittura = true;
                        }
                    }
                }
            }

            if (scrittura)
            {
                
                /*XmlTextWriter c = new XmlTextWriter("C:/Users/BatCave/Copy/Tesi/DatabaseGesture/databaseGesture_1.xml", null);
                // Apro il documento
                c.WriteStartDocumentAsync();
                //
                c.WriteStartElementAsync("Porcoddio");
                c.WriteWhitespaceAsync("\n");
                foreach(JointInformation jointI in newGesture.jointInformationList)
                {
                    c.WriteElementString("Joint: ", jointI.getName());
                    c.WriteWhitespace("\n");
                }

                c.WriteEndElement();
                c.WriteEndDocument();*/
            }
        }
    }
}
