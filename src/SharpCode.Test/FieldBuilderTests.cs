using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class FieldBuilderTests
    {
        [Test]
        public void CreateField_Works_WithDefaults()
        {
            var generatedCode = Code.CreateField()
                .WithType(typeof(int))
                .WithName("_username")
                .MakeReadonly()
                .ToSourceCode();

            const string ExpectedCode = "private readonly Int32 _username;";

            Assert.AreEqual(ExpectedCode, generatedCode);
        }

        [Test]
        public void CreateField_Works_WithChainedMethods()
        {
            var generatedCode = Code.CreateField()
                .WithAccessModifier(AccessModifier.Private)
                .WithType(typeof(int))
                .WithName("_username")
                .MakeReadonly()
                .ToSourceCode();

            const string ExpectedCode = "private readonly Int32 _username;";

            Assert.AreEqual(ExpectedCode, generatedCode);
        }

        [Test]
        public void CreateField_Throws_WhenRequiredSettingsNotProvided()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithType("string").ToSourceCode(),
                "Expected generating the source code for a field without setting the name to throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName("_iHaveNoType").ToSourceCode(),
                "Expected generating the source code for a field without setting the type to throw an exception.");
        }
    }
}
