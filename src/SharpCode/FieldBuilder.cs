using System;
using Optional;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building fields. <see cref="FieldBuilder"/> instances are <b>not</b> immutable.
    /// </summary>
    public class FieldBuilder
    {
        private readonly Field _field = new Field();

        internal FieldBuilder()
        {
        }

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

        /// <summary>
        /// Sets accessibilty modifier of the field being built.
        /// </summary>
        public FieldBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _field.AccessModifier = accessModifier;
            return this;
        }

        /// <summary>
        /// Sets the type of the field being built.
        /// </summary>
        public FieldBuilder WithType(string type)
        {
            _field.Type = type;
            return this;
        }

        /// <summary>
        /// Sets the type of the field being built.
        /// </summary>
        public FieldBuilder WithType(Type type)
        {
            _field.Type = type.Name;
            return this;
        }

        /// <summary>
        /// Sets the name of the field being built.
        /// </summary>
        public FieldBuilder WithName(string name)
        {
            _field.Name = name;
            return this;
        }

        /// <summary>
        /// Sets the readonly preference for the field being built.
        /// </summary>
        /// <param name="makeReadonly">
        /// Indicates whether the field will be made readonly (<c>true</c>) or not (<c>false</c>).
        /// </param>
        public FieldBuilder MakeReadonly(bool makeReadonly = true)
        {
            _field.IsReadonly = makeReadonly;
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the field.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        public FieldBuilder WithSummary(string summary)
        {
            _field.Summary = Option.Some(summary);
            return this;
        }

        /// <summary>
        /// Returns the source code of the built field.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true)
        {
            return Build().ToSourceCode(formatted);
        }

        /// <summary>
        /// Returns the source code of the built field.
        /// </summary>
        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Field Build()
        {
            if (string.IsNullOrWhiteSpace(_field.Type))
            {
                throw new MissingBuilderSettingException(
                    "Providing the type of the field is required when building a field.");
            }
            else if (string.IsNullOrWhiteSpace(_field.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the field is required when building a field.");
            }

            return _field;
        }
    }
}
