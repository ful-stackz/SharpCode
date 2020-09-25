using System;
using System.Runtime.Serialization;

namespace SharpCode
{
    /// <summary>
    /// The exception that is thrown when attempting to build source code, but a required setting is missing.
    /// </summary>
    public class MissingBuilderSettingException : Exception
    {
        public MissingBuilderSettingException() { }

        public MissingBuilderSettingException(string message)
            : base(message)
        { }

        public MissingBuilderSettingException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected MissingBuilderSettingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
