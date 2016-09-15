using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
// Kinect Gesture
using Microsoft.Kinect.VisualGestureBuilder;
using Unica.Djestit.Recognition.Kinect2;
// KinectSensorExtend

namespace Unica.Djestit.Kinect2.VisualGestureBuilder
{
    public class GestureDetector
    {
        /* Attributi */
        public GestureProgressContinuous gestureProgressContinuous = new GestureProgressContinuous();
        public GestureProgressDiscrete gestureProgressDiscrete = new GestureProgressDiscrete();

        /// <summary> 
        /// Rispettivamente:
        /// Gesture Frame Source che verrà legato al body da seguire
        /// Gesture Frame Reader che verrà usato per gestire gli eventi rilevati dalla Kinect
        /// </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /* Costruttore */
        public GestureDetector(ulong skeletonId, string pathDatabase, [Optional] List<String> namesGesture)
        {
            // Verifica se il file esiste
            if (!File.Exists(pathDatabase))
                throw new Exception("Database don't Exist");

            // Attiva il FrameSource
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(KinectSensorExtend.getSensor(), skeletonId);
            // Attiva il reader per i VisualGestureBuilder frame
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.FrameArrived += this.vgbFrameReader_FrameArrived;
            }

            // Carica tutte le gesture contenute nel database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(pathDatabase))
            {
                // Vengono caricate tutte le gesture presenti nel database, a meno che non venga specificato diversamente 
                // dall'utente.
                if (namesGesture != null)
                {
                    // Confronta il nome di ogni Gesture presente nel database con i nomi inviati dall'utente.
                    // Quando c'è un riscontro positivo, carica la gesture.
                    foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in database.AvailableGestures)
                    {
                        foreach (String name in namesGesture)
                        {
                            if (gesture.Name.Equals(name))
                                this.vgbFrameSource.AddGesture(gesture);
                        }
                    }
                }
                else
                    this.vgbFrameSource.AddGestures(database.AvailableGestures);
            }
        }

        private void vgbFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            // Preleva il frame
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)// se il frame non è nullo
                {
                    // Creo un dizionario, in cui per ogni gesture discreta è associato il suo valore di confidenza
                    IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, ContinuousGestureResult> continuousResult = frame.ContinuousGestureResults;
                    IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, DiscreteGestureResult> discreteGestureResult = frame.DiscreteGestureResults;

                    /* Gesture Discrete */
                    if (discreteGestureResult != null)
                        gestureProgressDiscrete.setLastProgress(frame.TrackingId, frame.RelativeTime, frame.DiscreteGestureResults);
                    /* Gesture Continue */
                    if (continuousResult != null)// Se è stata rilevata almeno un gesture continua
                        gestureProgressContinuous.setLastProgress(frame.TrackingId, frame.RelativeTime, frame.ContinuousGestureResults);
                }
            }
        }

        /// <summary>
        /// Aggiorna il tracking Id di vgbFrameSource.
        /// </summary>
        /// <param name="id"></param>
        public void updateId(ulong id)
        {
            vgbFrameSource.TrackingId = id;
        }
    }
}
