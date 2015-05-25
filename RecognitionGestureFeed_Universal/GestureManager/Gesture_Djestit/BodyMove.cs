using RecognitionGestureFeed_Universal.Djestit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class BodyMove : GroundTerm
    {
        private ulong ID { set; get; }

        public BodyMove(ulong id)
        {
            this.ID = id;
        }

        public bool accepts(BodyToken token)
        {
            if(token.type != TypeToken.Move)
                return false;
            if (this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            else return true;
        }
    }
}
