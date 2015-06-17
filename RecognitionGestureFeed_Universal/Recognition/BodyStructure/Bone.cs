using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect
using Microsoft.Kinect;
// Vector3D
using System.Windows.Media.Media3D;

namespace RecognitionGestureFeed_Universal.Recognition.BodyStructure
{
    /// <summary>
    /// La classe bone rappresenta un osso della classe scheletro
    /// </summary>
    public class Bone : ICloneable
    {
        /* Attributi */
        // Le joint dell'osso (i due estremi)
        public JointType start { get; private set; }
        public JointType end { get; private set; }
        // Coordinate in Vector4 dell'osso
        public Vector3D position { get; private set; }
        public Double lenght { get; private set; }
        // High (Fully tracked) or Low (Not tracked)
        public TrackingState trackingState { get; private set; }

        /* Costruttore */
        /// <summary>
        /// Definisce un nuovo osso tra le due joint in input. 
        /// </summary>
        /// <param name="jointType1"></param>
        /// <param name="jointType2"></param>
        public Bone(JointType jointType1, JointType jointType2)// : base(jointType1, jointType2) 
        {
            // Assegno i due valori presi in input
            this.start = jointType1;
            this.end = jointType2;
        }

        /* Metodi */
        /// <summary>
        /// Aggiorna la positione a partire dai due JointInformation (appena rilevati dalla kinect) e associati
        /// ai suoi due estremi.
        /// </summary>
        /// <param name="joint1"></param>
        /// <param name="jI2"></param>
        public void update(JointInformation jI1, JointInformation jI2)
        {
            // Aggiorna la posizione dell'osso e la lunghezza
            this.position = JointInformation.returnVector3D(jI1, jI2);
            this.lenght = this.position.Length;
            // Aggiorna lo stato
            if (jI1.status == TrackingState.Tracked && jI2.status == TrackingState.Tracked)
                this.trackingState = TrackingState.Tracked;
            else if (jI1.status == TrackingState.Inferred || jI2.status == TrackingState.Inferred)
                this.trackingState = TrackingState.Inferred;
            else
                this.trackingState = TrackingState.NotTracked;
        }
        /// <summary>
        /// Aggiorna la posizione dell'osso quando il body non è più rilevabile
        /// </summary>
        public void update()
        {
            this.position = new Vector3D();
            this.lenght = 0f;
            this.trackingState = TrackingState.NotTracked;
        }

        /// <summary>
        /// Restituisce l'angolo formato da due ossa
        /// </summary>
        /// <param name="boneA"></param>
        /// <param name="boneB"></param>
        public static double returnAngle(Bone boneA, Bone boneB)
        {
            // Prevedere l'inversione mannaggi!
            return calculateAngle(boneA.position, boneB.position);
        }

        /// <summary>
        /// Calcola l'angolo tra due Vector3D
        /// </summary>
        /// <param name="vector3D1"></param>
        /// <param name="vector3D2"></param>
        /// <returns></returns>
        private static double calculateAngle(Vector3D vector3D1, Vector3D vector3D2)
        {
            // Angolo 
            double degree;
            // Normalizza i due vettori
            vector3D1.Normalize();
            vector3D2.Normalize();
            // Calcola il rapporto tra i due vettori
            double ratio = Vector3D.DotProduct(vector3D1, vector3D2);

            // 
            if (ratio < 0)
                degree = Math.PI - 2.0 * Math.Asin((-vector3D1 - vector3D2).Length / 2.0);
            else
                degree = 2.0 * Math.Asin((vector3D1 - vector3D2).Length / 2.0);
            
            // Ritorna l'angolo appena calcolato
            return degree * (180.0 / Math.PI);
        }

        /// <summary>
        /// Crea il clone associato
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}