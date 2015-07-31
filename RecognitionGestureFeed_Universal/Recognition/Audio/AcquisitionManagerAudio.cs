using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// XML
using System.Xml;
using System.Xml.Linq;
// Memory Stream
using System.IO;
// Kinect
using Microsoft.Kinect;
// Audio Data
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;
// Speech Data
using RecognitionGestureFeed_Universal.Gesture.Audio_Djestit;
// Microsoft Speech Platform
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace RecognitionGestureFeed_Universal.Recognition.Audio
{
    // Delegate event che indica quando viene gestito un frame di tipo audio  
    public delegate void AudioFrameManaged(AcquisitionManagerAudio sender);
    // Delegate event che avvisa quando viene gestito un frame che contiene commandi vocali
    public delegate void SpeechFrameManaged(AcquisitionManagerAudio sender, SpeechRecognizedEventArgs result);

    public class AcquisitionManagerAudio
    {
            /* Eventi */
            public event AudioFrameManaged AudioFrameManaged;
            public event SpeechFrameManaged SpeechUpdate;

            /* Attributi */
            // Sensore Kinect
            private KinectSensor kinectSensor = null;
            // Audio
            private AudioSource audioSource = null;
            public List<AudioData> audioDataList { get; private set; }
            // Speech
            public SpeechRecognitionEngine speechEngine = null;
            public KinectAudioStream convertStream = null;

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
            // Convert
            convertStream = new KinectAudioStream(audioStream);

            // Crea il RecognizerInfo
            RecognizerInfo ri = TryGetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

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

                /////// Letturafile
                string fileName = @"C:\Users\BatCave\Copy\Tesi\Programmi\Esempi Kinect\SpeechBasics-WPF\SpeechGrammar.xml";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fileName);
                StringWriter sw = new StringWriter();
	            XmlTextWriter xw = new XmlTextWriter(sw);
	            doc.WriteTo(xw);
                // Prende il file XML in cui si trova la grammatica (prenderla da input)
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(sw.ToString())))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;
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
        /// <param name="result">event arguments.</param>
        private void SpeechRecognized(object obj, SpeechRecognizedEventArgs result)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (result.Result.Confidence >= ConfidenceThreshold)
            {
                // Genera evento collegato al riconoscimento di un comando vocale corretto
                if (this.SpeechUpdate != null)
                    this.SpeechUpdate(this, result);
            }
        }
    }
}
