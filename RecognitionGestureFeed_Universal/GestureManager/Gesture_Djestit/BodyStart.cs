using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class BodyStart : GroundTerm
    {
        /* Attributi */
        private ulong ID { get; set; }

        /* Metodi */
        public bool accepts(BodyToken token)
        {
            if(token.type != TypeToken.Start)
                return false;
            if(this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            return true;
        }
    }
}
