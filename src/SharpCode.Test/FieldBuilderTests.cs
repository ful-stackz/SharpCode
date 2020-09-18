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
                .WithAccessModifier(AccessModifier.Private)
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
    }
}
