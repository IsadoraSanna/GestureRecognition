using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Diagnostics;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

namespace RecognitionGestureFeed_Universal.GestureManager
{
    public class GestureDetector
    {
        /// <summary> 
        /// Il Path del database in cui risiedono le gesture
        /// </summary>
        private readonly string gestureDatabase = @"C:\Users\BatCave\Copy\Tesi\Carca\Programma_di_Prova\Programma_di_Prova\Database\database_gesture_burde.gbd";

        /// <summary> 
        /// Rispettivamente:
        /// Gesture Frame Source che verrà legato al body da seguire
        /// Gesture Frame Reader che verrà usato per gestire gli eventi rilevati dalla Kinect
        /// </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        public GestureDetector(KinectSensor kinectSensor)
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

            // Carico le varie gesture che voglio rilevare dal database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.gestureDatabase))
            {
                // Carichiamo tutte le gesture disponibili nel database attraverso la chiamata 
                // this.vgbFrameSource.AddGestures(databaseName.AvailableGestures);
                // qualora volessimo caricarne solo alcune, possiamo specificarlo direttamente tramite nome.
                foreach (Gesture gesture in database.AvailableGestures)
                {
                    //if (gesture.Name.Equals(this.GestureName))
                    //{                
                         this.vgbFrameSource.AddGesture(gesture);
                    //}
                }
            }
        }

        // Handle che gestisce l'arrivo di un Gesture frame 
        void vgbFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)// se il frame non è nullo
                {
                    // Prendo tutti i DiscreteGesture rilevati e contenuti nell'ultimo frame arrivato
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
                    
                    if (discreteResults != null)// Quindi se è stato rilevato almeno un DiscreteGesture
                    {
                        // Per ogni DiscreteGesture rilevata e contenuta nel frame, verifico la sua somiglianza
                        // con quelle contenute nel database
                        foreach (Gesture gesture in vgbFrameSource.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Discrete)//gesture.Name.Equals(this.GestureName) && gesture.GestureType == GestureType.Discrete)
                            {
                                // Non so cosa sto facendo; sicuramente sto dichiarando una variabile
                                // di tipo DiscreteGestureResult che, presumo, utilizzerò per controllare 
                                // se tale gesture è stata rilevata o meno.
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // 
                                    Debug.WriteLine(result.Confidence);
                                    // Aggiorno la visualizzione in base al contenuto giunto dalla Kinect (feed)
                                    // this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Handle che viene richiamato quando la kinect non rileva più un body
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
