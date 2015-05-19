using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
// Add Kinect
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
// RGF
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Stream;
using RecognitionGestureFeed_Universal.GestureManager;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public static class GestureManager
    {
        /****** Attributi ******/
        // Numero massimo di scheletri gestibili contemporaneamente
        internal const ulong n_max_skeleton = 6;

        // Lista di GestureDetector, una per ogni scheletro rilevato.
        private static List<GestureDetector> gestureDetectorList = null;

        /**************** Metodi ********************/
        public static void startGesture(AcquisitionManager am, KinectSensor kinectSensor)
        {
            // Inizializzo la lista di GestureDetector
            gestureDetectorList = new List<GestureDetector>();

            // Iniziliazza l'array di skeleton e GestureDetector
            for (int i = 0; i < (int)n_max_skeleton; i++)
            {
                GestureDetector detector = new GestureDetector(kinectSensor);
                gestureDetectorList.Add(detector);
            }

            // Associo l'handler updateStream all'evento frameManaged
            am.frameManaged += updateGesture;
        }

        public static void updateGesture(AcquisitionManager sender)
        {
            /// Rilevamento Gesture
            /// Se TrackingID del body cambia, aggiorno la gesture detector corrispondente col nuovo valore.
            int i = 0;
            foreach (Body body in sender.bodyList)
            {
                ulong trackingId = body.TrackingId;
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