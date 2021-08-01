using System;
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
        public void CreatingField_WithoutRequiredSettings_Throws()
        {
            // --- WithName() API
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithType("string").ToSourceCode(),
                "Generating the source code for a field without setting the name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName(string.Empty).WithType("string").ToSourceCode(),
                "Generating the source code for a field with an empty string as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName("  ").WithType("string").ToSourceCode(),
                "Generating the source code for a field with whitespace name should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithName(null).ToSourceCode(),
                "Generating the source code for a field with null as name should throw an exception.");

            // --- WithType() API
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName("count").ToSourceCode(),
                "Generating the source code for a field without setting the type should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName("count").WithType(string.Empty).ToSourceCode(),
                "Generating the source code for a field with an empty string as type should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName("count").WithType("  ").ToSourceCode(),
                "Generating the source code for a field with whitespace type should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithName("count").WithType((Type)null).ToSourceCode(),
                "Generating the source code for a field with null as type should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithName("count").WithType((string)null).ToSourceCode(),
                "Generating the source code for a field with null as type should throw an exception.");

            // --- Shorthand API
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField(name: null, type: "string").ToSourceCode(),
                "Generating the source code for a field with null as name should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField(name: "count", type: (Type)null).ToSourceCode(),
                "Generating the source code for a field with null as type should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField(name: "count", type: (string)null).ToSourceCode(),
                "Generating the source code for a field with null as type should throw an exception.");
        }

        [Test]
        public void CreatingField_WithSummary_Works()
        {
            var generatedCode = Code.CreateField()
                .WithAccessModifier(AccessModifier.PrivateInternal)
                .WithSummary("Stores the name of the thing.")
                .WithType("string")
                .WithName("_name")
                .MakeReadonly()
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
/// <summary>
/// Stores the name of the thing.
/// </summary>
private internal readonly string _name;
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingField_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Code.CreateField().WithSummary(null));
        }

        [Test]
        public void FieldBuilder_ToSourceCode_ToString_YieldSame()
        {
            var builder = Code.CreateField(typeof(int), "_count", AccessModifier.ProtectedInternal);
            Assert.AreEqual(builder.ToSourceCode(), builder.ToString());
        }
    }
}
