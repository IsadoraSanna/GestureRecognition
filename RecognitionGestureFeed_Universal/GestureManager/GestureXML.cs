using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;

namespace RecognitionGestureFeed_Universal.GestureManager
{
    class GestureXML
    {
        /*** Attributi ***/
        public List<JointInformation> jointInformationList { get; set; }
        public string name { get; set; } 

        /*** Costruttore ***/
        public GestureXML() { }
    }
}
