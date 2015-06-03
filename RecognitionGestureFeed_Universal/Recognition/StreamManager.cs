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
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.Stream;
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;
// Thread
using System.ComponentModel;


namespace RecognitionGestureFeed_Universal.Recognition
{
    public static class StreamManager
    {
        /**** Attributi ****/
        // Immagine che contiene il frame da stampare (depth, infrared, color e skeleton)
        private static WriteableBitmap bodyIndexBitmap = null;
        private static WriteableBitmap depthBitmap = null;
        private static WriteableBitmap infraredBitmap = null;
        private static WriteableBitmap colorBitmap = null;
        private static WriteableBitmap skeletonBitmap = null;
        // Mapper che viene utilizzato per passare da una rappresentazione all'altra.
        private static CoordinateMapper coordinateMapper;

        /**** Metodi ****/
        /// <summary>
        /// Inizializzo le WritableBitmap e associo all'evento frameManaged il suo handler.
        /// </summary>
        /// <param name="am"></param>
        /// <param name="kinectSensor"></param>
        public static void startAllStream(AcquisitionManager am, KinectSensor kinectSensor)
        {
            /* Inizializzazione WritableBitmap */
            initBodyIndexStream(am);
            initDepthStream(am);
            initInfraredStream(am);
            initColorStream(am);
            initSkeletonStream(kinectSensor);

            // Associo l'handler updateStream all'evento frameManaged
            am.frameManaged += updateAllStream;
        }

        /// <summary>
        /// Aggiorno le WritableBitmap in base ai nuovi valori acquisiti dalla kinect.
        /// </summary>
        /// <param name="sender"></param>
        private static void updateAllStream(AcquisitionManager sender)
        {
            bodyIndexBitmap.convertBitmap(sender.bodyIndexData);
 	        depthBitmap.convertBitmap(sender.depthData);
            infraredBitmap.convertBitmap(sender.infraredData);
            colorBitmap.convertBitmap(sender.colorData);
            skeletonBitmap.drawSkeletons(sender.skeletonList, coordinateMapper);
        }
        #region Bitmap Stream
        /****** Metodi ******/
        /// <summary>
        /// Funzione che restituisce il WritableBitmap associato ad ogni tipo di frame
        /// </summary>
        /// <param name="displayFrameType"></param>
        /// <returns></returns>
        public static ImageSource updateStream(DisplayFrameType displayFrameType)
        {
            switch (displayFrameType)
            {
                case DisplayFrameType.Depth:
                    return depthBitmap;
                case DisplayFrameType.Infrared:
                    return infraredBitmap;
                case DisplayFrameType.Color:
                    return colorBitmap;
                case DisplayFrameType.Body:
                    //return this.skeletonImage as BitmapImage;
                    return skeletonBitmap;
                case DisplayFrameType.BodyIndex:
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
        public static bool isStreamReady(DisplayFrameType displayFrameType)
        {
            bool valReturn = false;

            switch (displayFrameType)
            {
                case DisplayFrameType.Depth:
                    if (depthBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.Infrared:
                    if (infraredBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.Color:
                    if (colorBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.Body:
                    if (skeletonBitmap != null)
                        valReturn = true;
                    break;
                case DisplayFrameType.BodyIndex:
                    if (bodyIndexBitmap != null)
                        valReturn = true;
                    break;
            }
            return valReturn;
        }
        #endregion

        #region Init Stream
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
        private static void initSkeletonStream(KinectSensor kinectSensor)
        {
            skeletonBitmap = new WriteableBitmap(800, 600, 96.0, 96.0, PixelFormats.Bgr32, null);
            // Prendo dalla variabile kinectSensor, il CoordinateMapper che verrà usata per la stampa degli scheletri
            coordinateMapper = kinectSensor.CoordinateMapper;
        }
        #endregion Init Stream

        /************************************************** In Prova *********************************/
        public static void startBodyIndexStream(AcquisitionManager am)
        {
            initBodyIndexStream(am);
            am.bodyFrameManaged += updateBodyIndexStream;
        }
        public static void startDepthStream(AcquisitionManager am)
        {
            initDepthStream(am);
            am.frameManaged += updateDepthStream;
        }
        public static void startInfraredStream(AcquisitionManager am)
        {
            initInfraredStream(am);
            am.frameManaged += updateInfraredStream;
        }
        public static void startColorStream(AcquisitionManager am)
        {
            initColorStream(am);
            am.frameManaged += updateColorStream;
        }
        public static void startSkeletonStream(AcquisitionManager am, KinectSensor kinectSensor)
        {
            initSkeletonStream(kinectSensor);
            am.frameManaged += updateSkeletonStream;
        }

        private static void updateBodyIndexStream(AcquisitionManager sender)
        {
            bodyIndexBitmap.convertBitmap(sender.bodyIndexData);
        }
        private static void updateDepthStream(AcquisitionManager sender)
        {
            depthBitmap.convertBitmap(sender.depthData);
        }
        private static void updateInfraredStream(AcquisitionManager sender)
        {
            infraredBitmap.convertBitmap(sender.infraredData);
        }
        private static void updateColorStream(AcquisitionManager sender)
        {
            colorBitmap.convertBitmap(sender.colorData);
        }
        private static void updateSkeletonStream(AcquisitionManager sender)
        {
            skeletonBitmap.drawSkeletons(sender.skeletonList, coordinateMapper);
        }
    }
}
