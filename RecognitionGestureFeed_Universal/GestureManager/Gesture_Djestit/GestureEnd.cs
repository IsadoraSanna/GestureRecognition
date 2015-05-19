using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// GroundTerm
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class GestureEnd : GroundTerm
    {
        // Attributi
        ulong ID;

        public GestureEnd(ulong id)
        {
            this.ID = id;
        }

        public bool accepts(GestureToken token)
        {
            if(token.type != TypeToken.End)
                return false;
            if(this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            return true;
        }
    }
}