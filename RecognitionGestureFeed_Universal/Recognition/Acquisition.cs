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
// Other Library
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// RGF
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Stream;
using RecognitionGestureFeed_Universal.GestureManager;

namespace RecognitionGestureFeed_Universal.Recognition
{
    //Indentifico che tipologia di frame sto ricevendo
    public enum DisplayFrameType
    {
        Depth,
        Body,
        Infrared,
        Color
    }

    public class Acquisition
    {
        /**************** Attributi ********************/
        /// <summary>
        /// Defition of Bones
        /// </summary>
        // Numero massimo di scheletri gestibili contemporaneamente
        static private ulong n_max_skeleton = 6; 
        //private IList<Body> bodyList; // Lista di Body
        private Body[] bodyList = null;
        // Array che contiene gli n_max_skeleton rilevati dalla kinect 
        private Skeleton[] skeletonList = new Skeleton[n_max_skeleton];
        /// <summary> 
        /// List of gesture detectors, there will be one detector created for each potential body (max of 6) 
        /// </summary>
        private List<GestureDetector> gestureDetectorList = null;

        /* ATTENZIONE LAVORI IN CORSO */
        private KinectSensor kinectSensor = null;

        /**************** Metodi ********************/
        /* Costruttore */
        public Acquisition(KinectSensor ks)
        {
            // Verifica la presenza di una kinect
            if (ks == null)
                throw new ArgumentNullException("KinectSensor non è inizializzato.");
            else
                this.kinectSensor = ks;

            this.startAcquisition();
        }

        /* Acquisizione */
        private void startAcquisition()
        {
            // Inizializzo la lista di GestureDetector
            this.gestureDetectorList = new List<GestureDetector>();
            // Iniziliazza l'array di skeleton e GestureDetector
            for (int i = 0; i < (int)n_max_skeleton; i++)
            {
                skeletonList[i] = new Skeleton();
                GestureDetector detector = new GestureDetector(this.kinectSensor);
                this.gestureDetectorList.Add(detector);
            }

            // Attivo il lettore di multiframe
            MultiSourceFrameReader multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
            // e vi associo il relativo handler
            multiSourceFrameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs evento)
        {
            // Acquisisco il frame arrivato in input
            MultiSourceFrame multiSourceFrame = evento.FrameReference.AcquireFrame();
            // Se il frame è inizializzato provvedo a gestirlo, altrimenti faccio un return
            if (multiSourceFrame == null)
            {
                return;
            }

            //Nel caso in cui stiamo leggendo un Body frame
            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                /// Se il frame non è vuoto, allora prelevo da esso
                /// gli eventuali body rilevati.
                if (bodyFrame != null)
                {
                    // Creo tanti elementi in bodyList quanti sono i body presenti nel frame
                    if (this.bodyList == null)
                        this.bodyList = new Body[bodyFrame.BodyFrameSource.BodyCount];

                    // Aggiorno la lista con i nuovi elementi
                    bodyFrame.GetAndRefreshBodyData(bodyList);
                    // Indice per accedere alla lista di scheletri
                    int index = 0;
                    // Aggiorno lo scheletro associato ad ogni body
                    foreach (Body body in this.bodyList)
                    {
                        //
                        ulong trackingId = body.TrackingId;
                        /// Se il corpo in oggetto è effettivamente presente, 
                        /// allora aggiorno l'oggetto di tipo Skeleton 
                        /// (che contiene tutte le informazioni sul corpo rilevato)
                        /// e provvedo ad aggiornare l'ImageSource ad esso associato
                        if (body.IsTracked)
                        {
                            // Aggiorno lo scheletro
                            skeletonList[index].updateSkeleton(body);
                        }
                        else
                        {
                            // Se lo scheletro non è più visibile
                            if (skeletonList[index].getStatus())
                                skeletonList[index].updateSkeleton();
                        }

                        index++;// Aggiorno l'indice
                    }
                }
            }
        }
    }
}
