using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
// Kinect
using Microsoft.Kinect;
// RecognitionGestureFeed
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Stream;
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;
using RecognitionGestureFeed_Universal.GestureManager;
// Debug
using System.Diagnostics;

namespace RecognitionGestureFeed_Universal.Recognition
{
    public class AcquisitionManager
    {
        /****** Attributi ******/
        // Mapper che viene utilizzato per passare da una rappresentazione all'altra.
        private CoordinateMapper coordinateMapper;
        // Numero massimo di scheletri gestibili contemporaneamente
        static private ulong n_max_skeleton = 10;
        //private IList<Body> bodyList; // Lista di Body
        private Body[] bodyList = null;
        // Array che contiene gli n_max_skeleton rilevati dalla kinect 
        private Skeleton[] skeletonList = new Skeleton[n_max_skeleton];
        // List of gesture detectors, there will be one detector created for each potential body (max of 6) 
        private List<GestureDetector> gestureDetectorList = null;

        /// <summary>
        /// Rispettivamente, depthFrameData è l'array che indica per ogni pixel il livello di profondità rilevato;
        /// infraredFrameData è l'array che indica per ogni pixel il livello di infrarossi rilevato dalla kinect;
        /// </summary>
        private DepthData depthData = null;
        private InfraredData infraredData = null;
        private ColorData colorData = null;
        // Immagine che contiene il frame da stampare (depth, infrared, color e skeleton)
        private WriteableBitmap depthBitmap = null;
        private WriteableBitmap infraredBitmap = null;
        private WriteableBitmap colorBitmap = null;
        private WriteableBitmap skeletonBitmap = null;
        private ImageSource skeletonImage = null;
        // Altezza e Larghezza delle immagini
        private int width = 600;
        private int height = 800;

        //Bool per capire se l'utente preferisce stampare le immagini o le informazioni
        private bool infoRequest;
        private bool printRequest;

        // Variabile usata per la comunicazione con la kinect
        private KinectSensor kinectSensor = null;

        public AcquisitionManager(KinectSensor ks, bool infoR, bool printR)
        {
            kinectSensor = ks;
            infoRequest = infoR;
            printRequest = printR;

            // Inizializzo la lista di GestureDetector
            this.gestureDetectorList = new List<GestureDetector>();
            // Iniziliazza l'array di skeleton e GestureDetector
            for (int i = 0; i < (int)n_max_skeleton; i++)
            {
                skeletonList[i] = new Skeleton();
                GestureDetector detector = new GestureDetector(this.kinectSensor);
                this.gestureDetectorList.Add(detector);
            }
            // Inizializzo la WritableBitmap usata per stampare gli scheletri
            this.skeletonBitmap = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgr32, null);
            // Creo tanti elementi in bodyList quanti sono i body presenti nel frame
            if (this.bodyList == null)
                this.bodyList = new Body[kinectSensor.BodyFrameSource.BodyCount];

            /* Inizializzazione Array frameData e ImageSource */
            // Inizializza l'oggetto depthFrameData
            FrameDescription depthFrameDescription = kinectSensor.DepthFrameSource.FrameDescription;
            this.depthData = new DepthData(depthFrameDescription);
            this.depthBitmap = new WriteableBitmap(depthData.width, depthData.height, 96.0, 96.0, PixelFormats.Gray8, null);
            // Inizializza l'oggetto InfraredFrameData
            FrameDescription infraredFrameDescription = kinectSensor.InfraredFrameSource.FrameDescription;
            this.infraredData = new InfraredData(infraredFrameDescription);
            this.infraredBitmap = new WriteableBitmap(infraredData.width, infraredData.height, 96.0, 96.0, PixelFormats.Bgr32, null);
            // Inizializza l'oggetto ColorFrameData
            FrameDescription colorFrameDescription = kinectSensor.ColorFrameSource.FrameDescription;
            this.colorData = new ColorData(colorFrameDescription);
            colorBitmap = new WriteableBitmap(colorData.width, colorData.height, 96.0, 96.0, PixelFormats.Bgr32, null);
       
            // Attivo il lettore di multiframe
            MultiSourceFrameReader multiSourceFrameReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Color | FrameSourceTypes.Body);
            // e vi associo il relativo handler
            multiSourceFrameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            // Prendo dalla variabile kinectSensor, 
            coordinateMapper = kinectSensor.CoordinateMapper;
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

            // Nel caso in cui stiamo leggendo un Depth frame
            using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
            {
                // Controllo se il depthFrame è nullo
                if (depthFrame != null)
                {
                    // Se il depthFrame non è vuoto, allora aggiorno il contenuto dell'oggetto depthData
                    depthData.update(depthFrame);
                }
            }
            // Nel caso in cui stiamo leggendo un Infrared frame
            using (InfraredFrame infraredFrame = multiSourceFrame.InfraredFrameReference.AcquireFrame())
            {
                // Controllo se l'infraredFrame è nullo
                if (infraredFrame != null)
                {
                    // Se l'infraredFrame non è vuoto, allora aggiorno il contenuto dell'oggetto infraredData
                    infraredData.update(infraredFrame);
                }   
            }
            // Nel caso in cui stiamo leggendo un Color frame
            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                // Controllo se l'infraredFrame è nullo
                if (colorFrame != null)
                {
                    // Se l'infraredFrame non è vuoto, allora aggiorno il contenuto dell'oggetto infraredData
                    colorData.update(colorFrame);
                }
            }
            // Nel caso in cui stiamo leggendo un Body frame
            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    // Aggiorno la lista con i nuovi elementi
                    bodyFrame.GetAndRefreshBodyData(bodyList);
                    /// Indice per accedere ai vari elementi dell'array di scheletri
                    int index = 0;
                    // Aggiorno lo scheletro associato ad ogni body
                    foreach (Body body in this.bodyList)
                    {
                        /// Se il corpo in oggetto è effettivamente presente, 
                        /// allora aggiorno l'oggetto di tipo Skeleton 
                        /// (che contiene tutte le informazioni sul corpo rilevato)
                        if (body.IsTracked)
                            skeletonList[index].updateSkeleton(body);
                        else
                            skeletonList[index].updateSkeleton();
                        /// Aggiorno l'indice
                        index++;
                    }                    
                }
            }

            /*************************** Stampa *************************************/
            if (printRequest)
            {
                //this.depthBitmap = StreamManager.convertBitmap(this.depthData);
                this.depthBitmap.convertBitmap(this.depthData);
                this.infraredBitmap.convertBitmap(this.infraredData);
                this.colorBitmap.convertBitmap(this.colorData);
                this.skeletonBitmap.drawSkeletons(this.skeletonList, this.coordinateMapper);
                //this.skeletonImage = StreamManager.drawSkeletons(this.skeletonList, 400, 500, this.coordinateMapper);
            }
            /************************** Gesture ************************************/
            if (infoRequest)
            {
                /// Rilevamento Gesture
                /// Se TrackingID del body cambia, aggiorno la gesture detector corrispondente col nuovo valore.
                int i = 0;
                foreach (Body body in this.bodyList)
                {
                    ulong trackingId = body.TrackingId;
                    if (trackingId != this.gestureDetectorList[i].TrackingId)
                    {
                        this.gestureDetectorList[i].TrackingId = trackingId;
                        // Se il body è tracciato, il suo detector esce dalla pausa per catturare gli eventi VisualGestureBuilderFrameArrived.
                        // Altrimenti il suo detector rimane in pausa e non sprechiamo risorse cercando di gestire gesture invalide.
                        this.gestureDetectorList[i].IsPaused = trackingId == 0;
                    }
                    i++;// aggiorno l'indice
                }
            }

            // Prova Aggiunta GestureXML
            /*List<JointType> patagherru = new List<JointType>();
            patagherru.Add(JointType.Head);
            patagherru.Add(JointType.Neck);
            AddNewGesture cacca = new AddNewGesture("ie!", patagherru, skeletonList);*/
        }

        #region Bitmap Stream
        /// <summary>
        /// Funzione che restituisce il WritableBitmap associato ad ogni tipo di frame
        /// </summary>
        /// <param name="displayFrameType"></param>
        /// <returns></returns>
        public ImageSource updateStream(DisplayFrameType displayFrameType)
        {
            switch (displayFrameType)
            {
                case DisplayFrameType.Depth:
                    return this.depthBitmap;
                case DisplayFrameType.Infrared:
                    return this.infraredBitmap;
                case DisplayFrameType.Color:
                    return this.colorBitmap;
                case DisplayFrameType.Body:
                    //return this.skeletonImage as BitmapImage;
                    return this.skeletonBitmap;
                default:
                    return null;
            }
        }

        /// <summary>
        /// streamIsReady, prendendo in input un valore valido di displayFrameType, restituisce
        /// true se l'ImageSource associato a quel tipo di displayFrameType è stato inizializzato. Viceversa
        /// restituisce false.
        /// </summary>
        /// <param name="displayFrameType"></param>
        /// <returns></returns>
        public bool isStreamReady(DisplayFrameType displayFrameType)
        {
            bool valReturn = false;

            switch (displayFrameType)
            {
                case DisplayFrameType.Depth:
                    if (this.depthBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.Infrared:
                    if (this.infraredBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.Color:
                    if (colorBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.Body:
                    //if (skeletonImage != null)
                    if (skeletonBitmap != null)
                        valReturn = true;
                    break;
            }
            return valReturn;
        }
        #endregion
    }
}
