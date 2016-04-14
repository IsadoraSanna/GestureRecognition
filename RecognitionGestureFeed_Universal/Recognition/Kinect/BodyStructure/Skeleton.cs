using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect e Face
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
// Kinect Gesture
using RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_VisualGestureBuilder;
// Draw
using System.Windows.Media;

namespace RecognitionGestureFeed_Universal.Recognition.Kinect.BodyStructure
{
    public class Skeleton : ICloneable
    {
        private readonly Int16 number_joints = 25;// Numero di Joint disponibili
        /* Attributi della classe Skeleton */
        /// <summary>
        /// idBody: id del body associato dalla kinect
        /// idSkeleton: id dello scheletro
        /// clippedEdges: indica se un qualche joint "esce" fuori dallo spazio visivo della camera
        /// leftHandStatus: status della mano sinistra (unknown = 0, not tracked = 1, aperta = 2, chiusa = 3, lasso = 4)
        /// rightHandStatus: status della mano destra (unknown = 0, not tracked = 1, aperta = 2, chiusa = 3, lasso = 4)
        /// joints:  è la lista che riporta per ogni "giuntura" rilevata dalla kinect 
        /// bones: è la lista di tutte le ossa
        /// lean: rappresenta il body lean
        /// leanTrackingState: indica lo stato di tracciamento del lean (not tracked = 0, infeered = 1, tracked = 2)
        /// status: che indica se lo scheletro è rilevato o meno
        /// timeSpan: è il tempo di rilevamento dell'ultimo frame utilizzato per aggiornare lo scheletro
        /// faceFrameReader: reader per i FaceFrame in arrivo dalla kinect
        /// faceFrameSource: rappresenta la sorgente per i FaceFrame in arrivo dalla kinect
        /// faceFrameResults: oggetto che conterrà tutte le informazioni circa la faccia rilevata (dall'infrarosso al colore, dalle proprietà alle espressioni riconosciute)
        /// highDefinitionFaceFrameReader: reader per l'HighDefinitionFaceFrame in arrivo dalla kinect
        /// highDefitionFaceFrameSource: rappresenta la sorgente per l'HighDefinitionFaceFrame in arrivo dalla kinect
        /// faceAlignment: memorizza i punti principali di un Face (ovvero occhi, naso, bocca) a partire di un'immagine in 2D.
        /// faceModel: rappresenta un face model (da cui si possono ottenere informazioni circa colore dei capelli, della pelle, la scala ecc.
        /// colorSkeleton: colore con cui verrà rappresentato lo scheletro(modificabile da parte dell'utente).
        /// gestureDetector: rilevatore delle gesture riconosciute tramite gli algoritmi forniti dalla Kinect.
        /// </summary>
        // Skeleton
        public ulong idBody { get; private set; }
        public int idSkeleton { get; private set; }
        public FrameEdges clippedEdges { get; private set; }
        public HandState leftHandStatus {get; private set;}
        public HandState rightHandStatus {get; private set;}
        internal List<JointInformation> joints;
        internal List<Bone> bones = new List<Bone>();
        public bool status { get; private set; }
        public PointF lean { get; private set; }
        public TrackingState leandTrackingState { get; private set; }
        public TimeSpan timeSpan { get; private set; }
        // Stampa
        public Pen colorSkeleton { get; set; }

        /* Costruttore */
        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="i"></param>
        public Skeleton(int i, Pen color)
        {
            // Inizializzo a false lo status
            this.status = false;
            // Inizializzo l'idBody e l'idSkeleton
            this.idBody = 0;
            this.idSkeleton = i;
            // Stato delle mani (HandSideState restituisce: unknown = 0, not tracked = 1, aperta = 2, chiusa = 3, lasso = 4)
            this.leftHandStatus = 0;
            this.rightHandStatus = 0;
            // Inizializzo la lista joints
            this.joints = new List<JointInformation>();
            // Inizializzo la lista di bone
            boneBuilder(this.bones);
            // Associo allo scheletro il colore con cui verrà rappresenta in bitmap
            this.colorSkeleton = color;
        }

        /* Metodi */
        /// <summary>
        /// Funzione di aggiornamento delle compenenti dello scheletro
        /// </summary>
        /// <param name="body"></param>
        public virtual void updateSkeleton(Body body, TimeSpan ts)
        {
            // Aggiorno lo stato dello scheletro qualora il corpo sia tornato attivo 
            if (body.IsTracked && !this.status)
            {
                this.status = body.IsTracked;
                this.idBody = body.TrackingId;
            }
            // Aggiorno le informazioni riguardanti il clippedEdge e il lean 
            this.clippedEdges = body.ClippedEdges;
            this.lean = body.Lean;
            this.leandTrackingState = body.LeanTrackingState;
            // Aggiorno lo stato delle mani (HandSideState restituisce: unknown = 0, not tracked = 1, aperta = 2, chiusa = 3, lasso = 4)
            this.leftHandStatus = body.HandLeftState;
            this.rightHandStatus = body.HandRightState;

            // Aggiorno le coordinate delle varie joint rilevate
            if (this.joints.Count > 0)
            {
                foreach (JointInformation jInf in this.getListJointInformation())
                    jInf.Update(body.Joints[jInf.getType()], body.JointOrientations[jInf.getType()].Orientation);
                
            }
            else
            {
                for (int index = 0; index < number_joints; index++)
                {
                    // A seconda di quante joint sono riconosciuto nel body, le aggiungiamo in una lista di tuple composta da:
                    // 1° elemento: ID del body
                    // 3° elemento: l'oggetto Joints
                    // 4° elemento: Orientamento del joint
                    joints.Add(new JointInformation(idBody, body.Joints[((JointType)index)], body.JointOrientations[((JointType)index)].Orientation, idSkeleton));
                }
            }

            // Aggiorno lo scheletro
            foreach(Bone bone in this.bones)
            {
                bone.update(this.getJointInformation(bone.start), this.getJointInformation(bone.end));
            }
            // Aggiorno il timespan
            timeSpan = ts;
        }

        /// <summary>
        /// Funzione di aggiornamento qualora lo scheletro non sia tracciato
        /// </summary>
        public virtual void updateSkeleton()
        {
            // Aggiorna lo stato in false e resetta idBody
            this.status = false;
            this.idBody = 0;
            // Aggiorna lo stato delle mani, pone lo HandSideState come: unknown = 0)
            this.leftHandStatus = HandState.NotTracked;
            this.rightHandStatus = HandState.NotTracked;
            // Aggiorno le informazioni riguardanti il clippedEdge e il lean 
            this.clippedEdges = FrameEdges.None;
            this.lean = new PointF();
            this.leandTrackingState = TrackingState.NotTracked;
            // Cancella le coordinate delle varie joint rilevate
            if (joints.Count > 0)
                joints.Clear();
            // Aggiorno lo scheletro
            foreach (Bone bone in this.bones)
            {
                bone.update();
            }
        }
     
        /// <summary>
        /// Dato il nome del Joint a cui vuole accedere, restituisce il JointInformation relativo
        /// </summary>
        /// <param name="jointType"></param>
        /// <returns></returns>
        public JointInformation getJointInformation(JointType jointType)
        {
            return (JointInformation)this.joints[(int)jointType].Clone();
        } 
        /// <summary>
        /// Restituisci la lista di ossa associata a quello scheletro
        /// </summary>
        /// <returns></returns>
        public List<Bone> getBones()
        {
            return this.bones;
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public List<JointInformation> getListJointInformation()
        {
            if (this.getStatus())
                return this.joints;
            else
                return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool getStatus()
        {
            return this.status;
        }
        /// <summary>
        /// Fornisce una nuova copia dell'oggetto scheletro.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            // Creo una prima copia dello scheletro
            Skeleton clone = (Skeleton)this.MemberwiseClone();
            // Creo una copia delle liste
            this.joints = this.joints.Select(item => (JointInformation)item.Clone()).ToList();
            this.bones = this.bones.Select(item => (Bone)item.Clone()).ToList();
            // Restituisco il valore
            return clone;
        }
        
        #region Creazione Bone
        private static void boneBuilder(List<Bone> bones)
        {
            // Torso
            bones.Add(new Bone(JointType.Head, JointType.Neck));
            bones.Add(new Bone(JointType.Neck, JointType.SpineShoulder));
            bones.Add(new Bone(JointType.SpineShoulder, JointType.SpineMid));
            bones.Add(new Bone(JointType.SpineMid, JointType.SpineBase));
            bones.Add(new Bone(JointType.SpineShoulder, JointType.ShoulderRight));
            bones.Add(new Bone(JointType.SpineShoulder, JointType.ShoulderLeft));
            bones.Add(new Bone(JointType.SpineBase, JointType.HipRight));
            bones.Add(new Bone(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            bones.Add(new Bone(JointType.ShoulderRight, JointType.ElbowRight));
            bones.Add(new Bone(JointType.ElbowRight, JointType.WristRight));
            bones.Add(new Bone(JointType.WristRight, JointType.HandRight));
            bones.Add(new Bone(JointType.HandRight, JointType.HandTipRight));
            bones.Add(new Bone(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            bones.Add(new Bone(JointType.ShoulderLeft, JointType.ElbowLeft));
            bones.Add(new Bone(JointType.ElbowLeft, JointType.WristLeft));
            bones.Add(new Bone(JointType.WristLeft, JointType.HandLeft));
            bones.Add(new Bone(JointType.HandLeft, JointType.HandTipLeft));
            bones.Add(new Bone(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            bones.Add(new Bone(JointType.HipRight, JointType.KneeRight));
            bones.Add(new Bone(JointType.KneeRight, JointType.AnkleRight));
            bones.Add(new Bone(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            bones.Add(new Bone(JointType.HipLeft, JointType.KneeLeft));
            bones.Add(new Bone(JointType.KneeLeft, JointType.AnkleLeft));
            bones.Add(new Bone(JointType.AnkleLeft, JointType.FootLeft));
        }
        #endregion
    }
}

