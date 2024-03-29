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
    public double Hypotenuse { get => _hypotenuse; set => _hypotenuse = value; }

    public double Adjacent { get => _adjacent; set => _adjacent = value; }

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
    public UserCredentials(string username, string password) : base(username, password, ""basic"")
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
    public BasicCredentials() : base(string.Empty, string.Empty, ""basic"")
    {
    }

    public BasicCredentials(string username) : base(username, string.Empty, ""basic"")
    {
    }

    public BasicCredentials(string username, string password) : base(username, password, ""basic"")
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
    public static string Name { get; } = ""SharpCode"";
    public static System.Version Version { get; } = new System.Version(0, 0, 1);
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
        }

        [Test]
        public void CreateClass_WithTypeParameter_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameter(Code.CreateTypeParameter("TKey", "IEquatable<TKey>"))
                .WithTypeParameter(Code.CreateTypeParameter("TValue", TypeParameterConstraint.NotNull))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_Params_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameters(
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<K, V>
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_IEnumerable_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameters(new List<TypeParameterBuilder>()
                {
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"),
                })
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<K, V>
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_WithField_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameter(Code.CreateTypeParameter("TKey", "IEquatable<TKey>"))
                .WithTypeParameter(Code.CreateTypeParameter("TValue", TypeParameterConstraint.NotNull))
                .WithField(
                    Code.CreateField("Dictionary", "_store")
                        .WithTypeParameters(
                            Code.CreateTypeParameter("TKey"),
                            Code.CreateTypeParameter("TValue"))
                        .MakeReadonly())
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
    private readonly Dictionary<TKey, TValue> _store;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_Params_WithField_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameters(
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"))
                .WithField(
                    Code.CreateField("Dictionary", "_store")
                        .WithTypeParameters(
                            Code.CreateTypeParameter("K"),
                            Code.CreateTypeParameter("V"))
                        .MakeReadonly())
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<K, V>
{
    private readonly Dictionary<K, V> _store;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_IEnumerable_WithField_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameters(new List<TypeParameterBuilder>()
                {
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"),
                })
                .WithField(
                    Code.CreateField("Dictionary", "_store")
                        .WithTypeParameters(
                            Code.CreateTypeParameter("K"),
                            Code.CreateTypeParameter("V"))
                        .MakeReadonly())
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<K, V>
{
    private readonly Dictionary<K, V> _store;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_WithProperty_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameter(Code.CreateTypeParameter("TKey", "IEquatable<TKey>"))
                .WithTypeParameter(Code.CreateTypeParameter("TValue", TypeParameterConstraint.NotNull))
                .WithProperty(
                    Code.CreateProperty("Dictionary", "Store")
                        .WithTypeParameters(
                            Code.CreateTypeParameter("TKey"),
                            Code.CreateTypeParameter("TValue")))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
    public Dictionary<TKey, TValue> Store { get; set; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_Params_WithProperty_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameters(
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"))
                .WithProperty(
                    Code.CreateProperty("Dictionary", "Store")
                        .WithTypeParameters(
                            Code.CreateTypeParameter("K"),
                            Code.CreateTypeParameter("V")))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<K, V>
{
    public Dictionary<K, V> Store { get; set; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_WithTypeParameters_IEnumerable_WithProperty_Works()
        {
            var generatedCode = Code.CreateClass("Dict")
                .WithTypeParameters(new List<TypeParameterBuilder>()
                {
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"),
                })
                .WithProperty(
                    Code.CreateProperty("Dictionary", "Store")
                        .WithTypeParameters(
                            Code.CreateTypeParameter("K"),
                            Code.CreateTypeParameter("V")))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public class Dict<K, V>
{
    public Dictionary<K, V> Store { get; set; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
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

        [Test]
        public void CreateClass_WithInvalidName_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithName(null),
                "Generating the source code for a class with null as a name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithName(string.Empty),
                "Generating the source code for a class with an empty name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithName("  "),
                "Generating the source code for a class with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreateClass_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithSummary(null),
                "Generating the source code for a class with null as summary should throw an exception.");
        }

        [Test]
        public void CreateClass_WithInvalidInheritedClass_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithInheritedClass(null),
                "Adding an inherited class with 'null' name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithInheritedClass(string.Empty),
                "Adding an inherited class with an empty name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithInheritedClass("  "),
                "Adding an inherited class with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreateClass_WithInvalidImplementedInterface_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithImplementedInterface(null),
                "Adding an implemented interface with 'null' name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithImplementedInterface(string.Empty),
                "Adding an implemented interface with an empty name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithImplementedInterface("  "),
                "Adding an implemented interface with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreateClass_WithInvalidBuilders_Throws()
        {
            // WithTypeParameter()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithTypeParameter(null));

            // WithTypeParameters(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithTypeParameters(null as TypeParameterBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithTypeParameters(new TypeParameterBuilder[] { null }));

            // WithTypeParameters(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithTypeParameters(null as IEnumerable<TypeParameterBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithTypeParameters(new List<TypeParameterBuilder> { null }));

            // WithField()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithField(null));

            // WithFields(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithFields(null as FieldBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithFields(new FieldBuilder[] { null }));

            // WithProperty()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithProperty(null));

            // WithProperties(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithProperties(null as PropertyBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithProperties(new PropertyBuilder[] { null }));

            // WithProperties(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithProperties(null as IEnumerable<PropertyBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateClass().WithProperties(new List<PropertyBuilder> { null }));

            // WithConstructor()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateClass().WithConstructor(null));
        }
    }
}
