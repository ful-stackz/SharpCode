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

        [Test]
        public void CreatingConstructor_WithSummary_Works()
        {
            var generatedCode = Code.CreateClass()
                .WithAccessModifier(AccessModifier.Public)
                .WithName("Test")
                .WithConstructor(Code.CreateConstructor(AccessModifier.Public)
                    .WithSummary("Initializes a new instance of the Test class."))
                .WithConstructor(Code.CreateConstructor(AccessModifier.Private)
                    .WithSummary("Privately initializes a new instance of the Test class.")
                    .WithParameter("string", "name"))
                .WithConstructor(Code.CreateConstructor(AccessModifier.PrivateInternal)
                    .WithParameter("string", "name")
                    .WithParameter("int", "count"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Test
{
    /// <summary>
    /// Initializes a new instance of the Test class.
    /// </summary>
    public Test()
    {
    }

    /// <summary>
    /// Privately initializes a new instance of the Test class.
    /// </summary>
    private Test(string name)
    {
    }

    private internal Test(string name, int count)
    {
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}
