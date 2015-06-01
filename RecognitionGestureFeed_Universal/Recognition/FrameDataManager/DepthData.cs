using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Recognition.FrameDataManager
{
    /// <summary>
    /// La Classe DepthData contiene le informazioni basilari tratte dal frame in arrivo sul sensore.
    /// </summary>
    public class DepthData : FrameData
    {
        // frameData è l'array che contiene i valori di profondità rilevati dalla kinect
        public ushort[] frameData { get; private set; }
        // Minima e massima profondità rilevati
        public ushort maxDepth { get; private set; }
        public ushort minDepth { get; private set; }

        /// <summary>
        /// Inizializza gli oggetti del depthData. Per farlo usa anche il costruttore del padre (per width e height).
        /// </summary>
        /// <param name="frameDescription"></param>
        public DepthData(FrameDescription frameDescription) : base(frameDescription)
        {
            this.frameData = new ushort[frameDescription.Width * frameDescription.Height];
            this.maxDepth = 0;
            this.minDepth = 0;
        }

        /// <summary>
        /// Aggiorna il contenuto del depthData (quindi array, max e min depth).
        /// </summary>
        /// <param name="frame"></param>
        public void update(DepthFrame frame)
        {
            // Aggiorno il contenuto dell'array
            frame.CopyFrameDataToArray(this.frameData);
            // Aggiorno i valori di minima e massima profondità
            this.minDepth = frame.DepthMinReliableDistance;
            this.maxDepth = frame.DepthMaxReliableDistance;
        }
    }
}
