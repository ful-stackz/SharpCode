using System.Collections.Generic;
using System.Linq;
using Optional;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building class structures. <see cref="ClassBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class ClassBuilder
    {
        private readonly Class _class = new Class();
        private readonly List<FieldBuilder> _fields = new List<FieldBuilder>();
        private readonly List<PropertyBuilder> _properties = new List<PropertyBuilder>();
        private readonly List<ConstructorBuilder> _constructors = new List<ConstructorBuilder>();

        internal ClassBuilder() { }

        internal ClassBuilder(AccessModifier accessModifier, string name)
        {
            _class.AccessModifier = accessModifier;
            _class.Name = name;
        }

        /// <summary>
        /// Sets the access modifier of the class being built.
        /// </summary>
        public ClassBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _class.AccessModifier = accessModifier;
            return this;
        }

        /// <summary>
        /// Sets the name of the class being built.
        /// </summary>
        public ClassBuilder WithName(string name)
        {
            _class.Name = name;
            return this;
        }

        /// <summary>
        /// Sets the class that the class being built inherits from.
        /// </summary>
        public ClassBuilder WithInheritedClass(string name)
        {
            _class.InheritedClass = Option.Some(name);
            return this;
        }

        /// <summary>
        /// Adds an interface to the list of interfaces that the class being built implements.
        /// </summary>
        public ClassBuilder WithImplementedInterface(string name)
        {
            _class.ImplementedInterfaces.Add(name);
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the class
        /// </summary>
        /// <param name="summaryDocs">
        /// The content of the summary documentation
        /// </param>
        public ClassBuilder WithSummary(string summaryDocs)
        {
            _class.Summary = Option.Some(summaryDocs);
            return this;
        }

        /// <summary>
        /// Adds a field to the class being built.
        /// </summary>
        public ClassBuilder WithField(FieldBuilder builder)
        {
            _fields.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of fields to the class being built.
        /// </summary>
        public ClassBuilder WithFields(params FieldBuilder[] builders)
        {
            _fields.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a property to the class being built.
        /// </summary>
        public ClassBuilder WithProperty(PropertyBuilder builder)
        {
            _properties.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the class being built.
        /// </summary>
        public ClassBuilder WithProperties(params PropertyBuilder[] builders)
        {
            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the class being built.
        /// </summary>
        public ClassBuilder WithProperties(IEnumerable<PropertyBuilder> builders)
        {
            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a constructor to the class being built.
        /// </summary>
        public ClassBuilder WithConstructor(ConstructorBuilder builder)
        {
            _constructors.Add(builder);
            return this;
        }

        /// <summary>
        /// Sets whether the class being built should be static or not.
        /// </summary>
        /// <param name="makeStatic">
        /// Indicates whether the class should be static or not.
        /// </param>
        public ClassBuilder MakeStatic(bool makeStatic = true)
        {
            _class.IsStatic = makeStatic;
            return this;
        }

        /// <summary>
        /// Returns the source code of the built class.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true)
        {
            return Build().ToSourceCode(formatted);
        }

        /// <summary>
        /// Returns the source code of the built class.
        /// </summary>
        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Class Build()
        {
            if (string.IsNullOrWhiteSpace(_class.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the class is required when building a class.");
            }
            else if (_class.IsStatic && _constructors.Count > 1)
            {
                throw new SyntaxException("Static classes can have only 1 constructor.");
            }

            _class.Fields.AddRange(_fields.Select(builder => builder.Build()));
            _class.Properties.AddRange(_properties.Select(builder => builder.Build()));
            _class.Constructors.AddRange(
                _constructors.Select(builder => builder
                    .WithClassName(_class.Name!)
                    .MakeStatic(_class.IsStatic)
                    .Build()));

            return _class;
        }
    }
}
