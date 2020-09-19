using System;
using Optional;

namespace SharpCode
{
    public class PropertyBuilder
    {
        private Property _property = new Property();

        internal PropertyBuilder() { }

        internal PropertyBuilder(AccessModifier accessModifier, string type, string name)
        {
            _property.AccessModifier = accessModifier;
            _property.Type = type;
            _property.Name = name;
        }

        internal PropertyBuilder(AccessModifier accessModifier, Type type, string name)
            : this(accessModifier, type.Name, name)
        {
        }

        public PropertyBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _property.AccessModifier = accessModifier;
            return this;
        }

        public PropertyBuilder WithType(string type)
        {
            _property.Type = type;
            return this;
        }

        public PropertyBuilder WithType(Type type)
        {
            _property.Type = type.Name;
            return this;
        }

        public PropertyBuilder WithName(string name)
        {
            _property.Name = name;
            return this;
        }

        public PropertyBuilder WithGetter(string expression)
        {
            _property.Getter = Option.Some(expression);
            return this;
        }

        public PropertyBuilder WithoutGetter()
        {
            _property.Getter = Option.None<string>();
            return this;
        }

        public PropertyBuilder WithSetter(string expression)
        {
            _property.Setter = Option.Some(expression);
            return this;
        }

        public PropertyBuilder WithoutSetter()
        {
            _property.Setter = Option.None<string>();
            return this;
        }

        public string ToSourceCode(bool formatted = true)
        {
            return Build().ToSourceCode(formatted);
        }

        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Property Build()
        {
            return _property;
        }
    }
}
