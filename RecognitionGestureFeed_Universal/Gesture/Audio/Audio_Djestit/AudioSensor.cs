using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// AudioSpeech
using Microsoft.Speech.Recognition;

namespace RecognitionGestureFeed_Universal.Gesture.Audio_Djestit
{
    public class AudioSensor : Sensor
    {
        /* Attributi  */
        AudioStateSequence sequence;

        /* Costruttore */
        public AudioSensor(Term root, int capacity) : base(root, capacity)
        {
            this.sequence = new AudioStateSequence(this.capacity);
        }

        public Token generateToken(TypeToken type, SpeechRecognizedEventArgs audio)
        {
            // Creo uno SkeletonToken a partire dallo Skeleton ricevuto in input
            AudioToken token = new AudioToken(type, 0, audio);
            AudioEventArgs e = new AudioEventArgs(this);
            switch (type)
            {
                case TypeToken.Start:
                    // Genero l'evento onSkeletonStart
                    this._onTokenStart(e);
                    break;
                case TypeToken.Move:
                    // Genero l'evento onSkeletonStart
                    this._onTokenMove(e);
                    // Copio la lista di vecchi token
                    List<AudioToken> listToken;
                    sequence.speechs.TryGetValue(token.identifier, out listToken);
                    token.precSpeechRecognized = listToken.Select(item => (SpeechRecognizedEventArgs)item.speechRecognized).ToList();
                    break;
                case TypeToken.End:
                    // Genero l'evento onSkeletonStart
                    this._onTokenEnd(e);
                    // Rimuovo lo scheletro in questione dalla mappa
                    sequence.removeById(token.identifier);

                    break;
            }
            // Se lo scheletro gestito non è di tipo end, allora si provvede ad inserirlo nel buffer
            if (type != TypeToken.End)
                this.sequence.push(token);
            return token;
        }

        /// <summary>
        /// Verifica se in state sequence è già presente quello scheletro.
        /// </summary>
        /// <param name="skeletonId"></param>
        /// <returns></returns>
        public override bool checkId(int identifier)
        {
            if (this.sequence.speechs.ContainsKey(identifier))
                return true;
            else
                return false;
        }
    }
}
