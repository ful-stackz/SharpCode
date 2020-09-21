using System;
using System.Runtime.Serialization;

namespace SharpCode
{
    public class SyntaxException : Exception
    {
        public SyntaxException()
        {
        }

        public SyntaxException(string message)
            : base(message)
        {
        }

        public SyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
