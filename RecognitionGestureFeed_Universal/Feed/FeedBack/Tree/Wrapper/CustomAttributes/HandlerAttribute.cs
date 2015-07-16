using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Attributes
using System.Reflection;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class HandlerAttribute : Attribute
    {
        public object obj { get; private set; }
        public float value { get; private set; }

        //
        public HandlerAttribute(object obj, float value)
        {
            this.obj = obj;
            this.value = value;
        }
        public HandlerAttribute(){}
    }
}
