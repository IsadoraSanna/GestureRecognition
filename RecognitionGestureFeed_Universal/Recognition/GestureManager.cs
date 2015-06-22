using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
// Kinect
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
// RGF
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Stream;
using RecognitionGestureFeed_Universal.Gesture.Kinect_VisualGestureBuilder;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public static class GestureManager
    {
        /**** Attributi ****/
        // Lista di GestureDetector, una per ogni scheletro rilevato.
        private static List<GestureDetector> gestureDetectorList = null;

        /**** Metodi ****/
        /// <summary>
        /// Avvia il rilevamento di Gesture tramite i metodi forniti dalla Kinect
        /// </summary>
        /// <param name="am">Una variabile di tipo AcquisitionManager</param>
        /// <param name="pathDatabase">Path in cui si trova la variabile di partenza</param>
        public static void startGesture(AcquisitionManager am, string pathDatabase)
        {
            if(pathDatabase == "")
                throw new ArgumentNullException("Path not correct.");

            // Inizializzo la lista di GestureDetector
            gestureDetectorList = new List<GestureDetector>();
            for (int i = 0; i < am.numSkeletons; i++)
            {
                GestureDetector detector = new GestureDetector(am.kinectSensor, pathDatabase);
                gestureDetectorList.Add(detector);
            }

            // Associo l'handler updateStream all'evento frameManaged
            am.SkeletonFrameManaged += updateGesture;
        }
        /// <summary>
        /// Avvia il rilevamento di Gesture tramite i metodi forniti dalla Kinect delle sole gesture specificate dall'utente
        /// </summary>
        /// <param name="am"></param>
        /// <param name="pathDatabase"></param>
        /// <param name="namesGesture"></param>
        public static void startGesture(AcquisitionManager am, string pathDatabase, List<String> namesGesture)
        {
            if (pathDatabase == "")
                throw new ArgumentNullException("Path not correct.");

            // Inizializzo la lista di GestureDetector
            gestureDetectorList = new List<GestureDetector>();
            for (int i = 0; i < am.numSkeletons; i++)
            {
                GestureDetector detector = new GestureDetector(am.kinectSensor, pathDatabase, namesGesture);
                gestureDetectorList.Add(detector);
            }

            // Associo l'handler updateStream all'evento frameManaged
            am.SkeletonFrameManaged += updateGesture;
        }

        public static void updateGesture(Skeleton[] sender)
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