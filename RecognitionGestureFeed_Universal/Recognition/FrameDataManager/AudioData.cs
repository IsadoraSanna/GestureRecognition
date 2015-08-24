using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Recognition.FrameDataManager
{
    public class AudioData
    {
        /* Attributi */
        // Angolo del suono e la sua confidenza
        public float beamAngle { get; private set; }
        public float BeamAngleConfidence { get; private set; }
        // AudioBeamMode
        public AudioBeamMode audioBeamMode { get; private set; }
        // Indica a quale corpo fa riferimento
        public IReadOnlyList<AudioBodyCorrelation> AudioBodyCorrelations { get; private set; }
        // Lunghezza del frame in byte
        public uint FrameLengthInBytes { get; private set; }
        // Array in byte che descrive il frame
        public byte[] frameData { get; private set; }
        // Durata del comando percepito
        public TimeSpan duration { get; private set; }
        // Tempo relativo di durata
        public TimeSpan relativeTime { get; private set; }

        /* Costruttore */
        public AudioData(AudioBeamSubFrame frame)
        {
            // Aggiorna le informazioni
            this.beamAngle = frame.BeamAngle;
            this.BeamAngleConfidence = frame.BeamAngleConfidence;
            this.duration = frame.Duration;
            this.FrameLengthInBytes = frame.FrameLengthInBytes;
            this.relativeTime = frame.RelativeTime;
            this.audioBeamMode = frame.AudioBeamMode;
            this.AudioBodyCorrelations = frame.AudioBodyCorrelations;
            frame.CopyFrameDataToArray(frameData);
        }
        public AudioData() 
        { 
        
        }

        /* Metodi */

    }
}
