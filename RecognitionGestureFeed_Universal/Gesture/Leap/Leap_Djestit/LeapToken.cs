using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Leap
using Leap;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Hand
using RecognitionGestureFeed_Universal.Recognition.Leap.HandStructure;

namespace RecognitionGestureFeed_Universal.Gesture.Leap.Leap_Djestit
{
    internal class LeapToken : Token
    {
        /* Attributi */
        // Scheletro associato al token
        public HandClone hand;
        // Tipo di token (Start, Move o End)
        public TypeToken type;
        // Buffer che contiene gli n scheletri precedentemente rilevati
        public List<HandClone> precHands;
        // Identificativo associato allo scheletro
        public int identifier;

        /* Costruttore */
        public LeapToken(TypeToken type, HandClone hand)
        {
            // Ricave le informazioni di cui necessità
            this.hand = (HandClone)hand.Clone();
            this.type = type;
            this.identifier = hand.Id;
        }
    }
}
