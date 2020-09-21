using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class ConstructorBuilderTests
    {
        [Test]
        public void CreatingStaticConstructor_WithAccessModifier_Throws()
        {
            // @ConstructorBuilder does not expose an API for setting whether it is static.
            // Create a static class, instead, which makes the constructor static internally.
            Assert.Throws<SyntaxException>(
                () => Code.CreateClass("Test")
                    .MakeStatic()
                    .WithConstructor(Code.CreateConstructor().WithAccessModifier(AccessModifier.Public))
                    .ToSourceCode(),
                "Generating the source code for a static constructor with an access modifier should throw an exception.");
        }

        [Test]
        public void CreatingStaticConstructor_WithParameters_Throws()
        {
            // @ConstructorBuilder does not expose an API for setting whether it is static.
            // Create a static class, instead, which makes the constructor static internally.
            Assert.Throws<SyntaxException>(
                () => Code.CreateClass("Test")
                    .MakeStatic()
                    .WithConstructor(Code.CreateConstructor().WithParameter("int", "num"))
                    .ToSourceCode(),
                "Generating the source code for a static constructor with parameters should throw an exception.");
        }

        [Test]
        public void CreatingStaticConstructor_WithBaseCall_Throws()
        {
            // @ConstructorBuilder does not expose an API for setting whether it is static.
            // Create a static class, instead, which makes the constructor static internally.
            Assert.Throws<SyntaxException>(
                () => Code.CreateClass("Test")
                    .MakeStatic()
                    .WithConstructor(Code.CreateConstructor().WithBaseCall())
                    .ToSourceCode(),
                "Generating the source code for a static constructor with a base call should throw an exception.");
        }
    }
}
