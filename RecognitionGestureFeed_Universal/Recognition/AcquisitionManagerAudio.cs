using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public class AcquisitionManagerAudio
    {
        /* Attributi */
        KinectSensor kinectSensor = null;
        AudioSource audioSource = null;
        // grab the audio stream

        /* Costruttore */
        public AcquisitionManagerAudio(KinectSensor kinectSensor)
        {
            // Avvio il collegamento con la Kinect
            this.kinectSensor = kinectSensor;

            this.audioSource = this.kinectSensor.AudioSource;
            AudioBeamFrameReader reader = this.audioSource.OpenReader();
            reader.FrameArrived += Reader_FrameArrived;

            //IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
            //System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();
        }

        private void Reader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            AudioBeamFrameReference frameReference = e.FrameReference;
            AudioBeamFrameList frameList = frameReference.AcquireBeamFrames();

            /* Metodi */
            if (frameList != null)
            {
                // AudioBeamFrameList is IDisposable
                using (frameList)
                {
                    // Only one audio beam is supported. Get the sub frame list for this beam
                    IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                    // Loop over all sub frames, extract audio buffer and beam information
                    foreach (AudioBeamSubFrame subFrame in subFrameList)
                    {
                        subFrame.
                    }

                }
            }
        }


    }
}
