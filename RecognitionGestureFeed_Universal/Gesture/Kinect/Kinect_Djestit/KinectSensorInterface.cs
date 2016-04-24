using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Djestit
using Unica.Djestit;
// JointInformation
using Unica.Djestit.Recognition.Kinect2;

namespace Unica.Djestit.Kinect2
{
    public class KinectSensorInterface
    {
        /* Attributi */
        // Sensore usato per comunicare al motorino i nuovi token
        protected SkeletonSensor sensor;
        // Capacità del buffer
        protected int capacity = 5;

        /* Costruttore */
        /// <summary>
        /// Definisce un nuovo KinectSensor Interface
        /// </summary>
        /// <param name="acquisitionManager"></param>
        /// <param name="expression"></param>
        public KinectSensorInterface(Term expression)
        {
            // Associa all'evento SkeletonsFrameManaged il relativo handler.
            AcquisitionManager.getInstance().SkeletonsFrameManaged += updateSkeleton;

            // Inizializza la variabile KinectSensor
            sensorDefine(expression);
        }
        public KinectSensorInterface()
        {

        }

        /* Metodi */
        /// <summary>
        /// Assegna al sensore la lista delle gesture da riconoscere.
        /// </summary>
        /// <param name="expression"></param>
        protected void sensorDefine(Term expression)
        {
            // Inizializza la variabile KinectSensor
            this.sensor = new SkeletonSensor(expression, this.capacity);
        }

        /// <summary>
        /// Update Skeleton.
        /// </summary>
        /// <param name="skeletonList"></param>
        public void updateSkeleton(Skeleton[] skeletonList)
        {
            // Per ogni scheletro rilevato avvio il motorino
            foreach (Skeleton skeleton in skeletonList)
            {
                // Creo uno skeleton token
                Token token = GenerateToken(skeleton);

                // Se è stato creato un token, lo sparo al motore
                if (token != null)
                {
                    //if (token.type != TypeToken.End)
                    this.sensor.root.fire(token);
                }
            }
        }

        /// <summary>
        /// Crea uno Skeleton Token.
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        public virtual Token GenerateToken(Skeleton skeleton)
        {
            // Determino il tipo (Start, Move o End) e ne creo il token, e quindo lo genero
            if (skeleton.getStatus())
            {
                if (sensor.checkId(skeleton.idSkeleton))
                    return (SkeletonToken)sensor.generateToken(TypeToken.Move, skeleton);
                else
                    return (SkeletonToken)sensor.generateToken(TypeToken.Start, skeleton);
            }
            else if (sensor.checkId(skeleton.idSkeleton))
            {
                return (SkeletonToken)sensor.generateToken(TypeToken.End, skeleton);
            }

            return null;
        }
    }
}
