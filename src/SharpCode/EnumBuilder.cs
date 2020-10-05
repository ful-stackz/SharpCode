using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Collections;

namespace SharpCode
{
    /// <summary>
    /// Provides functionalty for building enum structures. <see cref="EnumBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class EnumBuilder
    {
        private readonly Enumeration _enumeration = new Enumeration();
        private readonly List<EnumMemberBuilder> _members = new List<EnumMemberBuilder>();

        internal EnumBuilder()
        {
        }

        internal EnumBuilder(string name, AccessModifier accessModifier)
        {
            _enumeration.Name = name;
            _enumeration.AccessModifier = accessModifier;
        }

        /// <summary>
        /// Sets the access modifier of the enum being built.
        /// </summary>
        public EnumBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _enumeration.AccessModifier = accessModifier;
            return this;
        }

        /// <summary>
        /// Sets the name of the enum being built.
        /// </summary>
        public EnumBuilder WithName(string name)
        {
            _enumeration.Name = name;
            return this;
        }

        /// <summary>
        /// Adds a member to the enum being built.
        /// </summary>
        public EnumBuilder WithMember(EnumMemberBuilder builder)
        {
            _members.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a bunch of members to the enum being built.
        /// </summary>
        public EnumBuilder WithMembers(params EnumMemberBuilder[] builders)
        {
            _members.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Adds a bunch of members to the enum being built.
        /// </summary>
        public EnumBuilder WithMembers(IEnumerable<EnumMemberBuilder> builders)
        {
            _members.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Specifies whether the enum being built represents a set of flags or not. A flags enum will be marked with
        /// the <see cref="FlagsAttribute"/> in the generated source code. The members of a flags enum will be assigned
        /// appropriate, auto-generated values. <b>Note</b> that explicitly set values will be overwritten.
        /// </summary>
        /// <param name="makeFlagsEnum">
        /// Indicates whether the enum represents a set of flags.
        /// </param>
        public EnumBuilder MakeFlagsEnum(bool makeFlagsEnum = true)
        {
            _enumeration.IsFlag = makeFlagsEnum;
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the enum.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        public EnumBuilder WithSummary(string summary)
        {
            _enumeration.Summary = Option.Some<string?>(summary);
            return this;
        }

        /// <summary>
        /// Returns the source code of the built enum.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true) =>
            Build().ToSourceCode(formatted);

        /// <summary>
        /// Returns the source code of the built enum.
        /// </summary>
        public override string ToString() =>
            ToSourceCode();

        internal Enumeration Build()
        {
            if (string.IsNullOrWhiteSpace(_enumeration.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the enum is required when building an enum.");
            }

            _enumeration.Members.AddRange(_members.Select(x => x.Build()));
            _enumeration.Members
                .GroupBy(x => x.Name)
                .Where(x => x.AtLeast(2))
                .Select(x => x.Key)
                .FirstOrNone()
                .MatchSome(duplicateMemberName => throw new SyntaxException(
                    $"The enum '{_enumeration.Name}' already contains a definition for '{duplicateMemberName}'."));

            if (_enumeration.IsFlag && _enumeration.Members.All(x => !x.Value.HasValue))
            {
                for (var i = 0; i < _enumeration.Members.Count; i++)
                {
                    _enumeration.Members[i].Value = Option.Some(i == 0 ? 0 : (int)Math.Pow(2, i - 1));
                }
            }

            return _enumeration;
        }
    }
}
