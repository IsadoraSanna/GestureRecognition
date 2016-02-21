using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Microsoft Kinect
using Microsoft.Kinect;

namespace RecognitionGestureFeed_Universal.Recognition.Kinect
{
    public static class InitializationKinect
    {
        public static void OpenSensor(this KinectSensor kinectSensor)
        {
            // Apro la connessione con il dispositivo
            kinectSensor = KinectSensor.GetDefault();
            kinectSensor.Open();
            // Verifica se è stata aperta correttamente la connessione
            if (!kinectSensor.IsOpen)
                throw new System.ArgumentException("Kinect not be connect.", "Connect the Kinect at PC");
        }

        public static void CloseSensor(this KinectSensor kinectSensor)
        {
            // Chiude la connessione con il dispositivo
            kinectSensor.Close();
            kinectSensor = null;
        }
    }
}
