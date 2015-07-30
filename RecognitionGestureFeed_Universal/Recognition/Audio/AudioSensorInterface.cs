using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using RecognitionGestureFeed_Universal.Djestit;
// Audio_Djestit
using RecognitionGestureFeed_Universal.Gesture.Audio_Djestit;
using RecognitionGestureFeed_Universal.Gesture;
// Microsoft Speech
using Microsoft.Speech.Recognition;

namespace RecognitionGestureFeed_Universal.Recognition
{
    class AudioSensorInterface
    {
        // Attributi
        internal AudioSensor sensor;

        public AudioSensorInterface(AcquisitionManagerAudio am, Term expression)
        {
            this.sensor = new AudioSensor(expression, 3);
            am.SpeechUpdate += update;
        }

        private void update(AcquisitionManagerAudio sender, SpeechRecognizedEventArgs result)
        {
            AudioToken token = null;

            if (this.sensor.checkSpeech(0))
                token = (AudioToken)sensor.generateToken(TypeToken.Move, result);
            else
                token = (AudioToken)sensor.generateToken(TypeToken.Start, result);

            if (token != null)
                this.sensor.root.fire(token);

            // Se lo stato della choice è in error o complete allora lo riazzero
            if (this.sensor.root.state == expressionState.Error || this.sensor.root.state == expressionState.Complete)
                this.sensor.root.reset();

            /*// Per ogni scheletro rilevato avvio il motorino
            foreach (Skeleton skeleton in skeletonList)
            {
                // Creo uno skeleton token
                SkeletonToken token = null;
                // Determino il tipo (Start, Move o End) e ne creo il token, e quindo lo genero
                if (skeleton.getStatus())
                {
                    if (sensor.checkSkeleton(skeleton.getIdSkeleton()))
                        token = (SkeletonToken)sensor.generateToken(TypeToken.Move, skeleton);
                    else
                        token = (SkeletonToken)sensor.generateToken(TypeToken.Start, skeleton);
                }
                else if (sensor.checkSkeleton(skeleton.getIdSkeleton()))
                {
                    token = (SkeletonToken)sensor.generateToken(TypeToken.End, skeleton);
                }

                // Se è stato creato un token, lo sparo al motore
                if (token != null)
                {
                    if (token.type != TypeToken.End)
                        this.sensor.root.fire(token);
                }

                // Se lo stato della choice è in error o complete allora lo riazzero
                if (this.sensor.root.state == expressionState.Error || this.sensor.root.state == expressionState.Complete)
                    this.sensor.root.reset();
            }*/
        }
    }
}
