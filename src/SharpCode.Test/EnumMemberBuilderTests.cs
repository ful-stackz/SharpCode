using System;
using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class EnumMemberBuilderTests
    {
        [Test]
        public void CreatingEnumMember_WithoutRequiredSettings_Throws()
        {
            // --- WithName() API
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnumMember().Build(),
                "Generating the source code for an enum member without setting the name should throw an exception.");
        }

        [Test]
        public void CreatingEnumMember_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateEnumMember().WithSummary(null),
                "Generating the source code for an enum member with null as summary should throw an exception.");
        }

        [Test]
        public void CreateEnumMember_WithInvalidName_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateEnumMember().WithName(null),
                "Generating the source code for an enum member with null as name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateEnumMember().WithName(string.Empty),
                "Generating the source code for an enum member with an empty string as name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateEnumMember().WithName("  "),
                "Generating the source code for an enum member with whitespace name should throw an exception.");
        }
    }
}
