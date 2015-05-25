using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// GroundTerm
using RecognitionGestureFeed_Universal.Djestit;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class BodyEnd : GroundTerm
    {
        // Attributi
        ulong ID;

        public BodyEnd(ulong id)
        {
            this.ID = id;
        }

        public bool accepts(BodyToken token)
        {
            if(token.type != TypeToken.End)
                return false;
            if(this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            return true;
        }
    }
}