using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;

namespace Unica.Djestit.Recognition.Kinect2
{
    public class KinectSensorExtend
    { 
        /* Attributi */
        private static KinectSensor kinectSensor;
        private static KinectSensorExtend singleton = null;

        /* Costruttore */
        private KinectSensorExtend()
        {
            OpenSensor();
        }

        /* Metodi */
        public static KinectSensor getSensor()
        {
            if (singleton == null)
                    singleton = new KinectSensorExtend();

            return kinectSensor;
        }

        public KinectSensor getKinectSensor()
        {
            return kinectSensor;
        }

        /// <summary>
        /// Apre la connessione con il dispositivo.
        /// </summary>
        /// <returns></returns>
        private void OpenSensor()
        {
            kinectSensor = KinectSensor.GetDefault();
            kinectSensor.Open();
        }

        /// <summary>
        /// Chiude la connessione con il dispositivo, e pone l'oggetto kinectSensor a null;
        /// </summary>
        /// <param name="kinectSensor"></param>
        public static void Close()
        {
            kinectSensor.Close();
            kinectSensor = null;
        }

    }
}
