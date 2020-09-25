using System;
using System.Runtime.Serialization;

namespace SharpCode
{
    /// <summary>
    /// The exception that is thrown when building with the provided configuration will result in invalid source code.
    /// </summary>
    public class SyntaxException : Exception
    {
        public SyntaxException() { }

        public SyntaxException(string message)
            : base(message)
        { }

        public SyntaxException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
