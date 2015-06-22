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
        //
        public float beamAngle { get; private set; }
        public float BeamAngleConfidence { get; private set; }
        //
        public AudioBeamMode audioBeamMode { get; private set; }
        public IReadOnlyList<AudioBodyCorrelation> AudioBodyCorrelations { get; private set; }
        //
        public uint FrameLengthInBytes { get; private set; }
        public byte[] frameData { get; private set; }
        //
        public TimeSpan duration { get; private set; }
        public TimeSpan relativeTime { get; private set; }

        /* Costruttore */
        public AudioData(AudioBeamSubFrame frame)
        {
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
