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
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Stream;
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;
using RecognitionGestureFeed_Universal.GestureManager;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Recognition
{
    // Indentifico la tipologia di dati che voglio stampare.
    public enum DisplayFrameType
    {
        Body,
        BodyIndex,
        Color,
        Infrared,
        Depth
    }

    /// <summary>
    /// Delegate per l'evento di tipo FrameManaged.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void FrameManaged(AcquisitionManager sender);

    public class AcquisitionManager
    {
        /**** Eventi ****/
        // Evento che indica quando un frame è stato gestito
        public event FrameManaged FrameManaged;
        public event FrameManaged BodyFrameManaged;
        public event FrameManaged DepthFrameManaged;
        public event FrameManaged InfraredFrameManaged;
        public event FrameManaged ColorFrameManaged;
        public event FrameManaged SkeletonFrameManaged;

        /****** Attributi ******/
        // Variabile usata per la comunicazione con la kinect
        private KinectSensor kinectSensor = null;
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

        SensorInterface s = null;
        /****** Costruttore ******/
        public AcquisitionManager(KinectSensor ks)
        {
            // Associo a kinectSensor la variabile KinectSensor in input
            this.kinectSensor = ks;

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
            // Attivo il lettore di multiframe
            MultiSourceFrameReader multiSourceFrameReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.LongExposureInfrared | FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
            // e vi associo il relativo handler
            multiSourceFrameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            s = new SensorInterface(this);
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
                    this.OnColorFrameManaged(this);
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
                    this.OnDepthFrameManaged(this);
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
                    this.OnInfraredFrameManaged(this);
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
                        if(bodyList[index].IsTracked)
                            skeletonList[index].updateSkeleton(bodyList[index], bodyFrame.RelativeTime);
                        else
                            skeletonList[index].updateSkeleton();
                    }
                    /*foreach (Body body in this.bodyList)
                    {
                        /// Se il corpo in oggetto è effettivamente presente, 
                        /// allora aggiorno l'oggetto di tipo Skeleton 
                        /// (che contiene tutte le informazioni sul corpo rilevato)
                        if (body.IsTracked)
                            skeletonList[index].updateSkeleton(body, bodyFrame.RelativeTime);
                        else
                            skeletonList[index].updateSkeleton();
                        /// Aggiorno l'indice
                        index++;
                    }      */              
                }
                this.OnSkeletonFrameManaged(this);
            }
            //
            using (BodyIndexFrame bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
            {
                if(bodyIndexFrame != null)
                {
                    bodyIndexData.update(bodyIndexFrame);
                    this.OnBodyFrameManaged(this);
                }
            }
            

            // Prova Aggiunta GestureXML
            //List<JointType> patagherru = new List<JointType>();
            //patagherru.Add(JointType.Head);
            //patagherru.Add(JointType.Neck);
            //AddNewGestureXML cacca = new AddNewGestureXML("ie!", patagherru, skeletonList);
            

            //prova stampa da XML
            //GestureDetectorXML filemanager = new GestureDetectorXML();
            //filemanager.printXML();

            // Richiamo l'evento
            this.OnFrameManaged(this);
            s.updateSkeleton(this);
        }

        #region Events
        /// <summary>
        /// Evento che avvisa la gestione di un Frame prelevato dalla kinect.
        /// </summary>
        /// <param name="sender">Passa in input l'oggetto di tipo AcquisitionManager, che contiene tutte le informazioni necessarie per la stampa.</param>
        protected virtual void OnFrameManaged(AcquisitionManager sender)
        {
            FrameManaged handler = FrameManaged;
            if (handler != null)
                handler(sender);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un BodyFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnBodyFrameManaged(AcquisitionManager sender)
        {
            FrameManaged handler = DepthFrameManaged;
            if (handler != null)
                handler(sender);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un DephtFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnDepthFrameManaged(AcquisitionManager sender)
        {
            FrameManaged handler = BodyFrameManaged;
            if (handler != null)
                handler(sender);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un InfraredFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnInfraredFrameManaged(AcquisitionManager sender)
        {
            FrameManaged handler = InfraredFrameManaged;
            if (handler != null)
                handler(sender);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un ColorFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnColorFrameManaged(AcquisitionManager sender)
        {
            FrameManaged handler = ColorFrameManaged;
            if (handler != null)
                handler(sender);
        }
        /// <summary>
        /// Evento che avvisa la gestione di un SkeletonFrame.
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnSkeletonFrameManaged(AcquisitionManager sender)
        {
            FrameManaged handler = SkeletonFrameManaged;
            if (handler != null)
                handler(sender);
        }
        #endregion
    }
}
