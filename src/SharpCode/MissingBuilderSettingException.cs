using System;
using System.Runtime.Serialization;

namespace SharpCode
{
    /// <summary>
    /// The exception that is thrown when attempting to build source code, but a required setting is missing.
    /// </summary>
    public class MissingBuilderSettingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingBuilderSettingException"/> class.
        /// </summary>
        public MissingBuilderSettingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingBuilderSettingException"/> class.
        /// </summary>
        public MissingBuilderSettingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingBuilderSettingException"/> class.
        /// </summary>
        public MissingBuilderSettingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingBuilderSettingException"/> class.
        /// </summary>
        protected MissingBuilderSettingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
