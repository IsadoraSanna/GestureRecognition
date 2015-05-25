using RecognitionGestureFeed_Universal.Djestit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class JointMove : GroundTerm
    {
        private ulong ID { set; get; }

        public JointMove(ulong id)
        {
            this.ID = id;
        }

        public bool accepts(JointToken token)
        {
            if(token.type != TypeToken.Move)
                return false;
            if (this.ID != null && this.ID != token.jointInformation.getId())
                return false;
            else return true;
        }
    }
}
