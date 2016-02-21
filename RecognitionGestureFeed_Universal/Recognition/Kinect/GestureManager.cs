using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
// RecognitionGestureFeed
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;
using RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_VisualGestureBuilder;

namespace RecognitionGestureFeed_Universal.Recognition.Kinect
{
    public class GestureManager
    {
        /**** Attributi ****/
        // Lista di GestureDetector, una per ogni scheletro rilevato.
        public List<GestureDetector> gestureDetectorList { get; private set; }

        /**** Metodi ****/
        /// <summary>
        /// Avvia il rilevamento di Gesture tramite i metodi forniti dalla Kinect
        /// </summary>
        /// <param name="acquisitionManager">Una variabile di tipo AcquisitionManager</param>
        /// <param name="databasePath">Path in cui si trova il database da utilizzare</param>
        public void StartGesture(AcquisitionManager acquisitionManager, string databasePath)
        {
            // Verifica se i parametri passati sono pronti per l'utilizzo
            if (acquisitionManager == null)
                throw new ArgumentNullException("AcquisitionManager is Null.");
            if (databasePath == "")
                throw new ArgumentException("Path not correct.");

            // Inizializza la lista di GestureDetector
            gestureDetectorList = new List<GestureDetector>();
            for (int i = 0; i < acquisitionManager.numSkeletons; i++)
            {
                GestureDetector detector = new GestureDetector(acquisitionManager.kinectSensor, databasePath);
                gestureDetectorList.Add(detector);
            }

            // Associo l'handler updateStream all'evento frameManaged
            acquisitionManager.SkeletonsFrameManaged += UpdateGesture;
        }
        /// <summary>
        /// Avvia il rilevamento di Gesture tramite i metodi forniti dalla Kinect delle sole gesture specificate dall'utente
        /// </summary>
        /// <param name="acquisitionManager">Variabile di tipo AcquisitionManager</param>
        /// <param name="databasePath">Path in cui si trova il database da utilizzare</param>
        /// <param name="namesGesture">Lista delle gesture che si vogliono riconoscere</param>
        public void StartGesture(AcquisitionManager acquisitionManager, string databasePath, List<string> gestureNames)
        {
            // Verifica se i parametri passati sono pronti per l'utilizzo
            if (acquisitionManager == null)
                throw new ArgumentNullException("AcquisitionManager is Null.");
            if (databasePath == "")
                throw new ArgumentException("Path not correct.");
            if (gestureNames.Count == 0)
                throw new ArgumentException("Gesture List is Empyt.");


            // Inizializza la lista di GestureDetector
            gestureDetectorList = new List<GestureDetector>();
            for (int i = 0; i < acquisitionManager.numSkeletons; i++)
            {
                GestureDetector detector = new GestureDetector(acquisitionManager.kinectSensor, databasePath, gestureNames);
                gestureDetectorList.Add(detector);
            }

            // Associa l'handler updateStream all'evento frameManaged
            acquisitionManager.SkeletonsFrameManaged += UpdateGesture;
        }

        private void UpdateGesture(Skeleton[] sender)
        {
            /// Rilevamento Gesture
            /// Se TrackingID del body cambia, aggiorno la gesture detector corrispondente col nuovo valore.
            int i = 0;
            foreach (Skeleton skeleton in sender)
            {
                ulong trackingId = skeleton.getIdBody();
                if (trackingId != gestureDetectorList[i].TrackingId)
                {
                    gestureDetectorList[i].TrackingId = trackingId;
                    // Se il body è tracciato, il suo detector esce dalla pausa per catturare gli eventi VisualGestureBuilderFrameArrived.
                    // Altrimenti il suo detector rimane in pausa e non sprechiamo risorse cercando di gestire gesture invalide.
                    gestureDetectorList[i].IsPaused = trackingId == 0;
                }
                i++;// aggiorno l'indice
            }
        }
    }
}