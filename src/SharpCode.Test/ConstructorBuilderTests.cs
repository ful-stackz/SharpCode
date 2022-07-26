using System;
using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class ConstructorBuilderTests
    {
        [Test]
        public void CreatingStaticConstructor_WithAccessModifier_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateConstructor()
                    .WithAccessModifier(AccessModifier.Public)
                    .MakeStatic(true)
                    .Build(),
                "Generating the source code for a static constructor with a specified access modifier should throw an exception.");
        }

        [Test]
        public void CreatingStaticConstructor_WithParameters_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateConstructor()
                    .WithAccessModifier(AccessModifier.None)
                    .WithParameter(typeof(string), "name")
                    .MakeStatic(true)
                    .Build(),
                "Generating the source code for a static constructor with parameters should throw an exception.");
        }

        [Test]
        public void CreatingStaticConstructor_WithBaseCall_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateConstructor()
                    .WithAccessModifier(AccessModifier.None)
                    .WithBaseCall()
                    .MakeStatic(true)
                    .Build(),
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
                .WithConstructor(Code.CreateConstructor(AccessModifier.PrivateProtected)
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

    private protected Test(string name, int count)
    {
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateConstructor_WithParameter_Works()
        {
            var generatedCode = Code.CreateClass()
                .WithAccessModifier(AccessModifier.Public)
                .WithName("Test")
                .WithFields(
                    Code.CreateField(typeof(string), "_name"),
                    Code.CreateField(typeof(int), "_count"))
                .WithConstructor(Code.CreateConstructor(AccessModifier.Private)
                    .WithSummary("Privately initializes a new instance of the Test class.")
                    .WithParameter("string", "name", "_name"))
                .WithConstructor(Code.CreateConstructor(AccessModifier.PrivateProtected)
                    .WithParameter(typeof(string), "name", "_name")
                    .WithParameter(typeof(int), "count", "_count"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Test
{
    private String _name;
    private Int32 _count;
    /// <summary>
    /// Privately initializes a new instance of the Test class.
    /// </summary>
    private Test(string name)
    {
        _name = name;
    }

    private protected Test(String name, Int32 count)
    {
        _name = name;
        _count = count;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateConstructor_WithInvalidParameter_Throws()
        {
            // WithParameter(Type type, ...) API
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateConstructor().WithParameter(
                    type: null as Type,
                    name: "something"),
                "Adding parameter with type '(Type)null' to a constructor should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateConstructor().WithParameter(
                    type: typeof(string),
                    name: null),
                "Adding parameter with name 'null' to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: typeof(string),
                    name: string.Empty),
                "Adding parameter with empty name to constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: typeof(string),
                    name: "  "),
                "Adding parameter with empty name to constructor should throw an exception.");

            // WithParameter(string type, ...) API
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateConstructor().WithParameter(
                    type: null as string,
                    name: "something"),
                "Adding parameter with type '(string)null' to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: string.Empty,
                    name: "something"),
                "Adding parameter with an empty string for type to a constructor should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateConstructor().WithParameter(
                    type: "string",
                    name: null),
                "Adding parameter with name 'null' to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: "string",
                    name: string.Empty),
                "Adding parameter with empty name to constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: "string",
                    name: "  "),
                "Adding parameter with empty name to constructor should throw an exception.");
        }

        [Test]
        public void CreateConstructor_WithInvalidReceivingMember_Throws()
        {
            // WithParameter(Type type, ...) API
            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: typeof(string),
                    name: "something",
                    receivingMember: string.Empty),
                "Adding parameter with an empty receiving member to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: typeof(string),
                    name: "something",
                    receivingMember: "  "),
                "Adding parameter with a whitespace receiving member to a constructor should throw an exception.");

            // WithParameter(string type, ...) API
            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: "string",
                    name: "something",
                    receivingMember: string.Empty),
                "Adding parameter with an empty receiving member to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithParameter(
                    type: "string",
                    name: "something",
                    receivingMember: "  "),
                "Adding parameter with a whitespace receiving member to a constructor should throw an exception.");
        }

        [Test]
        public void CreateConstructor_WithInvalidBaseCall_Throws()
        {
            // WithBaseCall(string passedParameter) API
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateConstructor().WithBaseCall(passedParameter: null),
                "Adding a base call with a 'null' name to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithBaseCall(passedParameter: string.Empty),
                "Adding a base call with an empty string name to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithBaseCall(passedParameter: "  "),
                "Adding a base call with a white space name to a constructor should throw an exception.");

            // WithBaseCall(params string[] passedParameters) API
            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithBaseCall(string.Empty, null),
                "Adding a base call with invalid parameter names to a constructor should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateConstructor().WithBaseCall("name", "name2", null),
                "Adding a base call with invalid parameter names to a constructor should throw an exception.");
        }

        [Test]
        public void CreateConstructor_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateConstructor().WithSummary(null),
                "Adding a 'null' summary to a constructor should throw an exception.");
        }
    }
}
