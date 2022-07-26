using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class InterfaceBuilderTests
    {
        [Test]
        public void CreatingInterface_WithoutRequiredSettings_Throws()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateInterface().ToSourceCode(),
                "Generating the source code for an interface without a name should throw an exception.");
        }

        [Test]
        public void CreatingInterface_ValidatesPropertiesDefinitions()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithDefaultValue("42"))
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that has a default value should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithGetter("_value").WithSetter("_value = value"))
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that has " +
                    "a non auto implemented getter should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithSetter("_value = value"))
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that has " +
                    "a non auto implemented setter should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateInterface("ITest")
                    .WithProperty(Code.CreateProperty("int", "Test").WithoutGetter().WithoutSetter())
                    .ToSourceCode(),
                "Generating the source code for an interface with a property that does not have " +
                    "a getter nor a setter should throw an exception.");
        }

        [Test]
        public void CreatingInterface_Works()
        {
            var generatedCode = Code.CreateInterface("ITest").ToSourceCode().WithUnixEOL();

            var expectedCode = @"
public interface ITest
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingInterface_WithImplementedInterfaces_Works()
        {
            var generatedCode = Code.CreateInterface("ITest")
                .WithImplementedInterface("IHaveTests")
                .WithImplementedInterfaces("IFailNot", "IHaveCoverage")
                .WithImplementedInterfaces(new List<string> { "IHaveNiceComments" })
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface ITest : IHaveTests, IFailNot, IHaveCoverage, IHaveNiceComments
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingInterface_WithDifferentAccessModifiers_Works()
        {
            var expectedCode = @"
{access-modifier} interface ITest
{
}
            ".WithUnixEOL();

            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", string.Empty).Trim(),
                Code.CreateInterface("ITest", AccessModifier.None).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "private").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Private).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "private protected").Trim(),
                Code.CreateInterface("ITest", AccessModifier.PrivateProtected).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "internal").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Internal).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "protected").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Protected).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "protected internal").Trim(),
                Code.CreateInterface("ITest", AccessModifier.ProtectedInternal).ToSourceCode().WithUnixEOL());
            Assert.AreEqual(
                expectedCode.Replace("{access-modifier}", "public").Trim(),
                Code.CreateInterface("ITest", AccessModifier.Public).ToSourceCode().WithUnixEOL());
        }

        [Test]
        public void CreatingInterface_WithProperties_Works()
        {
            var generatedCode = Code.CreateInterface("ITest")
                .WithProperty(Code.CreateProperty("int", "Number"))
                .WithProperties(Code.CreateProperty("string", "Stringy"))
                .WithProperties(new List<PropertyBuilder>
                {
                    Code.CreateProperty("bool", "Booly"),
                })
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface ITest
{
    int Number
    {
        get;
        set;
    }

    string Stringy
    {
        get;
        set;
    }

    bool Booly
    {
        get;
        set;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingInterface_WithSummary_Works()
        {
            var generatedCode = Code.CreateInterface()
                .WithAccessModifier(AccessModifier.Public)
                .WithName("ISerializable")
                .WithSummary("Allows an object to control its own serialization and deserialization.")
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
/// <summary>
/// Allows an object to control its own serialization and deserialization.
/// </summary>
public interface ISerializable
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithTypeParameter_Works()
        {
            var generatedCode = Code.CreateInterface("Dict")
                .WithTypeParameter(Code.CreateTypeParameter("TKey", "IEquatable<TKey>"))
                .WithTypeParameter(Code.CreateTypeParameter("TValue", TypeParameterConstraint.NotNull))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithTypeParameters_Params_Works()
        {
            var generatedCode = Code.CreateInterface("Dict")
                .WithTypeParameters(
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface Dict<K, V>
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithTypeParameters_IEnumerable_Works()
        {
            var generatedCode = Code.CreateInterface("Dict")
                .WithTypeParameters(new List<TypeParameterBuilder>()
                {
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"),
                })
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public interface Dict<K, V>
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithTypeParameters_WithProperty_Works()
        {
            var generatedCode = Code.CreateInterface("Dict")
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
public interface Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
    Dictionary<TKey, TValue> Store
    {
        get;
        set;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithTypeParameters_Params_WithProperty_Works()
        {
            var generatedCode = Code.CreateInterface("Dict")
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
public interface Dict<K, V>
{
    Dictionary<K, V> Store
    {
        get;
        set;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithTypeParameters_IEnumerable_WithProperty_Works()
        {
            var generatedCode = Code.CreateInterface("Dict")
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
public interface Dict<K, V>
{
    Dictionary<K, V> Store
    {
        get;
        set;
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateInterface_WithInvalidName_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithName(null),
                "Setting interface name to 'null' should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithName(string.Empty),
                "Setting interface name to an empty string should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithName("  "),
                "Setting interface name to a whitespace string should throw an exception.");
        }

        [Test]
        public void CreateInterface_WithInvalidImplementedInterface_Throws()
        {
            // WithImplementedInterface()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithImplementedInterface(null),
                "Adding an implemented interface with a 'null' name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithImplementedInterface(string.Empty),
                "Adding an implemented interface with an empty name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithImplementedInterface("  "),
                "Adding an implemented interface with a whitespace name should throw an exception.");

            // WithImplementedInterfaces(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithImplementedInterfaces(null as string[]),
                "Adding implemented interfaces from a 'null' collection should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithImplementedInterfaces("IEnumerable", null),
                "Adding an implemented interface with a 'null' name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithImplementedInterfaces("IEnumerable", string.Empty),
                "Adding an implemented interface with a whitespace name should throw an exception.");

            // WithImplementedInterfaces(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithImplementedInterfaces(null as IEnumerable<string>),
                "Adding implemented interfaces from a 'null' collection should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithImplementedInterfaces(new List<string> { "IEnumerable", null }),
                "Adding an implemented interface with a 'null' name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithImplementedInterfaces(new List<string> { "IEnumerable", string.Empty }),
                "Adding an implemented interface with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreateInterface_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithSummary(null),
                "Setting interface summary to 'null' should throw an exception.");
        }

        [Test]
        public void CreateInterface_WithInvalidProperty_Throws()
        {
            // WithProperty()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithProperty(null));

            // WithProperties(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithProperties(null as PropertyBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithProperties(new PropertyBuilder[] { null }));

            // WithProperties(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithProperties(null as IEnumerable<PropertyBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithProperties(new List<PropertyBuilder> { null }));
        }

        [Test]
        public void CreateInterface_WithInvalidTypeParameter_Throws()
        {
            // WithProperty()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithTypeParameter(null));

            // WithTypeParameters(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithTypeParameters(null as TypeParameterBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithTypeParameters(new TypeParameterBuilder[] { null }));

            // WithTypeParameters(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateInterface().WithTypeParameters(null as IEnumerable<TypeParameterBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateInterface().WithTypeParameters(new List<TypeParameterBuilder> { null }));
        }
    }
}
