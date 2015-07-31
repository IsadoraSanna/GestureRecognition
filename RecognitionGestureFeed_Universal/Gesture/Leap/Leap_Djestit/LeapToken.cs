using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Leap
using Leap;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Copy
using RecognitionGestureFeed_Universal.Utilities;

namespace RecognitionGestureFeed_Universal.Gesture.Leap.Leap_Djestit
{
    internal class LeapToken : Token
    {
        /* Attributi */
        // Scheletro associato al token
        public Hand hand;
        // Tipo di token (Start, Move o End)
        public TypeToken type;
        // Buffer che contiene gli n scheletri precedentemente rilevati
        public List<Hand> precHands;
        // Identificativo associato allo scheletro
        public int identifier;

        /* Costruttore */
        public LeapToken(TypeToken type, Hand hand)
        {
            // Ricave le informazioni di cui necessità
            this.hand = (Hand)hand.CloneObject();
            this.type = type;
            this.precHands = new List<Hand>();
            this.identifier = hand.Id;
        }
    }
}
