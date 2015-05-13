using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// Debug
using System.IO;
using System.Diagnostics;
// Kinect
using Microsoft.Kinect;
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using RecognitionGestureFeed_Universal.Recognition.FrameDataManager;

namespace RecognitionGestureFeed_Universal.Recognition.Stream
{
    public static class StreamManager
    {
        /* Infrared Data */
        // The highest value that can be returned in the InfraredFrame.
        // It is cast to a float for readability in the visualization code.
        private const float InfraredSourceValueMaximum =
            (float)ushort.MaxValue;
        // Used to set the lower limit, post processing, of the
        // infrared data that we will render.
        // Increasing or decreasing this value sets a brightness
        // "wall" either closer or further away.
        private const float InfraredOutputValueMinimum = 0.01f;
        // The upper limit, post processing, of the
        // infrared data that will render.
        private const float InfraredOutputValueMaximum = 1.0f;
        // The InfraredSceneValueAverage value specifies the 
        // average infrared value of the scene. 
        // This value was selected by analyzing the average
        // pixel intensity for a given scene.
        // This could be calculated at runtime to handle different
        // IR conditions of a scene (outside vs inside).
        private const float InfraredSceneValueAverage = 0.08f;
        // The InfraredSceneStandardDeviations value specifies 
        // the number of standard deviations to apply to
        // InfraredSceneValueAverage.
        // This value was selected by analyzing data from a given scene.
        // This could be calculated at runtime to handle different
        // IR conditions of a scene (outside vs inside).
        private const float InfraredSceneStandardDeviations = 3.0f;

        /****** Attributi ******/
        // Map depth range to byte range
        private const int MapDepthToByte = 8000 / 256;

        // Constant for clamping Z values of camera space points from being negative
        private const float InferredZPositionClamp = 0.1f;

        // I Brush consentono di definire gli oggetti utilizzati per riempire le parti interne di forme grafiche 
        // quali rettangoli, ellissi, torte, poligoni e tracciati. In questo è il Brush che serve per stampare
        // le joint singole.
        private static readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        // Spessore del Joint da disegnare.
        // Distinguiamo tra tracked ed inferred.
        private const double trackedJointThickness = 3;
        private const double inferredJointThickness = 1.5;
        // Creo l'oggetto pen che verrà usato per tracciare l'osso
        private static readonly Pen penTracked = new Pen(Brushes.Red, 6);
        private static readonly Pen penNotTracked = new Pen(Brushes.LightGray, 1);

        /****** Funzioni ******/
        /// <summary>
        /// La funzione convertBitmap provvede a rappresentare in una WritableBitmap i valori di profondità
        /// contenuti nell'array frameData di DepthData (dati calcolati a loro volta a partire dall'ultimo DepthFrame ricevuto).
        /// </summary>
        /// <param name="depthData">DepthData che contiene le informazioni basilari del DepthFrame.</param>
        /// <returns></returns>
        public static void convertBitmap(this WriteableBitmap bitmap, DepthData depthData) 
        {
            // Prendo dal depthData le informazioni riguardanti la larghezza, l'altezza e l'array frameData (in cui sono contenuti i valori di profondità rilevati)
            int width = depthData.width;
            int height = depthData.height;
            ushort[] data = depthData.frameData;
            // Numero di bit per Pixel, che dipende dal formato utilizzato
            int bitsPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7)/8;
            // Array di byte che conterrà i valori di intensità per ogni pixel
            byte[] pixels = new byte[width * height * bitsPerPixel];

            // Calcolo il valore di rgb per ogni pixel che comporrà l'immagine.
            for (int index = 0; index < data.Length; ++index)
            {
                // Prelevo il valore di profondità dall'array
                ushort depth = data[index];
                // Trasformo il valore di intensità: se il valore di profondità non è compreso tra min e max, 
                // allora lo pongo a 0.
                pixels[index] = (byte)(depth >= depthData.minDepth && depth <= depthData.maxDepth ? (depth / MapDepthToByte) : 0);
            }
            // Trasferisco sul WritableBitmap il contenuto di colorData.pixels (ovvero l'immagine vere e propria)
            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width, 0);// Disegno i nuovi pixel
            bitmap.Unlock();
        }       

        /// <summary>
        /// La funzione convertBitmap provvede a rappresentare in una WritableBitmap i valori di profondità
        /// contenuti nell'array frameData di InfraredData (dati calcolati a loro volta a partire dall'ultimo DepthFrame ricevuto).
        /// </summary>
        /// <param name="infraredData">InfraredData che contiene le informazioni basilari dell'InfraredFrame.</param>
        /// <returns></returns>
        public static void convertBitmap(this WriteableBitmap bitmap, InfraredData infraredData) 
        {
            // Prendo dal depthData le informazioni riguardanti la larghezza, l'altezza e l'array frameData (in cui sono contenuti i valori di infrarosso rilevati)
            int width = infraredData.width;
            int height = infraredData.height;
            ushort[] data = infraredData.frameData;
            // Numero di bit per Pixel, che dipende dal formato deciso
            int bpp = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            // Array di byte che conterrà i valori di intensità per ogni pixel
            byte[] pixels = new byte[width * height * bpp];
            // Indice usato per accedere all'array pixel
            int index = 0;

            for (int i = 0; i < data.Length; ++i)
            {
                /* Metodo alternativo per disegnare l'intesità */
                /*// 1. dividing the incoming value by the 
                // source maximum value
                float intensityRatio = (float)data[i] / InfraredSourceValueMaximum;
                // 2. dividing by the 
                // (average scene value * standard deviations)
                intensityRatio /= InfraredSceneValueAverage * InfraredSceneStandardDeviations;
                // 3. limiting the value to InfraredOutputValueMaximum
                intensityRatio = Math.Min(InfraredOutputValueMaximum, intensityRatio);
                // 4. limiting the lower value InfraredOutputValueMinimum
                intensityRatio = Math.Max(InfraredOutputValueMinimum, intensityRatio);
                // 5. converting the normalized value to a byte and using 
                // the result as the RGB components required by the image
                byte intensity = (byte)(intensityRatio * 255.0f);*/

                // Prendo dall'array il valore di infrarosso rilevato
                ushort ir = data[i];
                byte intensity = (byte)(ir >> 8);// Calcolo il valore di intensità
                // E salvo nell'array il valore di intensità appena calcolato
                pixels[index++] = intensity;//(byte)(intensity / 1);
                pixels[index++] = intensity;//(byte)(intensity / 1);
                pixels[index++] = intensity;//(byte)(intensity / 0.4);
                ++index;
            }

            // Trasferisco sul WritableBitmap il contenuto di pixels (ovvero l'immagine vere e propria)
            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * bpp, 0);
            bitmap.Unlock();
        }

        /// <summary>
        /// La funzione convertBitmap provvede a trasformare il frame di tipo infrared, 
        /// ricevuto dalla kinect, in un ImageSource.
        /// </summary>
        /// <param name="colorData">ColorData che contiene le informazioni basilari del ColorFrame.</param>
        /// <returns></returns>
        public static void convertBitmap(this WriteableBitmap bitmap, ColorData colorData) 
        {
            // Prelevo dal colorData le informazioni sulla larghezza e l'altezza dell'immagine.
            int width = colorData.width;
            int height = colorData.height;
            // Trasferisco sul WritableBitmap il contenuto di colorData.pixels (ovvero l'immagine vere e propria)
            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), colorData.pixels, width * 4, 0);// Disegno i nuovi pixel
            bitmap.Unlock();

        }

        /// <summary>
        /// Disegna lo scheletro
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="drawingContext"></param>
        public static void drawSkeletons(this WriteableBitmap bitmap, Skeleton[] skeletonList, CoordinateMapper coordinateMapper)
        //public static ImageSource drawSkeletons(Skeleton[] skeletonList, double width, double height, CoordinateMapper coordinateMapper)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            DrawingImage skeletonImage = new DrawingImage();

            // Creo uno sfondo nero per settare la dimensione del render (serve per la stampa degli scheletri)
            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, 450, 700));
            // Creo uno sfondo nero
            bitmap.Clear(Colors.Black);

            // Disegno le ossa e le joints
            foreach (Skeleton skeleton in skeletonList)
            {
                if (skeleton.getStatus())
                    bitmap.drawBones(skeleton, coordinateMapper);
                    //skeleton.drawBones(drawingContext, coordinateMapper);
                // prevent drawing outside of our render area
                //skeletonsDrawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 250, 400));
            }
            // Tutti gli scheletri disegnati vengono salvati in un Skeleton Image
            //drawingContext.DrawImage(skeletonImage, new Rect(new Point(0, 0), new Size(width, height)));
            //drawingContext.Close();
            // RenderTargetBitmap che viene usato per passare da DrawingVisual a BitmapSource
            //RenderTargetBitmap bmp = new RenderTargetBitmap(450, 700, 96, 96, PixelFormats.Pbgra32);
            //bmp.Render(drawingVisual);

            //return encodeBitmap(bmp);
        }

        /// <summary>
        /// drawBones è la funzione che dato uno scheletro e un DrawingContext disegna le varie ossa, disegnando
        /// una linea da un estremo all'altro, e le varie joints (come punti).
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="drawingContext"></param>
        //private static void drawBones(this Skeleton skeleton, DrawingContext drawingContext, CoordinateMapper coordinateMapper)
        private static void drawBones(this WriteableBitmap bitmap, Skeleton skeleton, CoordinateMapper coordinateMapper)
        {
            Point point0, point1;
            JointInformation joint0, joint1;

            // Prendo la lista dallo scheletro
            List<Bone> bones = skeleton.getBones();
            // Creo una lista di tuple, in cui per ogni jointType indichiamo il suo point
            List<Tuple<JointType, Point>> jointCoordinate = new List<Tuple<JointType, Point>>();

            // Determino per ogni joint in Skeleton la loro effettiva posizione
            foreach (var joint in skeleton.getListJointInformation())
            {
                /// Position è la variabile che conterrà i dati relativi alla posizione del joint
                CameraSpacePoint position = joint.getPosition();
                //
                if (position.Z < 0)
                    position.Z = InferredZPositionClamp;

                /// depthSpacePoint (that represents pixel coordinates within a depth image) è la variabile costruita a partire dal coordinateMapper (ottenuto dalla kinect) e dall'oggetto position
                DepthSpacePoint depthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(position);
                // point è l'oggetto costruito a partire dalle coordinate X e Y di depthSpacePoint
                Point point = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                // Quindi, aggiungo nella lista il JointType e il relativo point. (Creiamo una lista di tuple(JointType, Point) per poter poi accedere più 
                // facilmente all'oggetto point (che verrà usato per la stampa del corpo e delle joint stesse).
                jointCoordinate.Add(new Tuple<JointType, Point>(joint.getType(), point));
            }
            // Per ogni osso presente nella lista bones
            foreach (var bone in bones)
            {
                joint0 = skeleton.getJointInformation(bone.Item1);
                joint1 = skeleton.getJointInformation(bone.Item2);
                /// Prendo i due position associati ai JointType che sono estremi dell'osso, ovvero:
                /// prendo dalla lista jointCoordinate il secondo elemento della tupla (Point, ovvero le coordinate del punto) 
                /// che è associato al JoinType che forma l'osso. 
                point0 = jointCoordinate[(int)bone.Item1].Item2;
                point1 = jointCoordinate[(int)bone.Item2].Item2;
                // Se entrambe le joint sono rilevate, allora disegno l'osso,
                // altrimenti la disegno ma in maniera molto più leggera.
                if (joint0.getStatus() == TrackingState.Tracked && joint1.getStatus() == TrackingState.Tracked)
                    bitmap.DrawLineAa((int)point0.X, (int)point0.Y, (int)point1.X, (int)point1.Y, Colors.Red, 6);
                    ///****drawingContext.DrawLine(penTracked, point0, point1);
                else
                    bitmap.DrawLineAa((int)point0.X, (int)point0.Y, (int)point1.X, (int)point1.Y, Colors.LightGray, 1);
                    ///****drawingContext.DrawLine(penNotTracked, point0, point1);
            }
            // Disegna le palline delle joints
            foreach (JointInformation jointInformation in skeleton.getListJointInformation())
            {
                // TrackingState
                TrackingState trackingState = jointInformation.getStatus();
                // Prendi il point
                Point point = jointCoordinate[(int)jointInformation.getType()].Item2;
                // Se il Joint è tracciato, allora lo disegno
                if (trackingState == TrackingState.Tracked)
                    bitmap.FillEllipseCentered((int)point.X, (int)point.Y, 3, 3, Colors.Black);
                    ///****drawingContext.DrawEllipse(trackedJointBrush, null, point, trackedJointThickness, trackedJointThickness);
                // Se il Joint è nello stato Inferred allora lo disegno, però con 
                else if (trackingState == TrackingState.Inferred)
                    bitmap.FillEllipseCentered((int)point.X, (int)point.Y, 1, 1, Colors.Black);
                    ///****drawingContext.DrawEllipse(trackedJointBrush, null, point, inferredJointThickness, inferredJointThickness);
            }
        }

        #region encodeBitmap
        /// <summary>
        /// encodeBitmap trasforma un BitmapSource ricevuto in input in un BitmapImage.
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        private static BitmapImage encodeBitmap(BitmapSource bitmapSource)
        {
            // È il decodificatore che usiamo per passare da BitmapSource a BitmapImage (BitmapSource-JpegBitmapEncoder-BitmapImage).
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            // È la memoria che verrà usato per passare da una rappresentazione all'altra.
            MemoryStream memoryStream = new MemoryStream();
            // BitmapImage che verrà restutita alla fine (e che conterrà i dati di BitmapSource)
            BitmapImage bitMapImage = new BitmapImage();

            // Primo passo della "trasformazione": Da BitmapSource a JpegBitmapEncoder
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            jpegBitmapEncoder.Save(memoryStream);// Uso la zona di memoria inizializzata precedentemente per contenere i dati inseriti in JpegBitmapEncoder
            // Secondo passo della "trasformazione": Da JpegBitmapEncoder a BitmapImage
            bitMapImage.BeginInit();
            bitMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());// Inserisco i dati a partire dai salvati in memoria precedentemente
            bitMapImage.EndInit();
            memoryStream.Close();// Chiudo il collegamento con il flusso di dati 

            // Freezo la BitmapImage creata (in modo tale che possa essere usata anche da altri thread)
            bitMapImage.Freeze();
            // Restituisco l'immagine
            return bitMapImage;
        }
        #endregion
    }
}
