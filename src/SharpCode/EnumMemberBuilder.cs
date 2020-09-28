using Optional;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building enum members. <see cref="EnumMemberBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class EnumMemberBuilder
    {
        private readonly EnumerationMember _enumMember = new EnumerationMember();

        internal EnumMemberBuilder() { }

        internal EnumMemberBuilder(string name, Option<int> value)
        {
            _enumMember.Name = name;
            _enumMember.Value = value;
        }

        /// <summary>
        /// Sets the name of the enum member.
        /// </summary>
        /// <param name="name">
        /// The name of the member. Used as-is.
        /// </param>
        public EnumMemberBuilder WithName(string name)
        {
            _enumMember.Name = name;
            return this;
        }

        /// <summary>
        /// Specifies whether the enum member has an explicit value, and the value itself.
        /// </summary>
        /// <param name="value">
        /// Use <c>null</c> to specify that the enum member should not have an explicit value. Otherwise provide the
        /// value of the enum member.
        /// </param>
        public EnumMemberBuilder WithValue(int? value)
        {
            _enumMember.Value = value.ToOption();
            return this;
        }

        internal EnumerationMember Build()
        {
            if (string.IsNullOrWhiteSpace(_enumMember.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the enum member is required when building an enum.");
            }

            return _enumMember;
        }
    }
}
