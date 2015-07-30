using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Leap Motion Library
using Leap;
// LeapData
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;

// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Recognition.Leap
{
    public delegate void ListenerEvent(LeapData leapData);

    public class AcquisitionManagerLeap : Listener
    {
        /* Eventi */
        // Evento lanciato quando arriva un nuovo frame dal Leap Motion
        public event ListenerEvent _OnFrame;
        // Evento lanciato nel momento in cui si inizializza il Listener
        public event ListenerEvent _OnInit;
        // Evento lanciato quando viene creata la connessione tra il sistema e il Leap Motion
        public event ListenerEvent _OnConnect;
        // Evento lanciato quando il dispositivo cambia stato
        public event ListenerEvent _OnDeviceChange;
        // Evento lanciato quando viene persa la connessione con il Leap
        public event ListenerEvent _OnDisconnect;
        // Evento lanciato quando viene chiuso il programma che comunica con il Leap
        public event ListenerEvent _OnExit;
        // Evento lanciato quando il programma ottiene il focus
        public event ListenerEvent _OnFocusGained;
        // Evento lanciato quando il programma perde il focus
        public event ListenerEvent _OnFocusLost;
        // Evento lanciato quando arriva un immagine
        public event ListenerEvent _OnImages;
        // Evento lanciato quando il deamon del Leap Motion si connette al controller dell'applicazione
        public event ListenerEvent _OnServiceConnect;
        // Evento lanciato quando il deamon del Leap Motion si disconnette al controller dell'applicazione
        public event ListenerEvent _OnServiceDisconnect;

        /* Attributi */
        // Controller usato per creare la connessione con il Leap Motion
        private Controller controller = new Controller();
        // Object che contiene i dati ricevuti dal Leap Motion
        private LeapData leapData = new LeapData();

        /* Costruttore */
        public AcquisitionManagerLeap(Controller controller) : base()
        {
            this.controller = controller;
        }

        /* Metodi */
        /// <summary>
        /// La funzione viene richiamata nel momento in cui il Listener viene collegato al Controller
        /// </summary>
        /// <param name="arg0"></param>
        public override void OnInit(Controller controller)
        {
            Debug.WriteLine("Porcoddio");
            // Abilita il riconoscimento di determinate gesture

        }

        /// <summary>
        /// Funzione che viene chiamata quando si ottiene la connessione al Leap Motion
        /// </summary>
        /// <param name="controller"></param>
        public override void OnConnect(Controller controller)
        {
            Debug.WriteLine("Porcamadonna");

        }

        /// <summary>
        /// Funzione che viene chiamata quando si disconnette il Leap Motion
        /// </summary>
        /// <param name="controller"></param>
        public override void OnDisconnect(Controller controller)
        {
            Debug.WriteLine("DioLatticinoMerdoso");
        }

        /// <summary>
        /// Funzione che viene chiamata quando si rimuove o si esce dal Controller
        /// </summary>
        /// <param name="controller"></param>
        public override void OnExit(Controller controller)
        {

        }

        public override void OnFrame(Controller controller)
        {
            //
            Debug.WriteLine("Dio Boia di bambini indifesi");
            // Acquisisce l'ultimo Frame inviato dal Leap
            Frame frame = controller.Frame();
            // Aggiorna il contenuto di LeapData
            this.leapData.update(frame);
            // Genero l'evento per comunicare l'arrivo di un frame
            if (this._OnFrame != null)
                this._OnFrame(this.leapData);
        }

        public override void OnServiceDisconnect(Controller controller)
        {

        }

        public override void OnServiceConnect(Controller controller)
        {

        }

        public override void OnImages(Controller controller)
        {

        }

        public override void OnFocusGained(Controller controller)
        {

        }

        public override void OnFocusLost(Controller controller)
        {

        }

        public override void OnDeviceChange(Controller controller)
        {

        }

        // Restituisce una copia del Leap Data
        private LeapData GetLeapData()
        {
            return (LeapData)this.leapData.Clone();
        }
    }
}
