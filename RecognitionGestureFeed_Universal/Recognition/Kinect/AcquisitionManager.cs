using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Writable
using System.Windows.Media;
using System.Windows.Media.Imaging;
// Kinect
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
// RecognitionGestureFeed
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Kinect.Stream;
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;
using RecognitionGestureFeed_Universal.Gesture;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Recognition.Kinect
{
    /// <summary>
    /// Delegate per gli eventi di tipo FrameManaged(tutti i tipi di frame, esclusi Audio e Body) e BodyManaged (quando vengono gestiti
    /// gli scheletri rilevati dalla kinect).
    /// </summary>
    /// <param name="sender"></param>
    public delegate void FramesManaged(BodyIndexData bodyData, DepthData depthData, InfraredData infraredData, ColorData colorData, Skeleton[] skeletons);
    public delegate void FrameManaged(FrameData sender);
    public delegate void BodyManaged(Skeleton sender);
    public delegate void BodiesManaged(Skeleton[] sender);

    public class AcquisitionManager
    {
        /**** Eventi ****/
        // Evento che indica quando un frame è stato gestito
        public event FramesManaged FramesManaged;
        public event FrameManaged BodyFrameManaged;
        public event FrameManaged DepthFrameManaged;
        public event FrameManaged InfraredFrameManaged;
        public event FrameManaged ColorFrameManaged;
        public event FrameManaged LongExpsoureFrameManaged;
        public event BodyManaged SkeletonFrameManaged;
        public event BodyManaged SkeletonLoseManaged;
        public event BodiesManaged SkeletonsFrameManaged;
        
        /****** Attributi ******/
        // Variabile usata per la comunicazione con la kinect
        internal KinectSensor kinectSensor = null;
        // Numero massimo di scheletri gestibili contemporaneamente
        internal int numSkeletons;
        //private IList<Body> bodyList; // Lista di Body
        internal Body[] bodyList = null;
        // Array che contiene gli n_max_skeleton rilevati dalla kinect 
        internal Skeleton[] skeletonList;
        // Array che contiene i colori con cui verranno rappresentati i vari scheletri su bitmap
        private Pen[] skeletonColors = {(new Pen(Brushes.Red, 6)), (new Pen(Brushes.Orange, 6)), (new Pen(Brushes.Green, 6)), (new Pen(Brushes.Blue, 6)), (new Pen(Brushes.Indigo, 6)), (new Pen(Brushes.Violet, 6))};
        /// <summary>
        /// Rispettivamente, depthFrameData è l'array che indica per ogni pixel il livello di profondità rilevato;
        /// infraredFrameData è l'array che indica per ogni pixel il livello di infrarossi rilevato dalla kinect;
        /// </summary>
        internal BodyIndexData bodyIndexData = null;
        internal ColorData colorData = null;
        internal DepthData depthData = null;
        internal InfraredData infraredData = null;
        internal LongExposureInfraredData longExposureInfraredData = null;
        // Reader utilizzato per selezionare e leggere i frame in arrivo dalla kinect
        MultiSourceFrameReader multiSourceFrameReader = null;
        // Booleano che indica se l'utente ha avviato la lettura di tutti i frame
        bool allFrames;

        /****** Costruttore ******/
        public AcquisitionManager(KinectSensor kinectSensor, FrameSourceTypes enabledFrameSourceTypes)
        {
            // Avvio il collegamento con la Kinect
            if(kinectSensor == null)
                throw new ArgumentNullException("Kinect not be connect.");
            else
                this.kinectSensor = kinectSensor;

            // Numero massimo di scheletri gestibili
            this.numSkeletons = kinectSensor.BodyFrameSource.BodyCount;
            // Iniziliazza l'array di skeleton
            this.skeletonList = new Skeleton[this.numSkeletons];
            for (int index = 0; index < this.numSkeletons; index++)
            {
                // Creo il singolo scheletro
                skeletonList[index] = new Skeleton(index, kinectSensor, skeletonColors[index]);
            }
            // Creo tanti elementi in bodyList quanti sono i body presenti nel frame
            if (this.bodyList == null)
                this.bodyList = new Body[kinectSensor.BodyFrameSource.BodyCount];

            /* Inizializzazione Array frameData e ImageSource */
            // Inizializza l'oggetto bodyIndexData
            FrameDescription bodyIndexFrameDescription = kinectSensor.BodyIndexFrameSource.FrameDescription;
            bodyIndexData = new BodyIndexData(bodyIndexFrameDescription);
            // Inizializza l'oggetto depthData
            FrameDescription depthFrameDescription = kinectSensor.DepthFrameSource.FrameDescription;
            this.depthData = new DepthData(depthFrameDescription);
            // Inizializza l'oggetto InfraredData
            FrameDescription infraredFrameDescription = kinectSensor.InfraredFrameSource.FrameDescription;
            this.infraredData = new InfraredData(infraredFrameDescription);
            // Inizializza l'oggetto ColorData
            FrameDescription colorFrameDescription = kinectSensor.ColorFrameSource.FrameDescription;
            this.colorData = new ColorData(colorFrameDescription);
            // Inizializza l'oggetto LongExposureData
            FrameDescription longExposureFrameDescription = kinectSensor.LongExposureInfraredFrameSource.FrameDescription;
            this.longExposureInfraredData = new LongExposureInfraredData(longExposureFrameDescription);
            //
            if (enabledFrameSourceTypes.Equals(FrameSourceTypes.Body | FrameSourceTypes.BodyIndex | FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.LongExposureInfrared))
                this.allFrames = true;
            // Attivo il lettore di multiframe
            this.multiSourceFrameReader = kinectSensor.OpenMultiSourceFrameReader(enabledFrameSourceTypes);
            // e vi associo il relativo handler
            this.multiSourceFrameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
 	        // Acquisisco il frame arrivato in input
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

            // Se il frame è inizializzato provvedo a gestirlo, altrimenti faccio un return
            if (multiSourceFrame == null)
            {
                return;
            }

            // Nel caso in cui venga letto un Color frame
            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                // Controllo se l'infraredFrame è nullo
                if (colorFrame != null)
                {
                    // Se l'infraredFrame non è vuoto, allora aggiorno il contenuto dell'oggetto infraredData
                    colorData.update(colorFrame);
                    this.OnColorFrameManaged();
                }
            }
            // Nel caso in cui venga letto un Depth frame
            using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
            {
                // Controllo se il depthFrame è nullo
                if (depthFrame != null)
                {
                    // Se il depthFrame non è vuoto, allora aggiorno il contenuto dell'oggetto depthData
                    depthData.update(depthFrame);
                    this.OnDepthFrameManaged();
                }
            }
            // Nel caso in cui venga letto un Infrared frame
            using (InfraredFrame infraredFrame = multiSourceFrame.InfraredFrameReference.AcquireFrame())
            {
                // Controllo se l'infraredFrame è nullo
                if (infraredFrame != null)
                {
                    // Se l'infraredFrame non è vuoto, allora aggiorno il contenuto dell'oggetto infraredData
                    infraredData.update(infraredFrame);
                    this.OnInfraredFrameManaged();
                }   
            }
            // Nel caso in cui venga letto un LongExposureInfrared frame
            using (LongExposureInfraredFrame longExposureInfraredFrame = multiSourceFrame.LongExposureInfraredFrameReference.AcquireFrame())
            {
                // 
                if(longExposureInfraredFrame != null)
                {
                    // Se il LongExposureInfraredFrame non è vuoto, allora aggiorno il contenuto dell'oggetto infraredData
                    longExposureInfraredData.update(longExposureInfraredFrame);
                    this.LongExpsoureFrameManaged(longExposureInfraredData);
                }
            }
            // Nel caso in cui stiamo leggendo un Body frame
            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    // Aggiorno la lista con i nuovi elementi
                    bodyFrame.GetAndRefreshBodyData(bodyList);
                    // Aggiorno lo scheletro associato ad ogni body
                    for (int index = 0; index < this.numSkeletons; index++)
                    {
                        if (bodyList[index].IsTracked)
                        {
                            skeletonList[index].updateSkeleton(bodyList[index], bodyFrame.RelativeTime);// Aggiorna lo scheletro
                            this.OnSkeletonFrameManaged(index);// Avvisa che lo scheletro è stato aggiornato
                        }
                        else if (skeletonList[index].status)// Se lo scheletro è stato perso
                        {
                            this.OnSkeletonLoseManaged(index);// Avvisa che lo scheletro in questione è stato perso
                            skeletonList[index].updateSkeleton();// Resetto lo scheletro
                        }
                    }  
                }
                this.OnSkeletonsFrameManaged();
            }
            //
            using (BodyIndexFrame bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
            {
                if(bodyIndexFrame != null)
                {
                    bodyIndexData.update(bodyIndexFrame);
                    this.OnBodyFrameManaged();
                }
            }

            // Richiamo l'evento FramesManaged, solo se tutti i frame sono effettivamente gestiti
            if(this.allFrames)
                this.OnFramesManaged();
        }

        /// <summary>
        /// Chiude il collegamento con la Kinect e resetta l'handler.
        /// </summary>
        public void Close()
        {
            multiSourceFrameReader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
            Init.Close(this.kinectSensor);
        }

        #region Events
        /// <summary>
        /// Evento che avvisa la gestione di un Frame prelevato dalla kinect.
        /// </summary>
        /// <param name="sender">Passa in input l'oggetto di tipo AcquisitionManager, che contiene tutte le informazioni necessarie per la stampa.</param>
        protected virtual void OnFramesManaged()
        {
            if (FramesManaged != null)
                FramesManaged(this.bodyIndexData, this.depthData, this.infraredData, this.colorData, this.skeletonList);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un BodyFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnBodyFrameManaged()
        {
            if (BodyFrameManaged != null)
                BodyFrameManaged(this.bodyIndexData);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un DephtFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnDepthFrameManaged()
        {
            if (DepthFrameManaged != null)
                DepthFrameManaged(this.depthData);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un InfraredFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnInfraredFrameManaged()
        {
            if (InfraredFrameManaged != null)
                InfraredFrameManaged(this.infraredData);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un ColorFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnColorFrameManaged()
        {
            if (ColorFrameManaged != null)
                ColorFrameManaged(this.colorData);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un LongExposureData
        /// </summary>
        protected virtual void OnLongExposure()
        {
            if (LongExpsoureFrameManaged != null)
                LongExpsoureFrameManaged(this.longExposureInfraredData);
        }
        /// <summary>
        /// Evento che avvisa la gestione di uno scheletro
        /// </summary>
        private void OnSkeletonFrameManaged(int index)
        {
            if (SkeletonFrameManaged != null)
                SkeletonFrameManaged(this.skeletonList[index]);
        }
        private void OnSkeletonLoseManaged(int index)
        {
            if (SkeletonLoseManaged != null)
                SkeletonLoseManaged(this.skeletonList[index]);
        }
        /// <summary>
        /// Evento che avvisa la gestione degli scheletri.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnSkeletonsFrameManaged()
        {
            if (SkeletonsFrameManaged != null)
                SkeletonsFrameManaged(this.skeletonList);
        }

        #endregion
    }
}
