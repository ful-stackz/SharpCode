using System;

namespace SharpCode
{
    public class FieldBuilder
    {
        private Field _field = new Field();

        internal FieldBuilder() { }

        internal FieldBuilder(AccessModifier accessModifier, string type, string name)
        {
            _field.AccessModifier = accessModifier;
            _field.Type = type;
            _field.Name = name;
        }

        internal FieldBuilder(AccessModifier accessModifier, Type type, string name)
            : this(accessModifier, type.Name, name)
        {
        }

        public FieldBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _field.AccessModifier = accessModifier;
            return this;
        }

        public FieldBuilder WithType(string type)
        {
            _field.Type = type;
            return this;
        }

        public FieldBuilder WithType(Type type)
        {
            _field.Type = type.Name;
            return this;
        }

        public FieldBuilder WithName(string name)
        {
            _field.Name = name;
            return this;
        }

        public FieldBuilder MakeReadonly()
        {
            _field.IsReadonly = true;
            return this;
        }

        public string ToSourceCode(bool formatted = true)
        {
            return _field.ToSourceCode(formatted);
        }

        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Field Build()
        {
            return _field;
        }
    }
}
