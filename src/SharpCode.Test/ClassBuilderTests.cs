using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class ClassBuilderTests
    {
        [Test]
        public void CreateClass_WithNoMembers_Works()
        {
            var generatedCode = Code.CreateClass("User")
                .ToSourceCode(formatted: true)
                .WithUnixEOL();

            var expectedCode = @"
public class User
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithFields()
        {
            var generatedCode = Code.CreateClass("Triangle", AccessModifier.Internal)
                .WithField(Code.CreateField("int", "_hypotenuse"))
                .WithField(Code.CreateField("double", "_adjacent"))
                .WithField(Code.CreateField("float", "_opposite"))
                .ToSourceCode(formatted: true)
                .WithUnixEOL();

            var expectedCode = @"
internal class Triangle
{
    private int _hypotenuse;
    private double _adjacent;
    private float _opposite;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_Works_WithFieldsAndProperties()
        {
            var generatedCode = Code.CreateClass("Triangle", AccessModifier.Public)
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
                    .WithGetter("{ return _opposite; }")
                    .WithSetter("{ _opposite = value; }"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
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
        get
        {
            return _opposite;
        }

        set
        {
            _opposite = value;
        }
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithFields_AndConstructor()
        {
            var expectedCode = @"
internal class User
{
    private internal string _username;
    public User(string username)
    {
        _username = username;
    }
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateClass("User", AccessModifier.Internal)
                .WithField(Code.CreateField("string", "_username", AccessModifier.PrivateInternal))
                .WithConstructor(Code.CreateConstructor()
                    .WithParameter("string", "username", "_username"))
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithInheritance_AndBaseCall()
        {
            var expectedCode = @"
public class UserCredentials : Credentials, IAuthorizationDetails, IMadeThisUp
{
    public UserCredentials(string username, string password): base(username, password, ""basic"")
    {
    }
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateClass("UserCredentials", AccessModifier.Public)
                .WithInheritedClass("Credentials")
                .WithImplementedInterface("IAuthorizationDetails")
                .WithImplementedInterface("IMadeThisUp")
                .WithConstructor(Code.CreateConstructor()
                    .WithParameter("string", "username")
                    .WithParameter("string", "password")
                    .WithBaseCall("username", "password", "\"basic\""))
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithInheritance_AndMultipleConstructors()
        {
            var expectedCode = @"
public class BasicCredentials : AuthorizationDetails
{
    public BasicCredentials(): base(string.Empty, string.Empty, ""basic"")
    {
    }

    public BasicCredentials(string username): base(username, string.Empty, ""basic"")
    {
    }

    public BasicCredentials(string username, string password): base(username, password, ""basic"")
    {
    }
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateClass("BasicCredentials")
                .WithInheritedClass("AuthorizationDetails")
                .WithConstructor(Code.CreateConstructor()
                    .WithBaseCall("string.Empty", "string.Empty", "\"basic\""))
                .WithConstructor(Code.CreateConstructor()
                    .WithParameter("string", "username")
                    .WithBaseCall("username", "string.Empty", "\"basic\""))
                .WithConstructor(Code.CreateConstructor()
                    .WithParameter("string", "username")
                    .WithParameter("string", "password")
                    .WithBaseCall("username", "password", "\"basic\""))
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_StaticClass()
        {
            var generatedCode = Code.CreateClass("Factory")
                .MakeStatic()
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public static class Factory
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_Throws_WhenRequiredSettingsNotProvided()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass().ToSourceCode(),
                "Generating the source code for a class without setting the name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass(name: null).ToSourceCode(),
                "Generating the source for a class with null as a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass().WithName(null).ToSourceCode(),
                "Generating the source for a class with null as a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass(name: string.Empty).ToSourceCode(),
                "Generating the source for a class with an empty name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass().WithName(string.Empty).ToSourceCode(),
                "Generating the source for a class with an empty name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass(name: "  ").ToSourceCode(),
                "Generating the source for a class with a whitespace name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateClass().WithName("  ").ToSourceCode(),
                "Generating the source for a class with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreatingStaticClass_WithMultipleConstructors_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateClass("Factory")
                    .MakeStatic()
                    .WithConstructor(Code.CreateConstructor())
                    .WithConstructor(Code.CreateConstructor())
                    .ToSourceCode(),
                "Generating the source code for a static class with multiple constructors should throw an exception.");
        }
    }
}
