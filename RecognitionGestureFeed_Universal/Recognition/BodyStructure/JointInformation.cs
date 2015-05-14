using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect & Debug
using Microsoft.Kinect;
using System.Diagnostics;
// Point
using System.Windows;
// XmlAttribute
using System.Xml.Serialization;

namespace RecognitionGestureFeed_Universal.Recognition.BodyStructure
{
    [Serializable]
    public class JointInformation : ICloneable
    {
        /// <summary>
        /// - idBody: ID legato alla Joints (equivale ai values di https://msdn.microsoft.com/en-us/library/microsoft.kinect.jointtype.aspx)
        /// - type: Indica il jointType a cui fa riferimento il joint
        /// - joint: l'oggetto Joint vero e proprio (che contiene coordinate x,y,z)
        /// - position: le coordinate della posizione del joint
        /// - orientation: indica l'orientamento del joint
        /// - status: se il sensore non rileva il joint in questione, allora questo viene posto come false, viceversa se vi sono dei dati disponibili viene posto a true
        ///</summary>
        //public ulong idBody { get{return idBody;} set{ idBody = value ;} }
        public ulong idBody { get; set; }
        public JointType type { get; set; }
        public Joint joint { get; set; }
        public CameraSpacePoint position { get; set; }
        public Vector4 orientation { get; set; }
        public TrackingState status { get; set; }

        
        /* Costruttori */
        // Assegno al nuovo oggetto le informazioni passate in input
        public JointInformation(ulong idBody, Joint joint, Vector4 orientation)
        {
            this.idBody = idBody;
            this.type = joint.JointType;
            this.joint = joint;
            this.position = joint.Position;
            this.orientation = orientation; 
            this.status = joint.TrackingState;
        }
        public JointInformation()
        {

        }

        /// <summary>
        /// Restituisce le informazioni contenute nell'oggetto come tupla<idBody, name, joint, orientation>.
        /// Qualora status di jointInformation è false, restituisce null
        /// </summary>
        /// <returns></returns>
        public Tuple<ulong, Joint, Vector4, TrackingState> getJointInformation()
        {
            return Tuple.Create(this.idBody, this.joint, this.orientation, this.status);
        }

        // Restituisce le informazioni contenute nell'oggetto come stringa
        public String toString()
        {
            String result=null;
            if (this.status != TrackingState.NotTracked)// ovvero se il joint è tracciato
                result = ("ID Body: " + this.idBody + " - Joint: " + this.joint.ToString() + " - Joint's Orientation: " + this.orientation);
            else
                result = ("ID Body: " + this.idBody);
            return result;
        }

        #region utilities
        /* Clone */
        // Override del metodo Clone, clona il joint in input
        public object Clone()
        {
            return this.MemberwiseClone() as JointInformation;
        }
        /* Get functions */
        // Restituisce l'ID del Body associato
        public ulong getId()
        {
            return this.idBody;
        }
        // Restituisce il Joint 
        public Joint getJoint()
        {
            return this.joint;
        }
        // Restituisce l'orientamento del Joint
        public Vector4 getOrientation()
        {
            return this.orientation;
        }
        // Indica se l'oggetto è fa riferimento ad un joint valido oppure no
        public TrackingState getStatus()
        {
            return this.status;
        }
        // Restituisce il JointType
        public JointType getType()
        {
            return this.type;
        }
        // Restituisce il CameraSpacePoint costruito sulle coordinate X e Y del joint
        public CameraSpacePoint getPosition()
        {
            return this.position;
        }
        #endregion
    }
}