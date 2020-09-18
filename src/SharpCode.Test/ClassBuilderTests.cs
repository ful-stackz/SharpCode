using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class ClassBuilderTests
    {
        [Test]
        public void ClassBuilderBuildsSimpleClass()
        {
            var generatedCode = Code.CreateClass("User")
                .WithNamespace("Data")
                .ToSourceCode(formatted: true)
                .WithUnixEOL();

            var ExpectedCode = @"
namespace Data
{
    public class User
    {
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(ExpectedCode, generatedCode);
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
                .WithUnixEOL();

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
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_Works_WithFieldsAndProperties()
        {
            var generatedCode = Code.CreateClass("Triangle", AccessModifier.Public)
                .WithNamespace("Shapes")
                .WithField(Code.CreateField("double", "_hypotenuse"))
                .WithField(Code.CreateField("double", "_adjacent"))
                .WithField(Code.CreateField("double", "_opposite"))
                .WithProperty(Code.CreateProperty("double", "Hypotenuse")
                    .WithGetter("_hypotenuse")
                    .WithSetter("_hypotenuse = value"))
                .WithProperty(Code.CreateProperty("double", "Adjacent")
                    .WithGetter("_adjacent")
                    .WithSetter("_adjacent = value"))
                .WithProperty(Code.CreateProperty("double", "Opposite")
                    .WithGetter("_opposite")
                    .WithSetter("_opposite = value"))
                .ToSourceCode()
                .WithUnixEOL();

            string expectedCode = @"
namespace Shapes
{
    public class Triangle
    {
        private double _hypotenuse;
        private double _adjacent;
        private double _opposite;
        public double Hypotenuse
        {
            get => _hypotenuse;
            set => _hypotenuse = value;
        }

        public double Adjacent
        {
            get => _adjacent;
            set => _adjacent = value;
        }

        public double Opposite
        {
            get => _opposite;
            set => _opposite = value;
        }
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}
