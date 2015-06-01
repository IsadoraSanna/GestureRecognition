using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect & Draw
using Microsoft.Kinect;
using System.Diagnostics;
using System.Windows.Media;

namespace RecognitionGestureFeed_Universal.Recognition.BodyStructure
{
    public class Skeleton : ICloneable
    {
        private readonly Int16 number_joints = 25;// Numero di Joint disponibili
        /* Attributi della classe Skeleton */
        /// <summary>
        /// ID del body associato dalla kinect
        /// leftHand, rightHand stato della mano
        /// joints è la lista che riporta per ogni "giuntura" rilevata dalla kinect 
        /// bones è la lista di tutte le ossa
        /// status che indica se lo scheletro è rilevato o meno
        /// </summary>
        private ulong idBody;
        private int idSkeleton;
        private HandState leftHandStatus;
        private HandState rightHandStatus;
        private List<JointInformation> joints;
        private List<Bone> bones = new List<Bone>();
        private bool status;

        /// <summary>
        /// Costruttori
        /// </summary>
        public Skeleton(int i)
        {
            // Inizializzo l'idBody e l'idSkeleton
            this.idBody = 1000;
            this.idSkeleton = i;
            // Stato delle mani (HandSideState restituisce: unknown = 0, not tracked = 1, aperta = 2, chiusa = 3, lasso = 4)
            leftHandStatus = 0;
            rightHandStatus = 0;
            // Inizializzo la lista joints
            joints = new List<JointInformation>();
            // Inizializzo la lista di bone
            boneBuilder(bones);
            // Inizializzo a false lo status
            status = false;
        }

        /// <summary>
        /// Funzione di aggiornamento delle compenenti dello scheletro
        /// </summary>
        /// <param name="body"></param>
        public void updateSkeleton(Body body, int idSkeleton)
        {
            // Aggiorno lo stato
            if (body.IsTracked)
            {
                status = body.IsTracked;
                idBody = body.TrackingId;
            }
            else
            {
                status = false;
                idBody = 1000;
            }
            // Aggiorno lo stato delle mani (HandSideState restituisce: unknown = 0, not tracked = 1, aperta = 2, chiusa = 3, lasso = 4)
            leftHandStatus = body.HandLeftState;
            rightHandStatus = body.HandRightState;

            // Aggiorno le coordinate delle varie joint rilevate
            if (joints.Count > 0)
            {
                foreach (JointInformation ji in this.getListJointInformation())
                    ji.Update(body.Joints[ji.getType()], body.JointOrientations[ji.getType()].Orientation);
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
        }

        /// <summary>
        /// Ritorna l'Id del body a cui è associato lo scheletro
        /// </summary>
        /// <returns></returns>
        public ulong getIdBody()
        {
            return this.idBody;
        }
        /// <summary>
        /// Ritorna l'Id dello scheletro
        /// </summary>
        /// <returns></returns>
        public int getIdSkeleton()
        {
            return this.idSkeleton;
        }
        /// <summary>
        /// Funzione per settare lo scheletro qualora non sia tracciato
        /// </summary>
        public void updateSkeleton()
        {
            // Aggiorno lo stato
            status = false;
            // Aggiorno lo stato delle mani pongo lo HandSideState come: unknown = 0)
            leftHandStatus = HandState.NotTracked;
            rightHandStatus = HandState.NotTracked;
            // Cancello le coordinate delle varie joint rilevate
            if(joints.Count > 0) 
                joints.Clear();
        }
        
        /// <summary>
        /// Dato il nome del Joint a cui vuole accedere, restituisce il JointInformation relativo
        /// </summary>
        /// <param name="jointType"></param>
        /// <returns></returns>
        public JointInformation getJointInformation(JointType jointType)
        {
            return this.joints[(int)jointType];
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
            return this.MemberwiseClone();
        }
        
        #region utilities
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

