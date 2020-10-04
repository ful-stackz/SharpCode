using System.Collections.Generic;
using System.Linq;
using Optional;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building interface structures. <see cref="InterfaceBuilder"/> instances are
    /// <b>not</b> immutable.
    /// </summary>
    public class InterfaceBuilder
    {
        private readonly Interface _interface = new Interface();
        private readonly List<PropertyBuilder> _properties = new List<PropertyBuilder>();

        internal InterfaceBuilder()
        {
        }

        internal InterfaceBuilder(string name, AccessModifier accessModifier)
        {
            _interface.Name = name;
            _interface.AccessModifier = accessModifier;
        }

        /// <summary>
        /// Sets the access modifier of the interface being built.
        /// </summary>
        public InterfaceBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _interface.AccessModifier = accessModifier;
            return this;
        }

        /// <summary>
        /// Sets the name of the interface being built.
        /// </summary>
        public InterfaceBuilder WithName(string name)
        {
            _interface.Name = name;
            return this;
        }

        /// <summary>
        /// Adds a property to the interface being built.
        /// </summary>
        public InterfaceBuilder WithProperty(PropertyBuilder builder)
        {
            _properties.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the interface being built.
        /// </summary>
        public InterfaceBuilder WithProperties(params PropertyBuilder[] builders)
        {
            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the interface being built.
        /// </summary>
        public InterfaceBuilder WithProperties(IEnumerable<PropertyBuilder> builders)
        {
            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an interface to the list of interfaces that the interface being built implements.
        /// </summary>
        public InterfaceBuilder WithImplementedInterface(string name)
        {
            _interface.ImplementedInterfaces.Add(name);
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the interface.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        public InterfaceBuilder WithSummary(string summary)
        {
            _interface.Summary = Option.Some(summary);
            return this;
        }

        /// <summary>
        /// Returns the source code of the built interface.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true) =>
            Build().ToSourceCode(formatted);

        /// <summary>
        /// Returns the source code of the built interface.
        /// </summary>
        public override string ToString() =>
            ToSourceCode();

        internal Interface Build()
        {
            if (string.IsNullOrWhiteSpace(_interface.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the interface is required when building an interface.");
            }

            _interface.Properties.AddRange(
                _properties.Select(builder => builder
                    .WithAccessModifier(AccessModifier.None)
                    .Build()));
            if (_interface.Properties.Any(prop => prop.DefaultValue.HasValue))
            {
                throw new SyntaxException("Interface properties cannot have a default value. (CS8053)");
            }
            else if (_interface.Properties.Any(prop => !prop.Getter.HasValue && !prop.Setter.HasValue))
            {
                throw new SyntaxException("Interface properties should have at least a getter or a setter.");
            }
            else if (_interface.Properties.Any(prop => prop.Getter.Exists(expr => !string.IsNullOrWhiteSpace(expr))))
            {
                throw new SyntaxException("Interface properties can only define an auto implemented getter.");
            }
            else if (_interface.Properties.Any(prop => prop.Setter.Exists(expr => !string.IsNullOrWhiteSpace(expr))))
            {
                throw new SyntaxException("Interface properties can only define an auto implemented setter.");
            }

            return _interface;
        }
    }
}
