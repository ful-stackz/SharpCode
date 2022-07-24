using System;
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
        private readonly List<PropertyBuilder> _properties = new List<PropertyBuilder>();
        private readonly List<TypeParameterBuilder> _typeParameters = new List<TypeParameterBuilder>();

        internal InterfaceBuilder()
        {
        }

        internal InterfaceBuilder(string name, AccessModifier accessModifier)
        {
            Interface = new Interface(accessModifier, Option.Some(name));
        }

        internal Interface Interface { get; private set; } = new Interface(AccessModifier.Public);

        /// <summary>
        /// Sets the access modifier of the interface being built.
        /// </summary>
        public InterfaceBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            Interface = Interface.With(accessModifier: Option.Some(accessModifier));
            return this;
        }

        /// <summary>
        /// Sets the name of the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="name"/> is empty or invalid.
        /// </exception>
        public InterfaceBuilder WithName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Interface name must be a valid, non-empty string.", nameof(name));

            Interface = Interface.With(name: Option.Some(name));
            return this;
        }

        /// <summary>
        /// Adds a property to the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithProperty(PropertyBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            _properties.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the builders provided with <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithProperties(params PropertyBuilder[] builders)
        {
            if (builders is null)
                throw new ArgumentNullException(nameof(builders));

            if (builders.Any(x => x is null))
                throw new ArgumentException("One of the provided property builders is null.", nameof(builders));

            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the builders provided with the <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithProperties(IEnumerable<PropertyBuilder> builders)
        {
            if (builders is null)
                throw new ArgumentNullException(nameof(builders));

            if (builders.Any(x => x is null))
                throw new ArgumentException("One of the property builders is null.", nameof(builders));

            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an interface to the list of interfaces that the interface implements.
        /// </summary>
        /// <param name="name">
        /// The name of the interface that the interface implements.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="name"/> is empty or invalid.
        /// </exception>
        public InterfaceBuilder WithImplementedInterface(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Implemented interface must be a valid, non-empty string.", nameof(name));

            Interface.ImplementedInterfaces.Add(name);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interfaces to the list of interfaces that the interface implements.
        /// </summary>
        /// <param name="names">
        /// A collection with the names of interfaces that the interface implements.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="names"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="names"/> is <c>null</c> or empty.
        /// </exception>
        public InterfaceBuilder WithImplementedInterfaces(IEnumerable<string> names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));

            if (names.Any(x => string.IsNullOrWhiteSpace(x)))
                throw new ArgumentException("One of the provided names is null or empty.", nameof(names));

            Interface.ImplementedInterfaces.AddRange(names);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interfaces to the list of interfaces that the interface implements.
        /// </summary>
        /// <param name="names">
        /// A collection with the names of interfaces that the interface implements.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="names"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="names"/> is <c>null</c> or empty.
        /// </exception>
        public InterfaceBuilder WithImplementedInterfaces(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));

            if (names.Any(x => string.IsNullOrWhiteSpace(x)))
                throw new ArgumentException("One of the provided names is null or empty.", nameof(names));

            Interface.ImplementedInterfaces.AddRange(names);
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the interface.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="summary"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithSummary(string summary)
        {
            if (summary is null)
                throw new ArgumentNullException(nameof(summary));

            Interface = Interface.With(summary: Option.Some(summary));
            return this;
        }

        /// <summary>
        /// Adds a type parameter to the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithTypeParameter(TypeParameterBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            _typeParameters.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of type parameters to the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithTypeParameters(params TypeParameterBuilder[] builders)
        {
            if (builders is null)
                throw new ArgumentNullException(nameof(builders));

            if (builders.Any(x => x is null))
                throw new ArgumentException("One of the type parameter builders is null.", nameof(builders));

            _typeParameters.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of type parameters to the interface being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public InterfaceBuilder WithTypeParameters(IEnumerable<TypeParameterBuilder> builders)
        {
            if (builders is null)
                throw new ArgumentNullException(nameof(builders));

            if (builders.Any(x => x is null))
                throw new ArgumentException("One of the type parameter builders is null.", nameof(builders));

            _typeParameters.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Checks whether the described member exists in the interface structure.
        /// </summary>
        /// <param name="name">
        /// The name of the member.
        /// </param>
        /// <param name="memberType">
        /// The type of the member. By default all members will be taken into account.
        /// </param>
        /// <param name="comparison">
        /// The comparision type to be performed when comparing the described name against the names of the actual
        /// members. By default casing is ignored.
        /// </param>
        public bool HasMember(
            string name,
            MemberType? memberType = default,
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (memberType == MemberType.Property)
            {
                return _properties.Any(x => x.Property.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (!memberType.HasValue)
            {
                return HasMember(name, MemberType.Property, comparison);
            }

            return false;
        }

        /// <summary>
        /// Returns the source code of the built interface.
        /// </summary>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid class structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The class builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public string ToSourceCode() =>
            Ast.Stringify(Ast.FromDefinition(Build()));

        /// <summary>
        /// Returns the source code of the built interface.
        /// </summary>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid class structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The class builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public override string ToString() =>
            ToSourceCode();

        internal Interface Build()
        {
            if (string.IsNullOrWhiteSpace(Interface.Name.ValueOr(string.Empty)))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the interface is required when building an interface.");
            }

            Interface.Properties.AddRange(
                _properties.Select(builder => builder
                    .WithAccessModifier(AccessModifier.None)
                    .Build()));
            if (Interface.Properties.Any(prop => prop.DefaultValue.HasValue))
            {
                throw new SyntaxException("Interface properties cannot have a default value. (CS8053)");
            }
            else if (Interface.Properties.Any(prop => !prop.Getter.HasValue && !prop.Setter.HasValue))
            {
                throw new SyntaxException("Interface properties should have at least a getter or a setter.");
            }
            else if (Interface.Properties.Any(prop => prop.Getter.Exists(expr => !expr.Equals(Property.AutoGetterSetter))))
            {
                throw new SyntaxException("Interface properties can only define an auto implemented getter.");
            }
            else if (Interface.Properties.Any(prop => prop.Setter.Exists(expr => !expr.Equals(Property.AutoGetterSetter))))
            {
                throw new SyntaxException("Interface properties can only define an auto implemented setter.");
            }

            Interface.TypeParameters.AddRange(_typeParameters.Select(builder => builder.Build()));

            return Interface;
        }
    }
}
