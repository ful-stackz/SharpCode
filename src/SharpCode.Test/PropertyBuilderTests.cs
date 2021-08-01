using System;
using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class PropertyBuilderTests
    {
        [Test]
        public void CreateProperty_Works_WithDefaultValues()
        {
            var generatedCode = Code.CreateProperty("string", "Username")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public string Username
{
    get;
    set;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateProperty_Works_WithSummary()
        {
            var generatedCode = Code.CreateProperty("string", "Username")
                .WithSummary("The name of the user")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
/// <summary>
/// The name of the user
/// </summary>
public string Username
{
    get;
    set;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingProperty_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Code.CreateProperty().WithSummary(null));
        }

        [Test]
        public void CreateProperty_Works_WithCustomGetterSetter()
        {
            var generatedCode = Code.CreateProperty("int", "Id")
                .WithGetter("_id")
                .WithSetter("{ value = value > 10 ? value : 10; _id = value; }")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public int Id
{
    get => _id;
    set
    {
        value = value > 10 ? value : 10;
        _id = value;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateProperty_Works_WithGetterWithoutSetter()
        {
            var expectedCode = @"
private string EmptyString
{
    get => string.Empty;
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateProperty("string", "EmptyString", AccessModifier.Private)
                .WithGetter("string.Empty")
                .WithoutSetter()
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateProperty_WithoutGetterAndSetter_Works()
        {
            var generatedCode = Code.CreateProperty("string", "NoGetterOrSetter", AccessModifier.Public)
                .WithoutGetter()
                .WithoutSetter()
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = "public string NoGetterOrSetter;";

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingProperty_WithoutRequiredSettings_Throws()
        {
            // --- WithName() API
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithType("string").ToSourceCode(),
                "Generating the source code for a property without setting the name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithName(string.Empty).WithType("string").ToSourceCode(),
                "Generating the source code for a property with an empty string as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithName("  ").WithType("string").ToSourceCode(),
                "Generating the source code for a property with whitespace name should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateProperty().WithName(null).ToSourceCode(),
                "Generating the source code for a property with null as name should throw an exception.");

            // --- WithType() API
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithName("count").ToSourceCode(),
                "Generating the source code for a property without setting the type should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithName("count").WithType(string.Empty).ToSourceCode(),
                "Generating the source code for a property with an empty string as type should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithName("count").WithType("  ").ToSourceCode(),
                "Generating the source code for a property with whitespace type should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateProperty().WithName("count").WithType((Type)null).ToSourceCode(),
                "Generating the source code for a property with null as type should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateProperty().WithName("count").WithType((string)null).ToSourceCode(),
                "Generating the source code for a property with null as type should throw an exception.");

            // --- Shorthand API
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateProperty(name: null, type: "string").ToSourceCode(),
                "Generating the source code for a property with null as name should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateProperty(name: "count", type: (Type)null).ToSourceCode(),
                "Generating the source code for a property with null as type should throw an exception.");

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateProperty(name: "count", type: (string)null).ToSourceCode(),
                "Generating the source code for a property with null as type should throw an exception.");
        }

        [Test]
        public void CreatingProperty_WithCustomGetterOrSetterAndDefaultValue_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateProperty("int", "Test")
                    .WithGetter("0")
                    .WithDefaultValue("0")
                    .ToSourceCode(),
                "Generating the source code for a property with a custom getter and a default value should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateProperty("int", "Test")
                    .WithSetter("_local = value")
                    .WithDefaultValue("0")
                    .ToSourceCode(),
                "Generating the source code for a property with a custom setter and a default value should throw an exception.");
        }

        [Test]
        public void CreatingProperty_WithInvalidAutoGetterOrSetter_Throws()
        {
            // public int Test { set; } is not valid C# code
            Assert.Throws<SyntaxException>(
                () => Code.CreateProperty("int", "Test")
                    .WithoutGetter()
                    .WithSetter()
                    .ToSourceCode(),
                "Generating the source code for a property with an auto implemented setter " +
                    "and without a getter should throw an exception.");

            // public int Test { get => _local; set; } is not valid C# code
            Assert.Throws<SyntaxException>(
                () => Code.CreateProperty("int", "Test")
                    .WithGetter("_local")
                    .WithSetter()
                    .ToSourceCode(),
                "Generating the source code for a property with a custom getter and an auto implemented setter " +
                    "should throw an exception.");
        }

        [Test]
        public void CreatingProperty_WithDefaultValue_Works()
        {
            var generatedCode = Code.CreateProperty("int", "Zero")
                .WithGetter()
                .WithSetter()
                .WithDefaultValue("0")
                .ToSourceCode()
                .WithUnixEOL();
            var expectedCode = @"
public int Zero
{
    get;
    set;
}

= 0;
            ".Trim().WithUnixEOL();
            Assert.AreEqual(
                expectedCode,
                generatedCode,
                "Failed case: property with auto implemented getter and setter, and default value");

            generatedCode = Code.CreateProperty("string", "Name")
                .WithGetter()
                .WithoutSetter()
                .WithDefaultValue("\"Test\"")
                .ToSourceCode()
                .WithUnixEOL();
            expectedCode = @"
public string Name
{
    get;
}

= ""Test"";
            ".Trim().WithUnixEOL();
            Assert.AreEqual(
                expectedCode,
                generatedCode,
                "Failed case: property with auto implemented getter and default value");

            generatedCode = Code.CreateProperty("int", "Zero")
                .WithGetter()
                .WithSetter()
                .WithDefaultValue("0")
                .MakeStatic()
                .ToSourceCode()
                .WithUnixEOL();
            expectedCode = @"
public static int Zero
{
    get;
    set;
}

= 0;
            ".Trim().WithUnixEOL();
            Assert.AreEqual(
                expectedCode,
                generatedCode,
                "Failed case: static property with auto implemented getter and setter, and default value");

            generatedCode = Code.CreateProperty("int", "Zero")
                .WithGetter()
                .WithoutSetter()
                .WithDefaultValue("0")
                .MakeStatic()
                .ToSourceCode()
                .WithUnixEOL();
            expectedCode = @"
public static int Zero
{
    get;
}

= 0;
            ".Trim().WithUnixEOL();
            Assert.AreEqual(
                expectedCode,
                generatedCode,
                "Failed case: static property with auto implemented getter and default value");
        }

        [Test]
        public void CreatingProperty_WithInvalidDefaultValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Code.CreateProperty().WithDefaultValue(null));
        }

        [Test]
        public void PropertyBuilder_ToSourceCode_ToString_YieldSame()
        {
            var builder = Code.CreateProperty(typeof(string), "Name", AccessModifier.ProtectedInternal);
            Assert.AreEqual(builder.ToSourceCode(), builder.ToString());
        }
    }
}
