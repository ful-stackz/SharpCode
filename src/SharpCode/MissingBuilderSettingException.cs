using System;
using System.Runtime.Serialization;

namespace SharpCode
{
    public class MissingBuilderSettingException : Exception
    {
        public MissingBuilderSettingException()
        {
        }

        public MissingBuilderSettingException(string message)
            : base(message)
        {
        }

        public MissingBuilderSettingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MissingBuilderSettingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
