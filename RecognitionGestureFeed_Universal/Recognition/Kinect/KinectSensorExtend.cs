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
        private static KinectSensorExtend instance;
        private KinectSensor kinectSensor;

        /* Costruttore */
        private KinectSensorExtend()
        {
            OpenSensor();
        }

        /* Metodi */
        public static KinectSensorExtend Instance
        {
            get
            {
                if (instance == null)
                    instance = new KinectSensorExtend();

                return instance;
            }
        }

        public KinectSensor getKinectSensor()
        {
            return kinectSensor;
        }

        /// <summary>
        /// Apre la connessione con il dispositivo.
        /// </summary>
        /// <returns></returns>
        public void OpenSensor()
        {
            kinectSensor = KinectSensor.GetDefault();
            kinectSensor.Open();
        }

        /// <summary>
        /// Chiude la connessione con il dispositivo, e pone l'oggetto kinectSensor a null;
        /// </summary>
        /// <param name="kinectSensor"></param>
        public void Close()
        {
            kinectSensor.Close();
            kinectSensor = null;
        }

    }
}
