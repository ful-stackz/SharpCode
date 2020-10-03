using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class EnumBuilderTests
    {
        [Test]
        public void CreatingEnum_Works()
        {
            var generatedCode = Code.CreateEnum()
                .WithAccessModifier(AccessModifier.Internal)
                .WithName("UserStatus")
                .WithMember(Code.CreateEnumMember("Inactive"))
                .WithMember(Code.CreateEnumMember("Active"))
                .WithMembers(
                    Code.CreateEnumMember("Blocked"),
                    Code.CreateEnumMember("NotConfirmed"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
internal enum UserStatus
{
    Inactive,
    Active,
    Blocked,
    NotConfirmed,
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }
         
        [Test]
        public void CreatingEnumWithDocumentation_Works()
        {
            var generatedCode = Code.CreateEnum()
                .WithAccessModifier(AccessModifier.Internal)
                .WithName("UserStatus")
                .WithMember(Code.CreateEnumMember("Inactive").WithDocumentation("The state of the user when they are inactive"))
                .WithMember(Code.CreateEnumMember("Active").WithDocumentation("The user is active\nLike, all the time"))
                .WithMember(Code.CreateEnumMember("Asleep"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
internal enum UserStatus
{
    /// <summary>
    /// The state of the user when they are inactive
    /// </summary>
    Inactive,
    /// <summary>
    /// The user is active
    /// Like, all the time
    /// </summary>
    Active,
    Asleep,
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }


        [Test]
        public void CreatingFlagsEnum_Works()
        {
            var generatedCode = Code.CreateEnum()
                .MakeFlagsEnum()
                .WithAccessModifier(AccessModifier.Public)
                .WithName("Colors")
                .WithMember(Code.CreateEnumMember("Red"))
                .WithMembers(
                    Code.CreateEnumMember("Green"),
                    Code.CreateEnumMember("Blue"),
                    Code.CreateEnumMember("Orange"),
                    Code.CreateEnumMember("Black"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
[System.Flags]
public enum Colors
{
    Red = 0,
    Green = 1,
    Blue = 2,
    Orange = 4,
    Black = 8,
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingEnum_WithCustomValues()
        {
            var generatedCode = Code.CreateEnum(name: "Color")
                .WithMember(Code.CreateEnumMember("None", 0))
                .WithMember(Code.CreateEnumMember("Red", 100))
                .WithMember(Code.CreateEnumMember("Green", 222))
                .WithMember(Code.CreateEnumMember("Blue", 404))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public enum Color
{
    None = 0,
    Red = 100,
    Green = 222,
    Blue = 404,
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingFlagsEnum_WithExplicitMemberValues_DoesNotGenerateValues()
        {
            var generatedCode = Code.CreateEnum(name: "Storage")
                .WithMember(Code.CreateEnumMember("None", 0))
                .WithMember(Code.CreateEnumMember("HardDrive", 1))
                .WithMember(Code.CreateEnumMember("SolidStateDrive", 2))
                .WithMember(Code.CreateEnumMember("FlashStick"))
                .WithMember(Code.CreateEnumMember("ExternalHardDrive", 4))
                .MakeFlagsEnum()
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
[System.Flags]
public enum Storage
{
    None = 0,
    HardDrive = 1,
    SolidStateDrive = 2,
    FlashStick,
    ExternalHardDrive = 4,
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingEnum_WithMissingRequiredSettings_Throws()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum().ToSourceCode(),
                "Generating the source code for an enum without setting the name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum(name: null).ToSourceCode(),
                "Generating the source for an enum with null as a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum().WithName(null).ToSourceCode(),
                "Generating the source for an enum with null as a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum(name: string.Empty).ToSourceCode(),
                "Generating the source for an enum with an empty name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum().WithName(string.Empty).ToSourceCode(),
                "Generating the source for an enum with an empty name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum(name: "  ").ToSourceCode(),
                "Generating the source for an enum with a whitespace name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateEnum().WithName("  ").ToSourceCode(),
                "Generating the source for an enum with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreatingEnum_WithDuplicateMembers_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateEnum(name: "Test").WithMembers(
                    Code.CreateEnumMember("Duplicate"),
                    Code.CreateEnumMember("Duplicate"))
                    .ToSourceCode(),
                "Generating the source code for an enum with duplicate values should throw an exception.");
        }
    }
}
