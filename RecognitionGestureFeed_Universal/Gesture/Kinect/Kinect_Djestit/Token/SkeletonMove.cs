using Unica.Djestit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unica.Djestit.Kinect2
{
    class SkeletonMove : GroundTerm
    {
        public int id { get; private set; }

        public SkeletonMove(int id) : base()
        {
            this.id = id;
            this.Accepts = func_accepts;
        }

        public bool func_accepts(Token token)
        {
            if (token.GetType() == typeof(SkeletonToken))
            {
                SkeletonToken skeletonToken = (SkeletonToken)token;
                if (skeletonToken.type != TypeToken.Move)
                    return false;
                if (this.id != skeletonToken.identifier)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}
