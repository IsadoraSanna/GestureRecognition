using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes
{
    /// Questa classe rappresenta le eccezioni che posso essere inviate a causa di operazioni non corrette
    /// su un oggetto di tipo modifies. 
    internal class InvalidModifiesException : SystemException
    {
        // Costruttori
        public InvalidModifiesException(string message) : base(message) { }
        public InvalidModifiesException(string message, System.Exception inner) : base(message, inner) { }

        // Propagazione Eccezione
        protected InvalidModifiesException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) { }
    }
}
