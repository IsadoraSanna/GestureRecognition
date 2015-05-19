using RecognitionGestureFeed_Universal.Djestit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class GestureMove : GroundTerm
    {
        private ulong ID { set; get; }

        public GestureMove(ulong id)
        {
            this.ID = id;
        }

        public bool accepts(GestureToken token)
        {
            if(token.GetType != TypeToken.Move)
                return false;
            if (this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            else return true;
        }
    }
}
