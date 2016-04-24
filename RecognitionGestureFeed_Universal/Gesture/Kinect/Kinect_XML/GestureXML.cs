using System.Collections.Generic;
// JointInformation
using Unica.Djestit.Recognition.Kinect2;

namespace Unica.Djestit.Kinect2.XML
{
    public class GestureXML
    {
        /*** Attributi ***/
        public List<JointInformation> jointInformationList { get; set; }
        public string name { get; set; } 

        /*** Costruttore ***/
        public GestureXML() { }
    }
}
