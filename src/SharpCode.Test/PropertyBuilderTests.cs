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
        public void CreateProperty_Throws_WhenRequiredSettingsNotProvided()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithType("string").ToSourceCode(),
                "Expected generating the source code for a property without setting the name to throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateProperty().WithName("IHaveNoType").ToSourceCode(),
                "Expected generating the source code for a property without setting the type to throw an exception.");
        }
    }
}
