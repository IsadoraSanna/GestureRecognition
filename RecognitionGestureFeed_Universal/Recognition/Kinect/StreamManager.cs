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
// Writable
using System.Windows.Media;
using System.Windows.Media.Imaging;
// RGF
using RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Kinect.Stream;
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;


namespace RecognitionGestureFeed_Universal.Recognition.Kinect
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    public delegate void BitmapUpdate(ImageSource bitmap);

    public static class StreamManager
    {
        /**** Eventi ****/
        public static event BitmapUpdate BodyIndexBitmapUpdate;
        public static event BitmapUpdate DepthBitmapUpdate;
        public static event BitmapUpdate InfraredBitmapUpdate;
        public static event BitmapUpdate ColorBitmapUpdate;
        public static event BitmapUpdate SkeletonBitmapUpdate;

        /**** Attributi ****/
        // Immagine che contiene il frame da stampare (depth, infrared, color e skeleton)
        private static WriteableBitmap bodyIndexBitmap = null;
        private static WriteableBitmap depthBitmap = null;
        private static WriteableBitmap infraredBitmap = null;
        private static WriteableBitmap colorBitmap = null;
        private static ImageSource skeletonBitmap = null;
        // Mapper che viene utilizzato per passare da una rappresentazione all'altra.
        private static CoordinateMapper coordinateMapper;
        private static DrawingGroup drawingGroup = new DrawingGroup();

        /**** Handler ****/
        /// <summary>
        /// Gestisce la sola stampa del BodyIndexStream.
        /// </summary>
        /// <param name="am"></param>
        public static void startBodyIndexStream(AcquisitionManager am)
        {
            initBodyIndexStream(am);
            am.BodyFrameManaged += updateBodyIndexStream;
        }

        /// <summary>
        /// Gestisce la sola stampa del DepthStream.
        /// </summary>
        /// <param name="am"></param>
        public static void startDepthStream(AcquisitionManager am)
        {
            initDepthStream(am);
            am.DepthFrameManaged += updateDepthStream;
        }
        /// <summary>
        /// Gestisce la sola stampa del InfraredStream.
        /// </summary>
        /// <param name="am"></param>
        public static void startInfraredStream(AcquisitionManager am)
        {
            initInfraredStream(am);
            am.InfraredFrameManaged += updateInfraredStream;
        }
        /// <summary>
        /// Gestisce la sola stampa del ColorStream.
        /// </summary>
        /// <param name="am"></param>
        public static void startColorStream(AcquisitionManager am)
        {
            initColorStream(am);
            am.ColorFrameManaged += updateColorStream;
        }
        /// <summary>
        /// Gestisce la sola stampa del SkeletonStream.
        /// </summary>
        /// <param name="am"></param>
        /// <param name="kinectSensor"></param>
        public static void startSkeletonStream(AcquisitionManager am)
        {
            initSkeletonStream(am, am.kinectSensor);
            am.SkeletonsFrameManaged += updateSkeletonStream;
        }
        /// <summary>
        /// Gestisce la stampa di tutte le WritableBitmap e associo all'evento frameManaged il suo handler.
        /// </summary>
        /// <param name="am"></param>
        /// <param name="kinectSensor"></param>
        public static void startAllStream(AcquisitionManager am)
        {
            /* Inizializzazione WritableBitmap */
            initBodyIndexStream(am);
            initDepthStream(am);
            initInfraredStream(am);
            initColorStream(am);
            initSkeletonStream(am, am.kinectSensor);
            
            // Associo l'handler updateStream all'evento frameManaged
            am.FramesManaged += updateAllStream;
        }

        /// <summary>
        /// Aggiorna solo la stampa del BodyIndexStream
        /// </summary>
        /// <param name="sender"></param>
        private static void updateBodyIndexStream(FrameData sender)
        {
            bodyIndexBitmap.convertBitmap((BodyIndexData)sender);
            _OnBodyIndexStream();
        }

        /// <summary>
        /// Aggiorna solo la stampa del DepthStream
        /// </summary>
        /// <param name="sender"></param>
        private static void updateDepthStream(FrameData sender)
        {
            depthBitmap.convertBitmap((DepthData)sender);
            _OnDepthStream();
        }
        /// <summary>
        /// Aggiorna solo la stampa dell'InfraredStream
        /// </summary>
        /// <param name="sender"></param>
        private static void updateInfraredStream(FrameData sender)
        {
            infraredBitmap.convertBitmap((InfraredData)sender);
            _OnInfraredStream();
        }
        /// <summary>
        /// Aggiorna solo la stampa del ColorStream
        /// </summary>
        /// <param name="sender"></param>
        private static void updateColorStream(FrameData sender)
        {
            colorBitmap.convertBitmap((ColorData)sender);
            _OnColorStream();
        }
        /// <summary>
        /// Aggiorna solo la stampa dello SkeletonStream
        /// </summary>
        /// <param name="sender"></param>
        private static void updateSkeletonStream(Skeleton[] skeletonList)
        {
            skeletonBitmap.drawSkeletons(skeletonList, drawingGroup, coordinateMapper);
            _OnSkeletonStream();
        }
        /// <summary>
        /// Aggiorno le WritableBitmap in base ai nuovi valori acquisiti dalla kinect.
        /// </summary>
        /// <param name="sender"></param>
        private static void updateAllStream(BodyIndexData bodyData, DepthData depthData, InfraredData infraredData, ColorData colorData, Skeleton[] skeletons)
        {
            updateBodyIndexStream(bodyData);
            _OnBodyIndexStream();
            updateDepthStream(depthData);
            _OnDepthStream();
            updateInfraredStream(infraredData);
            _OnInfraredStream();
            updateColorStream(colorData);
            _OnColorStream();
            updateSkeletonStream(skeletons);
            _OnSkeletonStream();
        }

        #region Eventi BitmapUpdate
        /// <summary>
        /// Quando viene aggiornato il BodyIndexBitmap
        /// </summary>
        private static void _OnBodyIndexStream()
        {
            if (BodyIndexBitmapUpdate != null)
                BodyIndexBitmapUpdate(bodyIndexBitmap);
        }
        /// <summary>
        /// Quando viene aggiornato il DepthBitmap
        /// </summary>
        private static void _OnDepthStream()
        {
            if (DepthBitmapUpdate != null)
                DepthBitmapUpdate(depthBitmap);
        }
        /// <summary>
        /// Quando viene aggiornato l'InfraredBitmap
        /// </summary>
        private static void _OnInfraredStream()
        {
            if (InfraredBitmapUpdate != null)
                InfraredBitmapUpdate(infraredBitmap);
        }
        /// <summary>
        /// Quando viene aggiornato il ColorBitmap
        /// </summary>
        private static void _OnColorStream()
        {
            if (ColorBitmapUpdate != null)
                ColorBitmapUpdate(colorBitmap);
        }
        /// <summary>
        /// Quando viene aggiornato lo SkeletonBitmap
        /// </summary>
        private static void _OnSkeletonStream()
        {
            if (SkeletonBitmapUpdate != null)
                SkeletonBitmapUpdate(skeletonBitmap);
        }
        #endregion

        #region Bitmap Stream
        /// <summary>
        /// Funzione che restituisce il WritableBitmap associato ad ogni tipo di frame
        /// </summary>
        /// <param name="frameType"></param>
        /// <returns></returns>
        public static ImageSource updateStream(FrameSourceTypes frameType)
        {
            switch (frameType)
            {
                case FrameSourceTypes.Depth:
                    return depthBitmap;
                case FrameSourceTypes.Infrared:
                    return infraredBitmap;
                case FrameSourceTypes.Color:
                    return colorBitmap;
                case FrameSourceTypes.Body:
                    return skeletonBitmap;
                case FrameSourceTypes.BodyIndex:
                    return bodyIndexBitmap;
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
        public static bool isStreamReady(FrameSourceTypes frameType)
        {
            bool valReturn = false;

            switch (frameType)
            {
                case FrameSourceTypes.Depth:
                    if (depthBitmap != null)
                        valReturn = true;
                    break;
                case FrameSourceTypes.Infrared:
                    if (infraredBitmap != null)
                        valReturn = true;
                    break;
                case FrameSourceTypes.Color:
                    if (colorBitmap != null)
                        valReturn = true;
                    break;
                case FrameSourceTypes.Body:
                    if (skeletonBitmap != null)
                        valReturn = true;
                    break;
                case FrameSourceTypes.BodyIndex:
                    if (bodyIndexBitmap != null)
                        valReturn = true;
                    break;
            }
            return valReturn;
        }
        #endregion

        #region Init Stream
        /// <summary>
        /// Inizializza bodyIndexBitmap
        /// </summary>
        /// <param name="am"></param>
        private static void initBodyIndexStream(AcquisitionManager am)
        {
            bodyIndexBitmap = new WriteableBitmap(am.bodyIndexData.width, am.depthData.height, 96.0, 96.0, PixelFormats.Bgr32, null);
        }
        /// <summary>
        /// Inizializza depthBitmap
        /// </summary>
        /// <param name="am"></param>
        /// <param name="kinectSensor"></param>
        private static void initDepthStream(AcquisitionManager am)
        {
            depthBitmap = new WriteableBitmap(am.depthData.width, am.depthData.height, 96.0, 96.0, PixelFormats.Gray8, null);
        }
        /// <summary>
        /// Inizializza infraredBitmap
        /// </summary>
        /// <param name="am"></param>
        /// <param name="kinectSensor"></param>
        private static void initInfraredStream(AcquisitionManager am)
        {
            infraredBitmap = new WriteableBitmap(am.infraredData.width, am.infraredData.height, 96.0, 96.0, PixelFormats.Bgr32, null);
        }
        /// <summary>
        /// Inizializza colorBitmap
        /// </summary>
        /// <param name="am"></param>
        /// <param name="kinectSensor"></param>
        private static void initColorStream(AcquisitionManager am)
        {
            colorBitmap = new WriteableBitmap(am.colorData.width, am.colorData.height, 96.0, 96.0, PixelFormats.Bgr32, null);
        }
        /// <summary>
        /// Inizializza skeletonBitmap
        /// </summary>
        private static void initSkeletonStream(AcquisitionManager am, KinectSensor kinectSensor)
        {
            skeletonBitmap = new DrawingImage(drawingGroup);
            // Prendo dalla variabile kinectSensor, il CoordinateMapper che verrà usata per la stampa degli scheletri
            coordinateMapper = kinectSensor.CoordinateMapper;
        }
        #endregion Init Stream


    }
}
