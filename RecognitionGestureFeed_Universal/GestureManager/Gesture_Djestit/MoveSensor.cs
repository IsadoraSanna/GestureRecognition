using RecognitionGestureFeed_Universal.Djestit;
using RecognitionGestureFeed_Universal.Recognition.BodyStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.GestureManager.Gesture_Djestit
{

    class MoveSensor
    {
        public event GestureEventHandler onSkeletonStart;
        public event GestureEventHandler onSkeletonMove;
        public event GestureEventHandler onSkeletonEnd;
        //element... cosa è??
        private int capacity;
        private Term root;
        private GestureStateSequence sequence;
        private List<int> gestureToEvent;
        private List<int> eventToGesture;

        /*if (root instanceof djestit.Term) {
            this.root = root;
        } else {
            this.root = djestit.expression(root);
        }*/

        public MoveSensor(Term root, int capacity)
        {
            this.capacity = capacity;
            this.root = root;
            //statesequence e gesturestatesequence hanno il tipo di capacity diverso. controllare!!
            this.sequence = new GestureStateSequence(this.capacity);
            this.eventToGesture = new List<int>();
            this.gestureToEvent = new List<int>();
            this.gestureToEvent.Add(-1);


        }

        public Token generateToken(TypeToken type, JointInformation jointInformation){
            GestureToken token = new GestureToken(type, jointInformation);
            switch(type)
            {
                case TypeToken.Start:
                    int identifier = firstId(jointInformation.idSkeleton);
                    this.eventToGesture[jointInformation.idSkeleton] = identifier;
                    this.gestureToEvent[identifier] = jointInformation.idSkeleton;
                    token.identifier = (ulong)identifier; 
                    break;
                case TypeToken.Move:
                    token.identifier = (ulong)this.eventToGesture[jointInformation.idSkeleton];
                    break;
                case TypeToken.End:
                    token.identifier = (ulong)this.eventToGesture[jointInformation.idSkeleton];
                    this.eventToGesture.RemoveAt(jointInformation.idSkeleton);
                    this.gestureToEvent.RemoveAt((int)token.identifier);
                    break;
                default:
                    break;           
            }
            this.sequence.push(token);
            //token.sequence = this.sequence; ???? MIO DIO!!!!
            return token;
        }

        public int firstId(int id)
        {
            for (var i = 0; i < this.gestureToEvent.Count(); i++)
            {
                if (gestureToEvent[i] == null)
                    return i;
            }
            this.gestureToEvent.Add(id);
            return this.gestureToEvent.Count() - 1;
        }

        public void raiseMoveEvent(Skeleton skeleton, TypeToken typeToken)
        {
            foreach(JointInformation ji in skeleton.getListJointInformation())
            {
                Token token = this.generateToken(typeToken, ji);
                this.root.fire(token);
            }
        }

        public void _onSkeletonStart(Skeleton sender)
        {
            this.raiseMoveEvent(sender, TypeToken.Start);
        }
        public void _onSkeletonMove(Skeleton sender)
        {
            this.raiseMoveEvent(sender, TypeToken.Move);
        }
        public void _onSkeletonEnd(Skeleton sender)
        {
            this.raiseMoveEvent(sender, TypeToken.End);
        }
    }
}
