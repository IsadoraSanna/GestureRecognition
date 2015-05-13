using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// JointInformation
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{
    class GestureToken
    {
        /* Attributi */
        /// <summary>
        /// Il GestureToken rapressenta un input qualunque dell'utente (in questo caso il suo scheletro). 
        /// In sostanza si provvede a mettere in GestureToken tutte le informazioni riguardanti un token (ad esempio
        /// id, coordinate, JointType ecc.).
        /// </summary>
        Skeleton skeleton;

        /* Costruttore */
        public GestureToken(Skeleton skeletonInput)
        {
            // Creo un joint uguale a quello in input e lo assegno al joint di GestureToken.
            this.skeleton = (Skeleton)skeletonInput.Clone();
        }
    }
}
