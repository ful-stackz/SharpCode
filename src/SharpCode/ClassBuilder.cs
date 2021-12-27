using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building class structures. <see cref="ClassBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class ClassBuilder
    {
        private readonly List<FieldBuilder> _fields = new List<FieldBuilder>();
        private readonly List<PropertyBuilder> _properties = new List<PropertyBuilder>();
        private readonly List<ConstructorBuilder> _constructors = new List<ConstructorBuilder>();

        internal ClassBuilder()
        {
        }

        internal ClassBuilder(AccessModifier accessModifier, string name)
        {
            Class = new Class(
                accessModifier: accessModifier,
                name: Option.Some(name));
        }

        internal Class Class { get; private set; } = new Class(AccessModifier.Public);

        /// <summary>
        /// Sets the access modifier of the class being built.
        /// </summary>
        public ClassBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            Class = Class.With(accessModifier: Option.Some(accessModifier));
            return this;
        }

        /// <summary>
        /// Sets the name of the class being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithName(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Class = Class.With(name: Option.Some(name));
            return this;
        }

        /// <summary>
        /// Sets the class that the class being built inherits from.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithInheritedClass(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Class = Class.With(inheritedClass: Option.Some(name));
            return this;
        }

        /// <summary>
        /// Adds an interface to the list of interfaces that the class being built implements.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithImplementedInterface(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Class.ImplementedInterfaces.Add(name);
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the class.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="summary"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithSummary(string summary)
        {
            if (summary is null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            Class = Class.With(summary: Option.Some(summary));
            return this;
        }

        /// <summary>
        /// Adds a field to the class being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithField(FieldBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _fields.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of fields to the class being built.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithFields(params FieldBuilder[] builders)
        {
            if (builders.Any(x => x is null))
            {
                throw new ArgumentException($"On of the {nameof(builders)} parameter values is null.");
            }

            _fields.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a property to the class being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithProperty(PropertyBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _properties.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the class being built.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithProperties(params PropertyBuilder[] builders)
        {
            if (builders.Any(x => x is null))
            {
                throw new ArgumentException($"One of the {nameof(builders)} parameter values is null.");
            }

            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of properties to the class being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithProperties(IEnumerable<PropertyBuilder> builders)
        {
            if (builders is null)
            {
                throw new ArgumentNullException(nameof(builders));
            }

            if (builders.Any(x => x is null))
            {
                throw new ArgumentException($"One of the {nameof(builders)} parameter values is null.");
            }

            _properties.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a constructor to the class being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public ClassBuilder WithConstructor(ConstructorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

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
            Class = Class.With(isStatic: Option.Some(makeStatic));
            return this;
        }

        /// <summary>
        /// Checks whether the described member exists in the class structure.
        /// </summary>
        /// <param name="name">
        /// The name of the member.
        /// </param>
        /// <param name="memberType">
        /// The type of the member. By default all members will be taken into account.
        /// </param>
        /// <param name="accessModifier">
        /// The access modifier of the member. By default all access modifiers will be taken into account.
        /// </param>
        /// <param name="comparison">
        /// The comparision type to be performed when comparing the described name against the names of the actual
        /// members. By default casing is ignored.
        /// </param>
        public bool HasMember(
            string name,
            MemberType? memberType = default,
            AccessModifier? accessModifier = default,
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (memberType == MemberType.Field)
            {
                return _fields
                    .Where(x => !accessModifier.HasValue || x.Field.AccessModifier == accessModifier)
                    .Any(x => x.Field.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (memberType == MemberType.Property)
            {
                return _properties
                    .Where(x => !accessModifier.HasValue || x.Property.AccessModifier == accessModifier)
                    .Any(x => x.Property.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (!memberType.HasValue)
            {
                // Lookup all possible members
                return HasMember(name, MemberType.Field, accessModifier, comparison) ||
                       HasMember(name, MemberType.Property, accessModifier, comparison);
            }

            return false;
        }

        /// <summary>
        /// Returns the source code of the built class.
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
        /// Returns the source code of the built class.
        /// </summary>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid class structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The class builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public override string ToString() =>
            ToSourceCode();

        internal Class Build()
        {
            if (string.IsNullOrWhiteSpace(Class.Name.ValueOrDefault()))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the class is required when building a class.");
            }
            else if (Class.IsStatic && _constructors.Count > 1)
            {
                throw new SyntaxException("Static classes can have only 1 constructor.");
            }

            Class.Fields.AddRange(_fields.Select(builder => builder.Build()));
            Class.Properties.AddRange(_properties.Select(builder => builder.Build()));
            Class.Constructors.AddRange(
                _constructors.Select(builder => builder
                    .WithName(Class.Name.ValueOrFailure())
                    .MakeStatic(Class.IsStatic)
                    .Build()));

            return Class;
        }
    }
}