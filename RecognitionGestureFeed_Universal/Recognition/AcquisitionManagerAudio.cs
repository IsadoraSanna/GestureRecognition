using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;
// Data
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public delegate void AudioFrameManaged(AcquisitionManagerAudio sender);

    public class AcquisitionManagerAudio
    {
        /* Eventi */
        public event AudioFrameManaged AudioFrameManaged;

        /* Attributi */
        // Sensore Kinect
        private KinectSensor kinectSensor = null;
        // Audio
        private AudioSource audioSource = null;
        public List<AudioData> audioDataList { get; private set; }
        // Speech

        /* Costruttore */
        public AcquisitionManagerAudio(KinectSensor kinectSensor)
        {
            // Avvio il collegamento con la Kinect
            if (kinectSensor == null)
                throw new ArgumentNullException("Kinect not be connect.");
            else
                this.kinectSensor = kinectSensor;

            // Audio
            this.audioSource = this.kinectSensor.AudioSource;
            AudioBeamFrameReader reader = this.audioSource.OpenReader();
            reader.FrameArrived += Reader_FrameArrived;
            // Speech
            //IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
            //System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

            // Inizializzo la lista di AudioData
            this.audioDataList = new List<AudioData>();
        }

        /* Metodi */
        private void Reader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            // Cancello la lista di AudioData
            this.audioDataList.Clear();

            //
            AudioBeamFrameReference frameReference = e.FrameReference;
            AudioBeamFrameList frameList = frameReference.AcquireBeamFrames();

            if (frameList != null)
            {
                // AudioBeamFrameList is IDisposable
                using (frameList)
                {
                    // Only one audio beam is supported. Get the sub frame list for this beam
                    IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                    // Loop over all sub frames, extract audio buffer and beam information
                    foreach (AudioBeamSubFrame subFrame in subFrameList)
                        this.audioDataList.Add(new AudioData(subFrame));

                    // Lancia evento AudioFrameManaged
                    _OnAudioFrameManaged();
                }
            }
        }

        private void _OnAudioFrameManaged()
        {
            if (this.AudioFrameManaged != null)
                this.AudioFrameManaged(this);
        }
    }
}
