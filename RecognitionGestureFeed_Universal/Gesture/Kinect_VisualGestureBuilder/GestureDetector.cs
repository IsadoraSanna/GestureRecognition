using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Debug
using System.Diagnostics;
// Kinect
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect_VisualGestureBuilder
{
    // Delegate dell'evento che viene lanciato quando si esegue una gesture discreta
    public delegate void DiscreteGestureExecute(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture, DiscreteGestureResult result);
    // Delegate dell'evento che viene lanciato quando si esegue una gesture discreta
    public delegate void ContinuousGestureExecute(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture, ContinuousGestureResult result);

    public class GestureDetector
    {
        /* Eventi */
        DiscreteGestureExecute DiscreteGestureExecute;
        ContinuousGestureExecute ContinuousGestureExecute;

        /* Attributi */
        /// <summary> 
        /// Il Path del database in cui risiedono le gesture di prova
        /// </summary>
        private readonly string gestureDatabase = @"C:\Users\BatCave\Copy\Tesi\Carca\Programma_di_Prova\Programma_di_Prova\Database\database_gesture_burde.gbd";
        /// <summary> 
        /// Rispettivamente:
        /// Gesture Frame Source che verrà legato al body da seguire
        /// Gesture Frame Reader che verrà usato per gestire gli eventi rilevati dalla Kinect
        /// </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;
        private VisualGestureBuilderFrameReader vgbFrameReader = null;
        // Soglia di accettazione di una certa gesture
        private readonly float threshold = 0.95f;

        /* Costruttore */
        /// <summary>
        /// Costruttore che carica tutte le gesture del database
        /// </summary>
        /// <param name="kinectSensor"></param>
        /// <param name="pathDatabase">Indirizzo in cui risiede il database da cui prelevare le gesture</param>
        public GestureDetector(KinectSensor kinectSensor, string pathDatabase)
        {
            init(kinectSensor);

            // Carico tutte le gesture che voglio rilevare dal database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(pathDatabase))
            {
                // Carichiamo tutte le gesture disponibili nel database attraverso la chiamata 
                this.vgbFrameSource.AddGestures(database.AvailableGestures);
                // qualora volessimo caricarne solo alcune, possiamo specificarlo direttamente tramite nome.
                //foreach (Gesture gesture in database.AvailableGestures)
                //{
                    //if (gesture.Name.Equals(this.GestureName))
                    //{                
                    //this.vgbFrameSource.AddGesture(gesture);
                    //}
                //}
            }
        }
        /// <summary>
        /// Costruttore che carica le gesture specificate dall'utente
        /// </summary>
        /// <param name="kinectSensor"></param>
        /// <param name="pathDatabase"></param>
        /// <param name="namesGesture"></param>
        public GestureDetector(KinectSensor kinectSensor, string pathDatabase, List<String> namesGesture)
        {
            init(kinectSensor);

            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(pathDatabase))
            {
                // Confronto il nome di ogni Gesture presente nel database con i nomi inviati dall'utente.
                // Quando c'è un riscontro positivo, carico la gesture.
                foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in database.AvailableGestures)
                {
                    foreach(String name in namesGesture)
                    {
                        if (gesture.Name.Equals(name))
                        {
                            this.vgbFrameSource.AddGesture(gesture);
                            break;
                        }
                    }
                }
            }
        }

        /* Metodi */
        /// <summary>
        /// Inizializza gli elementi della classe
        /// </summary>
        /// <param name="kinectSensor"></param>
        private void init(KinectSensor kinectSensor)
        {
            if (kinectSensor == null)
                throw new ArgumentNullException("kinectSensor");

            /// <summary>
            /// Creo il VisualGestureBuilder source e lo assoccio al body rilevato dalla kinect 
            /// (questo se il body contenuto nel frame è valido). Indi si provede a gestire l'evento TrackingIdLost
            /// (che si attiva quando il body non è più rilevabile dalla kinect.
            /// </summary>
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += vgbFrameSource_TrackingIdLost;

            // Attivo il reader per i VisualGestureBuilder frame
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                // Se il frame è stato inizializzato, allora lo metto in pausa e associo l'evento FrameArrived al suo gestore
                //this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.vgbFrameReader_FrameArrived;
            }
        }
        /// <summary>
        /// Handle che gestisce l'arrivo di un Gesture frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        void vgbFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)// se il frame non è nullo
                {
                    // Creo un dizionario, in cui per ogni gesture discreta è associato il suo valore di confidenza
                    IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
                    // Creo un dizionario, in cui per ogni gesture discreta è associato il suo valore di confidenza
                    IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, ContinuousGestureResult> continuousResult = frame.ContinuousGestureResults;

                    /* Gesture Discrete */
                    if (discreteResults != null)// Se è stata rilevata almeno un gesture discreta
                    {
                        // Per ogni gesture discreta rilevata e contenuta nel frame, verifico la sua somiglianza
                        // con quelle contenute nel database
                        foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in vgbFrameSource.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Discrete)
                            {
                                // Prendo dalla mappa contenuta in discreteResult il valore di confidenza della gesture
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                /// Quindi se il valore di confidenza della gesture è superiore
                                /// alla soglia impostata, alllora lo comunico (tramite evento) all'utente.
                                if (result != null && result.Confidence > this.threshold)
                                {
                                    // Genero l'evento che informa il completamento della gesture
                                    OnDiscreteGesture(gesture, result);
                                }
                            }
                        }
                    }
                    /* Gesture Continue */
                    if (continuousResult != null)// Se è stata rilevata almeno un gesture continua
                    {
                        // Per ogni gesture continua rilevata e contenuta nel frame, verifico la sua somiglianza
                        // con quelle contenute nel database
                        foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in vgbFrameSource.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Continuous)
                            {
                                // Prendo dalla mappa contenuta in discreteResult il valore di confidenza della gesture
                                ContinuousGestureResult result = null;
                                continuousResult.TryGetValue(gesture, out result);

                                // Quindi se il valore di confidenza della gesture è nulla non faccio,
                                // viceversa comunico che la gesture in questione è stata rilevata
                                if (result != null && result.Progress > this.threshold)
                                {
                                    OnContinuousGesture(gesture, result);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Quando viene rilevata una Gesture Discreta
        /// </summary>
        private void OnDiscreteGesture(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture, DiscreteGestureResult result)
        {
            if (DiscreteGestureExecute != null)
                DiscreteGestureExecute(gesture, result);
        }
        /// <summary>
        /// Quando viene rilevata una Gesture Continua
        /// </summary>
        private void OnContinuousGesture(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture, ContinuousGestureResult result)
        {
            if (ContinuousGestureExecute != null)
                ContinuousGestureExecute(gesture, result);
        }
        /// <summary>
        /// Handle che viene richiamato quando la kinect non rileva più un body
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void vgbFrameSource_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Get/Set del body Tracking ID associato al body attuamente rilevato.
        /// Il Tracking ID cambia se il body non viene più rilevato o viceversa.
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }
            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Get/Set associato al lettore di Frame del vgb.
        /// Se il body Tracking ID associato allo scheletro è falso, allora il lettore viene messo in pausa.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }
    }
}
