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
                .Replace("\r\n", "\n");

            var expectedCode = @"
public string Username
{
    get;
    set;
}
            ".Trim();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateProperty_Works_WithCustomGetterSetter()
        {
            var generatedCode = Code.CreateProperty("int", "Id")
                .WithGetter("_id")
                .WithSetter("{ value = value > 10 ? value : 10; _id = value; }")
                .ToSourceCode()
                .Replace("\r\n", "\n");

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
            ".Trim();

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}
