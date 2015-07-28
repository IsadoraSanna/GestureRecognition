using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Memory Stream
using System.IO;
// Kinect
using Microsoft.Kinect;
// RecognitionGestureFeed_Universal
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;
// Microsoft Speech Platform
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace RecognitionGestureFeed_Universal.Recognition
{
    // Delegate event che indica quando viene gestito un frame di tipo audio  
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

            // Inizializzo la lista di AudioData
            this.audioDataList = new List<AudioData>();

            /************ Speech Part ************/
            IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
            System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

            RecognizerInfo ri = TryGetKinectRecognizer();
            if (null != ri)
            {
                this.recognitionSpans = new List<Span> { forwardSpan, backSpan, rightSpan, leftSpan };

                SpeechRecognitionEngine speechEngine = new SpeechRecognitionEngine(ri.Id);

                /****************************************************************
                * 
                * Use this code to create grammar programmatically rather than from
                * a grammar file.
                * 
                * var directions = new Choices();
                * directions.Add(new SemanticResultValue("forward", "FORWARD"));
                * directions.Add(new SemanticResultValue("forwards", "FORWARD"));
                * directions.Add(new SemanticResultValue("straight", "FORWARD"));
                * directions.Add(new SemanticResultValue("backward", "BACKWARD"));
                * directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
                * directions.Add(new SemanticResultValue("back", "BACKWARD"));
                * directions.Add(new SemanticResultValue("turn left", "LEFT"));
                * directions.Add(new SemanticResultValue("turn right", "RIGHT"));
                *
                * var gb = new GrammarBuilder { Culture = ri.Culture };
                * gb.Append(directions);
                *
                * var g = new Grammar(gb);
                * 
                ****************************************************************/

                // Prende il file XML in cui si trova la grammatica (prenderla da input)
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += this.SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);
                speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        /* Metodi */
        /// <summary>
        /// Lettura Frame 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        private void _OnAudioFrameManaged()
        {
            if (this.AudioFrameManaged != null)
                this.AudioFrameManaged(this);
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers = SpeechRecognitionEngine.InstalledRecognizers();

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            this.ClearRecognitionHighlights();

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            this.ClearRecognitionHighlights();
        }
    }
}
