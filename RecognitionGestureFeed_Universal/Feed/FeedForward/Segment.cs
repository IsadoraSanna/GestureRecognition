using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//La classe Segment rappresenta il singolo segmento che rappresenterà il tracciato della gesture
namespace RecognitionGestureFeed_Universal.Feed.FeedForward
{
    public class Segment
    {
        //possiede una lunghezza e una direzione
        public int size { set; get; }
        public float length { set; get; }
        public float inclination { set; get; }

        public Segment(int size, float length, float inclination)
        {
            this.size = size;
            this.length = length;
            this.inclination = inclination;
        }
    }
}
