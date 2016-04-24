using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unica.Djestit.Kinect2
{
    public class SkeletonEventArgs : EventArgs
    {
        public readonly SkeletonSensor sensor;

        public SkeletonEventArgs(SkeletonSensor sensor)
        {
            this.sensor = sensor;
        }
    }
}
