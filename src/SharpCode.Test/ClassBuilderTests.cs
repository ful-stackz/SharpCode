using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class ClassBuilderTests
    {
        [Test]
        public void ClassBuilderBuildsSimpleClass()
        {
            var code = Code.CreateClass("User")
                .WithNamespace("Data")
                .ToSourceCode(formatted: true);
            const string ExpectedCode = @"
namespace Data
{
    public class User
    {
    }
}
            ";

            Assert.AreEqual(ExpectedCode.Trim().Replace("\r\n", "\n"), code.Replace("\r\n", "\n"));
        }

        [Test]
        public void BuildsInternalClassWithFields()
        {
            var generatedCode = Code.CreateClass("Triangle", AccessModifier.Internal)
                .WithNamespace("Shapes")
                .WithField(Code.CreateField("int", "_hypotenuse"))
                .WithField(Code.CreateField("double", "_adjacent"))
                .WithField(Code.CreateField("float", "_opposite"))
                .ToSourceCode(formatted: true)
                .Replace("\r\n", "\n");

            string expectedCode = @"
namespace Shapes
{
    internal class Triangle
    {
        private int _hypotenuse;
        private double _adjacent;
        private float _opposite;
    }
}
            ".Trim().Replace("\r\n", "\n");

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}
