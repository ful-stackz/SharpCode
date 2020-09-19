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
    set =>
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
    }
}
