using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
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
            AccessModifier.Public,
        };

        private readonly List<ClassBuilder> _classes = new List<ClassBuilder>();
        private readonly List<StructBuilder> _structs = new List<StructBuilder>();
        private readonly List<InterfaceBuilder> _interfaces = new List<InterfaceBuilder>();
        private readonly List<EnumBuilder> _enums = new List<EnumBuilder>();

        internal NamespaceBuilder()
        {
        }

        internal NamespaceBuilder(string name)
        {
            Namespace = new Namespace(name: Option.Some(name));
        }

        internal Namespace Namespace { get; private set; } = new Namespace(name: Option.None<string>());

        /// <summary>
        /// Sets the name of the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithName(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Namespace = Namespace.With(name: Option.Some(name));
            return this;
        }

        /// <summary>
        /// Adds a using directive to the namespace being built.
        /// </summary>
        /// <param name="usingDirective">
        /// The using directive to be added, for example <c>"System"</c> or <c>"System.Collections.Generic"</c>
        /// The semi-colon at the end is optional.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="usingDirective"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithUsing(string usingDirective)
        {
            if (usingDirective is null)
            {
                throw new ArgumentNullException(nameof(usingDirective));
            }

            Namespace.Usings.Add(usingDirective.Replace(";", string.Empty));
            return this;
        }

        /// <summary>
        /// Adds a class to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithClass(ClassBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _classes.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of classes to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithClasses(params ClassBuilder[] builders)
        {
            if (builders.Any(x => x is null))
            {
                throw new ArgumentException($"One of the {nameof(builders)} parameter values is null.");
            }

            _classes.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of classes to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithClasses(IEnumerable<ClassBuilder> builders)
        {
            if (builders is null)
            {
                throw new ArgumentNullException(nameof(builders));
            }

            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _classes.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an interface definition to the namespace being built.
        /// </summary>
        /// <param name="builder">
        /// The configuration of the interface.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithInterface(InterfaceBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _interfaces.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interface definitions to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithInterfaces(params InterfaceBuilder[] builders)
        {
            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _interfaces.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of interface definitions to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithInterfaces(IEnumerable<InterfaceBuilder> builders)
        {
            if (builders is null)
            {
                throw new ArgumentNullException(nameof(builders));
            }

            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _interfaces.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds an enum definition to the namespace being built.
        /// </summary>
        /// <param name="builder">
        /// The configuration of the enum.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithEnum(EnumBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _enums.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of enum definitions to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithEnums(params EnumBuilder[] builders)
        {
            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _enums.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of enum definitions to the namespace being built.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithEnums(IEnumerable<EnumBuilder> builders)
        {
            if (builders is null)
            {
                throw new ArgumentNullException(nameof(builders));
            }

            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _enums.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a struct to the namespace.
        /// </summary>
        /// <param name="builder">
        /// The configuration of the struct.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithStruct(StructBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _structs.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of structs to the namespace.
        /// </summary>
        /// <param name="builders">
        /// A collection of structs which will be added to the namespace.
        /// </param>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithStructs(params StructBuilder[] builders)
        {
            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _structs.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of structs to the namespace.
        /// </summary>
        /// <param name="builders">
        /// A collection of structs which will be added to the namespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the specified <paramref name="builders"/> is <c>null</c>.
        /// </exception>
        public NamespaceBuilder WithStructs(IEnumerable<StructBuilder> builders)
        {
            if (builders is null)
            {
                throw new ArgumentNullException(nameof(builders));
            }

            if (builders.Any(x => x is null))
            {
                throw new ArgumentException("One of the builders is null.");
            }

            _structs.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Checks whether the described member exists in the namespace structure.
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
            if (memberType == MemberType.Class)
            {
                return _classes
                    .Where(x => !accessModifier.HasValue || x.Class.AccessModifier == accessModifier)
                    .Any(x => x.Class.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (memberType == MemberType.Enum)
            {
                return _enums
                    .Where(x => !accessModifier.HasValue || x.Enumeration.AccessModifier == accessModifier)
                    .Any(x => x.Enumeration.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (memberType == MemberType.Interface)
            {
                return _interfaces
                    .Where(x => !accessModifier.HasValue || x.Interface.AccessModifier == accessModifier)
                    .Any(x => x.Interface.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (memberType == MemberType.Struct)
            {
                return _structs
                    .Where(x => !accessModifier.HasValue || x.Struct.AccessModifier == accessModifier)
                    .Any(x => x.Struct.Name.Exists(n => n.Equals(name, comparison)));
            }

            if (!memberType.HasValue)
            {
                return HasMember(name, MemberType.Class, accessModifier, comparison) ||
                    HasMember(name, MemberType.Enum, accessModifier, comparison) ||
                    HasMember(name, MemberType.Interface, accessModifier, comparison) ||
                    HasMember(name, MemberType.Struct, accessModifier, comparison);
            }

            return false;
        }

        /// <summary>
        /// Returns the source code of the built namespace.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid class structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The class builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public string ToSourceCode(bool formatted = true) =>
            Build().ToSourceCode(formatted);

        /// <summary>
        /// Returns the source code of the built namespace.
        /// </summary>
        /// <exception cref="MissingBuilderSettingException">
        /// A setting that is required to build a valid class structure is missing.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// The class builder is configured in such a way that the resulting code would be invalid.
        /// </exception>
        public override string ToString() =>
            ToSourceCode();

        internal Namespace Build()
        {
            if (string.IsNullOrWhiteSpace(Namespace.Name.ValueOr(string.Empty)))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the namespace is required when building a namespace.");
            }

            Namespace.Classes.AddRange(_classes.Select(builder => builder.Build()));
            Namespace.Classes
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "A class defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            Namespace.Structs.AddRange(_structs.Select(builder => builder.Build()));
            Namespace.Structs
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "A struct defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            Namespace.Interfaces.AddRange(_interfaces.Select(builder => builder.Build()));
            Namespace.Interfaces
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "An interface defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            Namespace.Enums.AddRange(_enums.Select(builder => builder.Build()));
            Namespace.Enums
                .FirstOrNone(item => !AllowedMemberAccessModifiers.Contains(item.AccessModifier))
                .MatchSome(item => throw new SyntaxException(
                    "An enum defined under a namespace cannot have the access modifier " +
                    $"'{item.AccessModifier.ToSourceCode()}'."));

            return Namespace;
        }
    }
}
