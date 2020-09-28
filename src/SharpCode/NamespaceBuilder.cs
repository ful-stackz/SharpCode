using System.Collections.Generic;
using System.Linq;
using Optional.Collections;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building namespaces. <see cref="NamespaceBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class NamespaceBuilder
    {
        private static readonly AccessModifier[] AllowedMemberAccessModifiers = new AccessModifier[]
        {
            AccessModifier.None,
            AccessModifier.Internal,
            AccessModifier.Public
        };

        private readonly Namespace _namespace = new Namespace();
        private readonly List<ClassBuilder> _classes = new List<ClassBuilder>();
        private readonly List<InterfaceBuilder> _interfaces = new List<InterfaceBuilder>();
        private readonly List<EnumBuilder> _enums = new List<EnumBuilder>();

        internal NamespaceBuilder() { }

        internal NamespaceBuilder(string name)
        {
            _namespace.Name = name;
        }

        /// <summary>
        /// Sets the name of the namespace being built.
        /// </summary>
        public NamespaceBuilder WithName(string name)
        {
            _namespace.Name = name;
            return this;
        }

        /// <summary>
        /// Adds a using directive to the namespace being built.
        /// </summary>
        /// <param name="usingDirective">
        /// The using directive to be added, for example <c>"System"</c> or <c>"System.Collections.Generic"</c>
        /// The semi-colon at the end is optional.
        /// </param>
        public NamespaceBuilder WithUsing(string usingDirective)
        {
            _namespace.Usings.Add(usingDirective.Replace(";", string.Empty));
            return this;
        }

        /// <summary>
        /// Adds a class definition to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithClass(ClassBuilder builder)
        {
            _classes.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of class definitions to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithClasses(params ClassBuilder[] builders)
        {
            _classes.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of class definitions to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithClasses(IEnumerable<ClassBuilder> builders)
        {
            _classes.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an interface definition to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithInterface(InterfaceBuilder builder)
        {
            _interfaces.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interface definitions to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithInterfaces(params InterfaceBuilder[] builders)
        {
            _interfaces.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interface definitions to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithInterfaces(IEnumerable<InterfaceBuilder> builders)
        {
            _interfaces.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an enum definition to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithEnum(EnumBuilder builder)
        {
            _enums.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of enum definitions to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithEnums(params EnumBuilder[] builders)
        {
            _enums.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of enum definitions to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithEnums(IEnumerable<EnumBuilder> builders)
        {
            _enums.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Returns the source code of the built namespace.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true)
        {
            return Build().ToSourceCode(formatted);
        }

        /// <summary>
        /// Returns the source code of the built namespace.
        /// </summary>
        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Namespace Build()
        {
            if (string.IsNullOrWhiteSpace(_namespace.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the namespace is required when building a namespace.");
            }

            _namespace.Classes.AddRange(_classes.Select(builder => builder.Build()));
            _namespace.Classes
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "A class defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            _namespace.Interfaces.AddRange(_interfaces.Select(builder => builder.Build()));
            _namespace.Interfaces
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "An interface defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            _namespace.Enums.AddRange(_enums.Select(builder => builder.Build()));
            _namespace.Enums
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "An enum defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            return _namespace;
        }
    }
}
