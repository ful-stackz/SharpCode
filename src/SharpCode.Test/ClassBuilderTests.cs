using System;
using System.Collections.Generic;
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
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class User
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithSummary_Works()
        {
            var generatedCode = Code.CreateClass("User")
                .WithSummary("A user of the application")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
/// <summary>
/// A user of the application
/// </summary>
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
                .ToSourceCode()
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
    private protected string _username;
    public User(string username)
    {
        _username = username;
    }
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateClass("User", AccessModifier.Internal)
                .WithField(Code.CreateField("string", "_username", AccessModifier.PrivateProtected))
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
        public void CreatingStaticClass_WithStaticProperties_Works()
        {
            var generatedCode = Code.CreateClass("Config")
                .MakeStatic()
                .WithProperty(Code.CreateProperty("string", "Name")
                    .MakeStatic()
                    .WithoutSetter()
                    .WithDefaultValue("\"SharpCode\""))
                .WithProperty(Code.CreateProperty("System.Version", "Version")
                    .MakeStatic()
                    .WithoutSetter()
                    .WithDefaultValue("new System.Version(0, 0, 1)"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public static class Config
{
    public static string Name
    {
        get;
    }

    = ""SharpCode"";
    public static System.Version Version
    {
        get;
    }

    = new System.Version(0, 0, 1);
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

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass(name: null).ToSourceCode(),
                "Generating the source for a class with null as a name should throw an exception.");

            Assert.Throws<ArgumentNullException>(
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

        [Test]
        public void ClassBuilder_WithProperties_ApisYieldIdenticalResults()
        {
            var enumerableApi = Code.CreateClass("Test")
                .WithProperties(new List<PropertyBuilder>
                {
                    Code.CreateProperty().WithType(typeof(int)).WithName("Count"),
                    Code.CreateProperty().WithType(typeof(string)).WithName("Name"),
                })
                .ToSourceCode();

            var paramsApi = Code.CreateClass("Test")
                .WithProperties(
                    Code.CreateProperty().WithType(typeof(int)).WithName("Count"),
                    Code.CreateProperty().WithType(typeof(string)).WithName("Name"))
                .ToSourceCode();

            Assert.AreEqual(enumerableApi, paramsApi);
        }

        [Test]
        public void ClassBuilder_ToSourceCode_ToString_YieldIdentical()
        {
            var toSourceCode = Code.CreateClass("Test")
                .WithField(Code.CreateField("int", "_count"))
                .WithConstructor(Code.CreateConstructor())
                .WithProperty(Code.CreateProperty("string", "Name"))
                .ToSourceCode();

            var toString = Code.CreateClass("Test")
                .WithField(Code.CreateField("int", "_count"))
                .WithConstructor(Code.CreateConstructor())
                .WithProperty(Code.CreateProperty("string", "Name"))
                .ToString();

            Assert.AreEqual(toSourceCode, toString);
        }
    }
}
