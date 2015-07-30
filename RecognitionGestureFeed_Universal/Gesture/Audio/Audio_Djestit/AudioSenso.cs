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
    // Delegate che comunica la gestione di un AudioToken
    public delegate void SkeletonEventHandler(object sender, AudioEventArgs s);

    public class AudioSensor
    {
        // Eventi generati all'arrivo di un scheletro
        public event SkeletonEventHandler onSkeletonStart;
        public event SkeletonEventHandler onSkeletonMove;
        public event SkeletonEventHandler onSkeletonEnd;
        /* Attributi  */
        private int capacity;
        public Term root;
        public AudioStateSequence sequence;

        public AudioSensor(Term root, int capacity)
        {
            this.capacity = capacity;
            this.root = root;
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
                    _onSkeletonStart(e);
                    break;
                case TypeToken.Move:
                    // Genero l'evento onSkeletonStart
                    _onSkeletonMove(e);
                    // Copio la lista di vecchi token
                    List<AudioToken> listToken;
                    sequence.speechs.TryGetValue(token.identifier, out listToken);
                    token.precSpeechRecognized = listToken.Select(item => (SpeechRecognizedEventArgs)item.speechRecognized).ToList();
                    break;
                case TypeToken.End:
                    // Genero l'evento onSkeletonStart
                    _onSkeletonEnd(e);
                    // Rimuovo lo scheletro in questione dalla mappa
                    sequence.removeById(token.identifier);

                    break;
            }
            // Se lo scheletro gestito non è di tipo end, allora si provvede ad inserirlo nel buffer
            if (type != TypeToken.End)
                this.sequence.push(token);
            return token;
        }

        /* Handler eventi di start, move ed end */
        /// <summary>
        /// Handler per l'evento start
        /// </summary>
        /// <param name="sender"></param>
        public void _onSkeletonStart(AudioEventArgs sender)
        {
            if (onSkeletonStart != null)
                onSkeletonStart(this, sender);
        }
        /// <summary>
        /// Handler per l'evento move
        /// </summary>
        /// <param name="sender"></param>
        public void _onSkeletonMove(AudioEventArgs sender)
        {
            if (onSkeletonMove != null)
                onSkeletonMove(this, sender);
        }
        /// <summary>
        /// Handler per l'evento end
        /// </summary>
        /// <param name="sender"></param>
        public void _onSkeletonEnd(AudioEventArgs sender)
        {
            if (onSkeletonEnd != null)
                onSkeletonEnd(this, sender);
        }

        /// <summary>
        /// Verifica se in state sequence è già presente quello scheletro.
        /// </summary>
        /// <param name="skeletonId"></param>
        /// <returns></returns>
        public bool checkSpeech(int identifier)
        {
            if (this.sequence.speechs.ContainsKey(identifier))
                return true;
            else
                return false;
        }

    }
}
