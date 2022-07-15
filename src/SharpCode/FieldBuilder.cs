using System;
using Optional;
using Optional.Unsafe;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building fields. <see cref="FieldBuilder"/> instances are <b>not</b> immutable.
    /// </summary>
    public class FieldBuilder
    {
        internal FieldBuilder()
        {
        }

        internal FieldBuilder(AccessModifier accessModifier, string type, string name)
        {
            Field = new Field(
                accessModifier: accessModifier,
                name: Option.Some(name),
                type: Option.Some(type));
        }

        internal FieldBuilder(AccessModifier accessModifier, Type type, string name)
            : this(accessModifier, type.Name, name)
        {
        }

        internal Field Field { get; private set; } = new Field(AccessModifier.Private);

        /// <summary>
        /// Sets accessibilty modifier of the field being built.
        /// </summary>
        public FieldBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            Field = Field.With(accessModifier: Option.Some(accessModifier));
            return this;
        }

        /// <summary>
        /// Sets the type of the field being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public FieldBuilder WithType(string type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Field = Field.With(type: Option.Some(type));
            return this;
        }

        /// <summary>
        /// Sets the type of the field being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public FieldBuilder WithType(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Field = Field.With(type: Option.Some(type.Name));
            return this;
        }

        /// <summary>
        /// Sets the name of the field being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public FieldBuilder WithName(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Field = Field.With(name: Option.Some(name));
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
            Field = Field.With(isReadonly: Option.Some(makeReadonly));
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the field.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="summary"/> is <c>null</c>.
        /// </exception>
        public FieldBuilder WithSummary(string summary)
        {
            if (summary is null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            Field = Field.With(summary: Option.Some(summary));
            return this;
        }

        /// <summary>
        /// Returns the source code of the built field.
        /// </summary>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid field structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The field builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public string ToSourceCode() =>
            Ast.Stringify(Ast.FromDefinition(Build()));

        /// <summary>
        /// Returns the source code of the built field.
        /// </summary>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid field structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The field builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public override string ToString() =>
            ToSourceCode();

        internal Field Build()
        {
            if (string.IsNullOrWhiteSpace(Field.Type.ValueOrDefault()))
            {
                throw new MissingBuilderSettingException(
                    "Providing the type of the field is required when building a field.");
            }
            else if (string.IsNullOrWhiteSpace(Field.Name.ValueOrDefault()))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the field is required when building a field.");
            }

            return Field;
        }
    }
}
