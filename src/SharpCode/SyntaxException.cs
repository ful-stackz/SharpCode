using System;
using System.Runtime.Serialization;

namespace SharpCode
{
    /// <summary>
    /// The exception that is thrown when building with the provided configuration will result in invalid source code.
    /// </summary>
    public class SyntaxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class.
        /// </summary>
        public SyntaxException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class.
        /// </summary>
        public SyntaxException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class.
        /// </summary>
        public SyntaxException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class.
        /// </summary>
        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
