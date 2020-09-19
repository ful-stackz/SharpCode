using System.Collections.Generic;
using Optional;

namespace SharpCode
{
    public class ConstructorBuilder
    {
        private Constructor _constructor = new Constructor();

        public ConstructorBuilder WithParameter(string type, string name, string? receivingMember = null)
        {
            _constructor.Parameters.Add(new Parameter
            {
                Name = name,
                ReceivingMember = receivingMember,
                Type = type,
            });
            return this;
        }

        public ConstructorBuilder WithBaseCall(params string[] passedParameters)
        {
            _constructor.BaseCallParameters = Option.Some<IEnumerable<string>>(passedParameters);
            return this;
        }

        internal Constructor Build()
        {
            return _constructor;
        }
    }
}
