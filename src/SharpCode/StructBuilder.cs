using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Collections;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building structs. <see cref="StructBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class StructBuilder
    {
        private readonly Struct _struct = new Struct();
        private readonly List<ConstructorBuilder> _constructors = new List<ConstructorBuilder>();
        private readonly List<FieldBuilder> _fields = new List<FieldBuilder>();
        private readonly List<PropertyBuilder> _properties = new List<PropertyBuilder>();

        internal StructBuilder()
        {
        }

        internal StructBuilder(string name, AccessModifier accessModifier)
        {
            _struct.Name = name;
            _struct.AccessModifier = accessModifier;
        }

        /// <summary>
        /// Sets the access modifier of the struct.
        /// </summary>
        /// <param name="accessModifier">
        /// The access modifier of the struct.
        /// </param>
        public StructBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _struct.AccessModifier = accessModifier;
            return this;
        }

        /// <summary>
        /// Sets the name of the struct.
        /// </summary>
        /// <param name="name">
        /// The name of the struct.
        /// </param>
        public StructBuilder WithName(string name)
        {
            _struct.Name = name;
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the struct.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        public StructBuilder WithSummary(string summary)
        {
            _struct.Summary = Option.Some<string?>(summary);
            return this;
        }

        /// <summary>
        /// Adds a constructor to the struct.
        /// </summary>
        /// <param name="builder">
        /// The configuration of the constructor.
        /// </param>
        public StructBuilder WithConstructor(ConstructorBuilder builder)
        {
            _constructors.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a field to the struct.
        /// </summary>
        /// <param name="builder">
        /// The configuration of the field.
        /// </param>
        public StructBuilder WithField(FieldBuilder builder)
        {
            _fields.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of fields to the struct.
        /// </summary>
        /// <param name="builders">
        /// A collection of fields.
        /// </param>
        public StructBuilder WithFields(IEnumerable<FieldBuilder> builders)
        {
            _fields.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of fields to the struct.
        /// </summary>
        /// <param name="builders">
        /// A collection of fields.
        /// </param>
        public StructBuilder WithFields(params FieldBuilder[] builders)
        {
            _fields.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a property to the struct.
        /// </summary>
        /// <param name="builder">
        /// The configuration of the property.
        /// </param>
        public StructBuilder WithProperty(PropertyBuilder builder)
        {
            _properties.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the struct.
        /// </summary>
        /// <param name="builders">
        /// A collection of properties.
        /// </param>
        public StructBuilder WithProperties(IEnumerable<PropertyBuilder> builders)
        {
            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the struct.
        /// </summary>
        /// <param name="builders">
        /// A collection of properties.
        /// </param>
        public StructBuilder WithProperties(params PropertyBuilder[] builders)
        {
            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an interface to the list of interfaces that the struct implements.
        /// </summary>
        /// <param name="name">
        /// The name of the interface that the struct implements.
        /// </param>
        public StructBuilder WithImplementedInterface(string name)
        {
            _struct.ImplementedInterfaces.Add(name);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interfaces to the list of interfaces that the struct implements.
        /// </summary>
        /// <param name="names">
        /// A collection with the names of interfaces that the struct implements.
        /// </param>
        public StructBuilder WithImplementedInterfaces(IEnumerable<string> names)
        {
            _struct.ImplementedInterfaces.AddRange(names);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interfaces to the list of interfaces that the struct implements.
        /// </summary>
        /// <param name="names">
        /// A collection with the names of interfaces that the struct implements.
        /// </param>
        public StructBuilder WithImplementedInterfaces(params string[] names)
        {
            _struct.ImplementedInterfaces.AddRange(names);
            return this;
        }

        /// <summary>
        /// Returns the source code of the built struct.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true) =>
            Build().ToSourceCode(formatted);

        /// <summary>
        /// Returns the source code of the built struct.
        /// </summary>
        public override string ToString() =>
            ToSourceCode();

        internal Struct Build()
        {
            if (string.IsNullOrWhiteSpace(_struct.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the struct is required when building a struct.");
            }

            _struct.ImplementedInterfaces
                .FirstOrNone(x => string.IsNullOrWhiteSpace(x))
                .MatchSome(_ => throw new MissingBuilderSettingException(
                    "Providing the name of the interface is required when adding an implemented interface to a struct."));

            _struct.Fields.AddRange(_fields.Select(field => field.Build()));
            _struct.Properties.AddRange(_properties.Select(prop => prop.Build()));
            _struct.Properties
                .FirstOrNone(x => x.DefaultValue.HasValue)
                .MatchSome(prop => throw new SyntaxException(
                    "Default property values are not allowed in structs. (CS0573)"));

            _struct.Constructors.AddRange(
                _constructors.Select(ctor => ctor
                    .WithName(_struct.Name!)
                    .Build()));
            _struct.Constructors
                .FirstOrNone(x => !x.Parameters.Any())
                .MatchSome(ctor => throw new SyntaxException(
                    "Explicit parameterless contructors are not allowed in structs. (CS0568)"));

            return _struct;
        }
  }
}
