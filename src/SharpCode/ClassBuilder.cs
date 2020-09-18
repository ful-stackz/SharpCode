namespace SharpCode
{
    public class ClassBuilder
    {
        private Class _class = new Class();

        internal ClassBuilder() { }

        internal ClassBuilder(AccessModifier accessModifier, string name)
        {
            _class.AccessModifier = accessModifier;
            _class.Name = name;
        }

        public ClassBuilder WithNamespace(string @namespace)
        {
            _class.Namespace = @namespace;
            return this;
        }

        public ClassBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _class.AccessModifier = accessModifier;
            return this;
        }

        public ClassBuilder WithName(string name)
        {
            _class.Name = name;
            return this;
        }

        public ClassBuilder WithField(FieldBuilder builder)
        {
            _class.Fields.Add(builder.Build());
            return this;
        }

        public ClassBuilder WithProperty(PropertyBuilder builder)
        {
            _class.Properties.Add(builder.Build());
            return this;
        }

        public string ToSourceCode(bool formatted = false)
        {
            return _class.ToSourceCode(formatted);
        }

        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Class Build()
        {
            return _class;
        }
    }
}
