using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class InterfaceBuilderTests
    {
        [Test]
        public void CreatingInterface_WithoutRequiredSettings_Throws()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateInterface().ToSourceCode(),
                "Generating the source code for an interface without a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateInterface().WithName(null).ToSourceCode(),
                "Generating the source code for an interface with null as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateInterface().WithName(string.Empty).ToSourceCode(),
                "Generating the source code for an interface with an empty string as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateInterface().WithName("   ").ToSourceCode(),
                "Generating the source code for an interface with whitespace as name should throw an exception.");
        }

        [Test]
        public void CreatingInterface_ValidatesPropertiesDefinitions()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithDefaultValue("42"))
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that has a default value should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithGetter("_value"))
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that has " +
                    "a non auto implemented getter should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithSetter("_value = value"))
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that has " +
                    "a non auto implemented setter should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithoutGetter().WithoutSetter())
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that does not have " +
                    "a getter nor a setter should throw an exception.");
        }

        [Test]
        public void CreatingInterface_Works()
        {
            var generatedCode = Code.CreateInterface("ITest").ToSourceCode().WithUnixEOL();

            var expectedCode = @"
public interface ITest
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingInterface_WithImplementedInterfaces_Works()
        {
            var generatedCode = Code.CreateInterface("ITest")
                .WithImplementedInterface("IHaveTests")
                .WithImplementedInterface("IFailNot")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface ITest : IHaveTests, IFailNot
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingInterface_WithDifferentAccessModifiers_Works()
        {
            var expectedCode = @"
{access-modifier} interface ITest
{
}
            ".WithUnixEOL();

            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", string.Empty).Trim(),
                Code.CreateInterface("ITest", AccessModifier.None).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "private").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Private).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "private protected").Trim(),
                Code.CreateInterface("ITest", AccessModifier.PrivateProtected).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "internal").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Internal).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "protected").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Protected).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "protected internal").Trim(),
                Code.CreateInterface("ITest", AccessModifier.ProtectedInternal).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "public").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Public).ToSourceCode().WithUnixEOL());
        }

        [Test]
        public void CreatingInterface_WithProperties_Works()
        {
            var generatedCode = Code.CreateInterface("ITest")
                .WithProperty(Code.CreateProperty("int", "Number"))
                .WithProperty(Code.CreateProperty("string", "Stringy"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface ITest
{
    int Number
    {
        get;
        set;
    }

    string Stringy
    {
        get;
        set;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingInterface_WithSummary_Works()
        {
            var generatedCode = Code.CreateInterface()
                .WithAccessModifier(AccessModifier.Public)
                .WithName("ISerializable")
                .WithSummary("Allows an object to control its own serialization and deserialization.")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
/// <summary>
/// Allows an object to control its own serialization and deserialization.
/// </summary>
public interface ISerializable
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}
