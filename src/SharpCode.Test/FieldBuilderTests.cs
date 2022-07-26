using System;
using System.Collections.Generic;
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

            // --- WithType() API
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateField().WithName("count").ToSourceCode(),
                "Generating the source code for a field without setting the type should throw an exception.");
        }

        [Test]
        public void CreatingField_WithSummary_Works()
        {
            var generatedCode = Code.CreateField()
                .WithAccessModifier(AccessModifier.PrivateProtected)
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
private protected readonly string _name;
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingField_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Code.CreateField().WithSummary(null));
        }

        [Test]
        public void CreateField_WithTypeParameter_Works()
        {
            var generatedCode = Code.CreateField("Dictionary", "_store")
                .WithTypeParameter(Code.CreateTypeParameter("TKey"))
                .WithTypeParameter(Code.CreateTypeParameter("TValue"))
                .ToSourceCode();

            var expectedCode = "private Dictionary<TKey, TValue> _store;";

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateField_WithTypeParameters_Works()
        {
            var generatedCode = Code.CreateField("List", "_users", AccessModifier.Private)
                .MakeReadonly()
                .WithTypeParameters(
                    Code.CreateTypeParameter("User"))
                .ToSourceCode();

            var expectedCode = "private readonly List<User> _users;";

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateField_WithTypeParameters_IEnumerable_Works()
        {
            var generatedCode = Code.CreateField("Map", "_coordinates")
                .WithTypeParameters(new List<TypeParameterBuilder>()
                {
                    Code.CreateTypeParameter("string"),
                    Code.CreateTypeParameter("(double X, double Y)"),
                })
                .ToSourceCode();

            var expectedCode = "private Map<string, (double X, double Y)> _coordinates;";

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void FieldBuilder_ToSourceCode_ToString_YieldSame()
        {
            var builder = Code.CreateField(typeof(int), "_count", AccessModifier.ProtectedInternal);
            Assert.AreEqual(builder.ToSourceCode(), builder.ToString());
        }

        [Test]
        public void CreateField_WithInvalidName_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithName(null),
                "Setting the field name to 'null' should throw.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateField().WithName(string.Empty),
                "Setting the field name to an empty string should throw.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateField().WithName("  "),
                "Setting the field name to a whitespace string should throw.");
        }

        [Test]
        public void CreateField_WithInvalidType_Throws()
        {
            // WithType(string)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithType(null as string),
                "Setting the field type to 'null' (string) should throw.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateField().WithType(string.Empty),
                "Setting the field type to an empty string should throw.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateField().WithType("  "),
                "Setting the field type to a whitespace string should throw.");

            // WithType(Type)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithType(null as Type),
                "Setting the field type to 'null' (Type) should throw.");
        }

        [Test]
        public void CreateField_WithInvalidBuilders_Throws()
        {
            // WithTypeParameter(...)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithTypeParameter(null));

            // WithTypeParameters(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithTypeParameters(null as TypeParameterBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateField().WithTypeParameters(new TypeParameterBuilder[] { null }));

            // WithTypeParameters(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateField().WithTypeParameters(null as IEnumerable<TypeParameterBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateField().WithTypeParameters(new List<TypeParameterBuilder> { null }));
        }
    }
}
