using System;
using Optional;
using Optional.Unsafe;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building enum members. <see cref="EnumMemberBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class EnumMemberBuilder
    {
        internal EnumMemberBuilder()
        {
        }

        internal EnumMemberBuilder(string name, Option<int> value)
        {
            EnumerationMember = new EnumerationMember(
                name: Option.Some(name),
                value: value);
        }

        internal EnumerationMember EnumerationMember { get; private set; } = new EnumerationMember(
            name: Option.None<string>());

        /// <summary>
        /// Sets the name of the enum member.
        /// </summary>
        /// <param name="name">
        /// The name of the member. Used as-is.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the specified <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the specified <paramref name="name"/> is empty or invalid.
        /// </exception>
        public EnumMemberBuilder WithName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The enum member name must be a valid, non-empty string.", nameof(name));

            EnumerationMember = EnumerationMember.With(name: Option.Some(name));
            return this;
        }

        /// <summary>
        /// Specifies whether the enum member has an explicit value, and the value itself.
        /// </summary>
        /// <param name="value">
        /// Use <c>null</c> to specify that the enum member should not have an explicit value (default).
        /// Otherwise provide the value of the enum member.
        /// </param>
        public EnumMemberBuilder WithValue(int? value = null)
        {
            EnumerationMember = EnumerationMember.With(value: value.ToOption());
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the enum member.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the specified <paramref name="summary"/> is <c>null</c>.
        /// </exception>
        public EnumMemberBuilder WithSummary(string summary)
        {
            if (summary is null)
                throw new ArgumentNullException(nameof(summary));

            EnumerationMember = EnumerationMember.With(summary: Option.Some(summary));
            return this;
        }

        internal EnumerationMember Build()
        {
            if (string.IsNullOrWhiteSpace(EnumerationMember.Name.ValueOrDefault()))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the enum member is required when building an enum.");
            }

            return EnumerationMember;
        }
    }
}
