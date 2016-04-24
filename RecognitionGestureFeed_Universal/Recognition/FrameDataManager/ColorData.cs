using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// PixelFormat e ColorImageFormat
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
// Kinect
using Microsoft.Kinect;

namespace Unica.Djestit.Recognition.FrameDataManager
{
    /// <summary>
    /// La classe ColorData contiene le informazioni basilari tratte dal ColorFrame.
    /// Nello specifico (oltre agli attributi presenti in FrameData) contiene
    /// un array di tipo byte, che conterrà per ogni pixel dell'immagine, il suo valore in RGB.
    /// </summary>
    public class ColorData : FrameData
    {
        // Array con tutti i valori di rgb per ogni pixel.
        public byte[] pixels { get; private set; }

        /// <summary>
        /// Costruttore che inizializza l'array pixels e gli attributi width ed heigth.
        /// </summary>
        /// <param name="frameDescription"></param>
        public ColorData(FrameDescription frameDescription) : base(frameDescription)
        {
            /// pixels avrà un numero di elementi pari alla larghezza dell'immagine acquisita *
            /// l'altezza dell'immagine acquisita * il numero di bit per pixel (bpp).
            //PixelFormat format = PixelFormats.Bgr32;
            this.pixels = new byte[frameDescription.Width * frameDescription.Height * 4];// ((format.BitsPerPixel + 7) / 8)];
        }

        /// <summary>
        /// Aggiorna il contenuto del ColorData (quindi dell'array pixels) a partire dall'ultimo frame rilevato.
        /// </summary>
        /// <param name="colorFrame"></param>
        public void update(ColorFrame colorFrame)
        {
            /// Copio i dati dell'immagine rilevata nell'array pixels. A seconda del tipo di formato con
            /// cui vengono salvati i dati, utilizzo CopyRaw oppure CopyConverted.
            colorFrame.CopyConvertedFrameDataToArray(this.pixels, ColorImageFormat.Bgra);
        }
    }
}
