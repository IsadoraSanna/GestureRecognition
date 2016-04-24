using Leap;
using Unica.Djestit.Leap;
using Unica.Djestit.Recognition.FrameDataManager;

namespace Unica.Djestit.Recognition.Leap
{
    public class LeapSensorInterface
    {
        /* Attributi */
        // Sensore usato per comunicare al motorino i nuovi token
        internal LeapSensor sensor;
        // Capacità del buffer
        internal int capacity = 5;

        /* Costruttore */
        public LeapSensorInterface(AcquisitionManagerLeap aleap, Term expression)
        {
            // Inizializzazione sensore
            this.sensor = new LeapSensor(expression, this.capacity);
            // Si collega all'evento lanciato quando arriva un nuovo frame dal Leap Motion
            aleap._OnFrame += update;
        }

        private void update(LeapData leapData)
        {
            // Per ogni mano rilevata dal leap
            foreach(Hand hand in leapData.handList)
            {
                // Creo un LeapToken
                LeapToken token = null;
                if(hand.IsValid)
                {
                    if(sensor.checkId(hand.Id))
                        token = (LeapToken)sensor.generateToken(TypeToken.Move, hand);
                    else 
                        token = (LeapToken)sensor.generateToken(TypeToken.Start, hand);
                }
                else if(sensor.checkId(hand.Id))
                    token = (LeapToken)sensor.generateToken(TypeToken.End, hand);

                // Se è stato creato un token, lo sparo al motore
                if (token != null)
                {
                    if (token.type != TypeToken.End)
                        this.sensor.root.fire(token);
                }

                // Se lo stato della choice è in error o complete allora lo riazzero
                if (this.sensor.root.state == expressionState.Error || this.sensor.root.state == expressionState.Complete)
                    this.sensor.root.reset();
            }
        }
    }
}
