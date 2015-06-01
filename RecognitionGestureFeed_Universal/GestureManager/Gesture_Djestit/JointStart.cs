using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class JointStart : GroundTerm
    {
        /* Attributi */
        private ulong ID { get; set; }

        /*Costruttore
        public JointStart()
        {

        } DA FARE */

        /* Metodi */
        public bool accepts(JointToken token)
        {
            if(token.type != TypeToken.Start)
                return false;
            if(this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            return true;
        }
    }
}
