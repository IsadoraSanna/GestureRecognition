using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using Unica.Djestit;
// AudioSpeech
using Microsoft.Speech.Recognition;

namespace Unica.Djestit.Audio
{
    public class AudioToken : Token
    {
        /* Attributi */
        public TypeToken type { get; private set; }
        public int identifier { get; private set; }
        public SpeechRecognizedEventArgs speechRecognized { get; private set; }
        public List<SpeechRecognizedEventArgs> precSpeechRecognized { get; internal set; }

        /* Costruttore */
        public AudioToken(TypeToken type, int identifier, SpeechRecognizedEventArgs speechRecognized)
        {
            this.type = type;
            this.identifier = identifier;
            this.speechRecognized = speechRecognized;
            this.precSpeechRecognized = new List<SpeechRecognizedEventArgs>();
        }

        /* Metodi */
    }
}
