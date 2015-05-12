using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;
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
// Thread
using System.ComponentModel;


namespace RecognitionGestureFeed_Universal.Recognition
{
    public class StreamAcquisition
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

        /// <summary>
        /// Rispettivamente, depthFrameData è l'array che indica per ogni pixel il livello di profondità rilevato;
        /// infraredFrameData è l'array che indica per ogni pixel il livello di infrarossi rilevato dalla kinect;
        /// </summary>
        private ushort[] depthFrameData = null;
        private ushort[] infraredFrameData = null;
        // Immagine che contiene il frame da stampare (depth, infrared, color e skeleton)
        private ImageSource depthImage = null;
        private ImageSource infraredImage = null;
        private ImageSource colorImage = null;
        private ImageSource skeletonImage = null;
        // Altezza e Larghezza delle immagini
        private double width = 0;
        private double height = 0;

        /* ATTENZIONE LAVORI IN CORSO */
        private KinectSensor kinectSensor = null;

        /****** Funzioni ******/
        public StreamAcquisition(KinectSensor ks, double imageWidth, double imageHeight)
        {
            // Verifica la presenza di una kinect
            if (ks == null)
                throw new ArgumentNullException("KinectSensor non è inizializzato.");
            else
                this.kinectSensor = ks;
            // Inizializzo le variabili di imageWidth e imageHeight */
            this.width = imageWidth;
            this.height = imageHeight;

            // Faccio partire il thread che si occupa di inizializzare gli elementi che prelevano
            this.startStreamAcquisition();
        }

        private void startStreamAcquisition()
        {
            // Inizializzo la lista di scheletri
            for (int i = 0; i < (int)n_max_skeleton; i++)
                skeletonList[i] = new Skeleton();

            /* Inizializzazione Array frameData e ImageSource */
            // Inizializza l'array depthFrameData
            FrameDescription depthFrameDescription = kinectSensor.DepthFrameSource.FrameDescription;
            this.depthFrameData = new ushort[depthFrameDescription.Width * depthFrameDescription.Height];
            // Inizializza l'array InfraredFrameData
            FrameDescription infraredFrameDescription = kinectSensor.InfraredFrameSource.FrameDescription;
            this.infraredFrameData = new ushort[infraredFrameDescription.Width * infraredFrameDescription.Height];
            // Attivo il lettore di multiframe
            MultiSourceFrameReader multiSourceFrameReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Color | FrameSourceTypes.Body);
            // e vi associo il relativo handler
            multiSourceFrameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            // Prendo dalla variabile kinectSensor, 
            coordinateMapper = kinectSensor.CoordinateMapper;
        }

        #region handlers
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs evento)
        {
            // Acquisisco il frame arrivato in input
            MultiSourceFrame multiSourceFrame = evento.FrameReference.AcquireFrame();

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
                    // Se il depthFrame non è vuoto, allora copio il suo contenuto nell'array depthFrameData
                    depthFrame.CopyFrameDataToArray(depthFrameData);
                    /* Esiste un metodo più veloce per processare gli infrared frame, ed è quella
                     di accedere direttamente ad un underlying buffer, lo usiamo? */
                    // Quindi aggiorno l'ImageSource depthImage
                    //depthImage = StreamManager.convertBitmap(depthFrame, depthFrameData);
                }
            }
            // Nel caso in cui stiamo leggendo un Infrared frame
            using (InfraredFrame infraredFrame = multiSourceFrame.InfraredFrameReference.AcquireFrame())
            {
                // Controllo se l'infraredFrame è nullo
                if (infraredFrame != null)
                {
                    // Se l'infraredFrame non è vuoto, allora copio il suo contenuto nell'array infraredFrameData
                    infraredFrame.CopyFrameDataToArray(infraredFrameData);
                    /* Esiste un metodo più veloce per processare gli infrared frame, ed è quella
                     di accedere direttamente ad un underlying buffer, lo usiamo? */
                    // Quindi aggiorno l'ImageSource infraredImage
                    //infraredImage = StreamManager.convertBitmap(infraredFrame, infraredFrameData);
                }
                    
            }
            // Nel caso in cui stiamo leggendo un Color frame
            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                // Controllo se il colorFrame è nullo
                if (colorFrame != null)
                {
                    // Quindi aggiorno l'ImageSource colorImage
                    //colorImage = StreamManager.convertBitmap(colorFrame);
                }
            }
            // Nel caso in cui stiamo leggendo un Body frame
            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {

                    // Creo tanti elementi in bodyList quanti sono i body presenti nel frame
                    if (this.bodyList == null)
                        this.bodyList = new Body[bodyFrame.BodyFrameSource.BodyCount];
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
                    skeletonImage = StreamManager.drawSkeletons(skeletonList, width, height, coordinateMapper);                     
                }
            }
        }

        #endregion

    }
}
