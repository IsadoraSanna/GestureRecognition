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
    public enum TypeToken{
        Start,
        Move,
        End
    }

    public class BodyToken : Token
    {
        /* Attributi */
        /// <summary>
        /// Il GestureToken rapressenta un input qualunque dell'utente (in questo caso il suo scheletro). 
        /// In sostanza si provvede a mettere in GestureToken le informazioni riguardanti un token (ad esempio
        /// id, coordinate, JointType ecc. del singolo Joint).
        /// </summary>
        
        public Skeleton skeleton { get; set; }
        public ulong identifier { get; set; }
        // Tipo di token
        public TypeToken type { get; set; }//*************************************** Vedere con Davidino

        /* Costruttore */
        public BodyToken(TypeToken type, Skeleton skeleton)
        {
            // Creo un joint uguale a quello in input e lo assegno al joint di GestureToken.
            /*this.jointInformation = (JointInformation)jointInformation.Clone();
            this.identifier = jointInformation.getId();*/
            this.skeleton = (Skeleton)skeleton.Clone();
            this.type = type;
        }
    }
}
