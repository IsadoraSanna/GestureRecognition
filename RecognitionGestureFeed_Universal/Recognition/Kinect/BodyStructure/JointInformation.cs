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
// Vector3D
using System.Windows.Media.Media3D;

namespace RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure
{
    [Serializable()]
    public class JointInformationXML
    {
        /* Attributi */
        public ulong idBody { get; set; }
        public JointType type { get; set; }
        public CameraSpacePoint position { get; set; }
        public Vector4 orientation { get; set; }
        public TrackingState status { get; set; }
        public int idSkeleton { get; set; }

        /* Costruttore */
        public JointInformationXML()
        {

        }

        /* Metodo */
        /// <summary>
        /// Salva le informazioni contenute in JointInformation all'interno dell'oggetto JointInformationXML
        /// </summary>
        /// <param name="jointInformation"></param>
        public void fromJointInformation(JointInformation jointInformation)
        {
            this.idBody = jointInformation.getId();
            this.type = jointInformation.getType();
            this.position = jointInformation.getPosition();
            this.orientation = jointInformation.getOrientation();
            this.status = jointInformation.getStatus();
            this.idSkeleton = jointInformation.getIdSkeleton();
        }

    }

    [Serializable()]
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
        public ulong idBody { get; set; }
        public JointType type { get; set; }
        public Joint joint { get; set; }
        public CameraSpacePoint position { get; set; }
        public Vector4 orientation { get; set; }
        public TrackingState status { get; set; }
        public int idSkeleton { set; get; }

        
        /* Costruttori */
        /// <summary>
        /// Assegno al nuovo oggetto le informazioni passate in input
        /// </summary>
        /// <param name="idBody"></param>
        /// <param name="joint"></param>
        /// <param name="orientation"></param>
        /// <param name="idSkeleton"></param>
        public JointInformation(ulong idBody, Joint joint, Vector4 orientation, int idSkeleton)
        {
            this.idBody = idBody;
            this.type = joint.JointType;
            this.joint = joint;
            this.position = joint.Position;
            this.orientation = orientation; 
            this.status = joint.TrackingState;
            this.idSkeleton = idSkeleton;
        }
        public JointInformation()
        {
            
        }
        public JointInformation(int idSkeleton)
        {
            this.status = TrackingState.Tracked;
            this.idSkeleton = idSkeleton;
        }
        public JointInformation(CameraSpacePoint position, int idSkeleton)
        {
            this.position = position;
            this.idSkeleton = idSkeleton;
        }

        /// <summary>
        /// Aggiorna le informazioni del JointInformation.
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="orientation"></param>
        public void Update(Joint joint, Vector4 orientation)
        {
            this.joint = joint;
            this.position = joint.Position;
            this.orientation = orientation;
            this.status = joint.TrackingState;
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
        /// <summary>
        /// Restituisce il CameraSpacePoint costruito sulle coordinate X e Y del joint
        /// </summary>
        /// <returns></returns>
        public CameraSpacePoint getPosition()
        {
            return this.position;
        }
        /// <summary>
        /// Restituisce l'ID dello Skeleton a cui è associato
        /// </summary>
        /// <returns></returns>
        public int getIdSkeleton()
        {
            return this.idSkeleton;
        }

        /// <summary>
        /// Restituisce la distanza tra due JointInformation (se questi non sono diversi restituisce zero).
        /// </summary>
        /// <param name="jointInformation1"></param>
        /// <param name="jointInformation2"></param>
        /// <returns></returns>
        public double calculateDistance(JointInformation jointInformation1, JointInformation jointInformation2)
        {
            if (jointInformation1.getType() != jointInformation2.getType())
                return returnVector3D(jointInformation1, jointInformation2).Length;
            else
                return 0d;
        }

        /// <summary>
        /// Calcola il Vector3D a partire dalla posizione di due JointInformation
        /// </summary>
        /// <param name="jI1"></param>
        /// <param name="jI2"></param>
        /// <returns></returns>
        internal static Vector3D returnVector3D(JointInformation jI1, JointInformation jI2)
        {
            CameraSpacePoint positionJoint1 = jI1.getPosition();
            CameraSpacePoint positionJoint2 = jI2.getPosition();
            return new Vector3D((positionJoint1.X - positionJoint2.X), (positionJoint1.Y - positionJoint2.Y), (positionJoint1.Z - positionJoint2.Z));
        }
        #endregion
    }
}