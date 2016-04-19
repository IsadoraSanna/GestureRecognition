using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect Face Recognition
using Microsoft.Kinect.Face;

namespace RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure
{
    public class FaceDetector
    {
        /* Attributi */
        // Id dello scheletro
        public ulong idBody { get; private set; }
        public int idSkeleton { get; private set; }
        // Face Information
        private FaceFrameReader faceFrameReader = null;
        private FaceFrameSource faceFrameSource = null;
        public FaceFrameResult faceFrameResults { get; private set; }
        // Face HD
        private HighDefinitionFaceFrameReader highDefinitionFaceFrameReader = null;
        private HighDefinitionFaceFrameSource highDefinitionFaceFrameSource = null;
        public FaceAlignment faceAlignment { get; private set; }
        public FaceModel faceModel { get; private set; }

        /* Costruttore */
        public FaceDetector()
        {
            // Inizializzo il FaceFrameSource, specificando quali sono le espressioni che voglio riconoscere (per ora tutte)
            FaceFrameFeatures faceFrameFeatures = FaceFrameFeatures.BoundingBoxInColorSpace | FaceFrameFeatures.PointsInColorSpace | FaceFrameFeatures.RotationOrientation | FaceFrameFeatures.FaceEngagement | FaceFrameFeatures.Glasses | FaceFrameFeatures.Happy | FaceFrameFeatures.LeftEyeClosed | FaceFrameFeatures.RightEyeClosed | FaceFrameFeatures.LookingAway | FaceFrameFeatures.MouthMoved | FaceFrameFeatures.MouthOpen;
            this.faceFrameSource = new FaceFrameSource(AcquisitionManager.getInstance().kinectSensorExtend.getKinectSensor(), 0, faceFrameFeatures);
            // Inizializzo il FaceFrameReader e associo l'handler all'evento FrameArrived di FaceFrame
            this.faceFrameReader = this.faceFrameSource.OpenReader();
            this.faceFrameReader.FrameArrived += this.Reader_FaceFrameArrived;
            this.faceFrameResults = null;
            // Inizializzo l'HighDefinitionFaceFrameSource per l'acquisizione dei dati in hd del viso
            this.highDefinitionFaceFrameSource = new HighDefinitionFaceFrameSource(AcquisitionManager.getInstance().kinectSensorExtend.getKinectSensor());
            // Inizializzo l'HighDefinitionFaceFrameReader e associo l'handler all'evento FrameArrived di HighDefinitionFaceFrame
            this.highDefinitionFaceFrameReader = this.highDefinitionFaceFrameSource.OpenReader();
            this.highDefinitionFaceFrameReader.FrameArrived += this.HdFaceReader_FrameArrived;
            this.faceAlignment = new FaceAlignment();
            this.faceModel = new FaceModel();
        }

        /* Metodi */
        /// <summary>
        /// Aggiorna le informazioni relative alla faccia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            // Prelevo il frame
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                // Se il frame non è nullo
                if (faceFrame != null)
                {
                    // Memorizzo le informazioni rilevate dalla kinect e contenute in FaceFrame
                    this.faceFrameResults = faceFrame.FaceFrameResult;
                }
            }
        }

        /// <summary>
        /// Aggiorna le informazioni relative al HDFace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HdFaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            // Prelevo il frame
            using (HighDefinitionFaceFrame hdFaceFrame = e.FrameReference.AcquireFrame())
            {
                // Se il frame non è nullo
                if (hdFaceFrame != null && hdFaceFrame.IsFaceTracked)
                {
                    // Prendo dal frame i dati del face alignment, faceAlignmentQuality e FaceModel
                    hdFaceFrame.GetAndRefreshFaceAlignmentResult(this.faceAlignment);
                    this.faceModel = hdFaceFrame.FaceModel;
                }
            }
        }

        /// <summary>
        /// Aggiorna l'id dello scheletro a cui appartiene il viso da rilevare.
        /// </summary>
        /// <param name="idBody"></param>
        public void updateId(ulong idBody)
        {
            this.idBody = idBody;
            this.idSkeleton = (int)idBody % 6;
            this.faceFrameSource.TrackingId = idBody;
            this.highDefinitionFaceFrameSource.TrackingId = idBody;
        }
    }
}
