using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace Unica.Djestit.Recognition.FrameDataManager
{
    /// <summary>
    /// La classe InfraredData contiene le informazioni basilari tratte dall'InfraredFrame.
    /// Oltre agli attributi del padre, InfraredData contiene anche un array di tipo 
    /// ushort, che viene usato per contenere i valori di infrarosso prelevati dall'ultimo frame
    /// inviato dalla kinect.
    /// </summary>
    public class InfraredData : FrameData
    {
        // Array che contiene tutti i valori di profondità di un frame rilevato dalla kinect
        public ushort[] frameData { get; private set; }
        
        /// <summary>
        /// Costruttore che inizializza l'array frameData e gli attributi width
        /// ed height.
        /// </summary>
        /// <param name="frameDescription"></param>
        public InfraredData(FrameDescription frameDescription) : base(frameDescription)
        {
            this.frameData = new ushort[frameDescription.Width * frameDescription.Height];
        }

        /// <summary>
        /// Aggiorna il contenuto dell'InfraredData (ovvero dell'array frameData) 
        /// </summary>
        /// <param name="frame"></param>
        public void update(InfraredFrame frame)
        {
            // Aggiorno i valori frameData con quelli contenuti nell'ultimo frame inviato dal sensore.
            frame.CopyFrameDataToArray(this.frameData);
        }
    }
}
