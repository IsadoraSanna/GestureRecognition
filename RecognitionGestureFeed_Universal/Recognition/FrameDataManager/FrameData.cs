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
    /// La Classe astratta FrameData fornisce una prima definizione delle classi che descrivono
    /// i frame in arrivo dalla kinect.
    /// </summary>
    public abstract class FrameData
    {
        // Altezza e Larghezza dell'immagine. Tale informazione viene prelevata dalla descrizione
        // del frame stesso.
        public int height { get; private set; }
        public int width { get; private set; }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="frameDescription"></param>
        public FrameData(FrameDescription frameDescription)
        {
            this.height = frameDescription.Height;
            this.width = frameDescription.Width;
        }
        public FrameData()
        {

        }
    }
}
