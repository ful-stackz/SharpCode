using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class StructBuilderTests
    {
        [Test]
        public void CreatingStruct_WithoutRequiredSettings_Throws()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateStruct().ToSourceCode(),
                "Generating the source code for a struct without a name should throw an exception.");
        }

        [Test]
        public void CreatingStruct_WithParameterlessConstructor_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateStruct(name: "Test").WithConstructor(Code.CreateConstructor()).ToSourceCode(),
                "Generating the source code for a struct with a parameterless constructor should throw an exception.");
        }

        [Test]
        public void CreatingStruct_WithProperties_WithDefaultValue_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateStruct(name: "Test")
                    .WithProperty(Code.CreateProperty("string", "Name").WithDefaultValue("\"fail\""))
                    .ToSourceCode(),
                "Generating the source code for a struct with a property which has a default value should throw an exception.");
        }

        [Test]
        public void CreatingStruct_Works()
        {
            var generatedCode = Code.CreateStruct()
                .WithSummary("Represents an X/Y position.")
                .WithAccessModifier(AccessModifier.ProtectedInternal)
                .WithName("Position")
                .WithImplementedInterface("IComparable")
                .WithFields(
                    Code.CreateField("int", "_x"),
                    Code.CreateField("int", "_y"))
                .WithConstructor(Code.CreateConstructor()
                    .WithParameter("int", "x", "_x")
                    .WithParameter("int", "y", "_y"))
                .WithProperties(
                    Code.CreateProperty("int", "X").WithGetter("_x").WithSetter("_x = value"),
                    Code.CreateProperty("int", "Y").WithGetter("_y").WithSetter("_y = value"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
/// <summary>
/// Represents an X/Y position.
/// </summary>
protected internal struct Position : IComparable
{
    private int _x;
    private int _y;
    public Position(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public int X { get => _x; set => _x = value; }

    public int Y { get => _y; set => _y = value; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingStruct_WithoutConstructor_Works()
        {
            var generatedCode = Code.CreateStruct(name: "Position")
                .WithField(Code.CreateField("int", "_x"))
                .WithField(Code.CreateField("int", "_y"))
                .WithProperties(new List<PropertyBuilder>
                {
                    Code.CreateProperty("int", "X").WithGetter("_x").WithSetter("_x = value"),
                    Code.CreateProperty("int", "Y").WithGetter("_y").WithSetter("_y = value"),
                })
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public struct Position
{
    private int _x;
    private int _y;
    public int X { get => _x; set => _x = value; }

    public int Y { get => _y; set => _y = value; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void StructBuilder_WithImplementedInterfaces_ApisYieldIdenticalResults()
        {
            var enumerableApi = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.None)
                .WithName("Test")
                .WithImplementedInterfaces(new List<string>
                {
                    "IHasNoMembers",
                    "IAmAStruct",
                })
                .ToSourceCode();

            var paramsApi = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.None)
                .WithName("Test")
                .WithImplementedInterfaces(
                    "IHasNoMembers",
                    "IAmAStruct")
                .ToSourceCode();

            Assert.AreEqual(enumerableApi, paramsApi);
        }

        [Test]
        public void StructBuilder_WithFields_ApisYieldIdenticalResults()
        {
            var enumerableApi = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.Private)
                .WithName("Test")
                .WithFields(new List<FieldBuilder>
                {
                    Code.CreateField("int", "_count"),
                    Code.CreateField("string", "_name"),
                })
                .ToSourceCode();

            var paramsApi = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.Private)
                .WithName("Test")
                .WithFields(
                    Code.CreateField("int", "_count"),
                    Code.CreateField("string", "_name"))
                .ToSourceCode();

            Assert.AreEqual(enumerableApi, paramsApi);
        }

        [Test]
        public void StructBuilder_WithProperties_ApisYieldIdenticalResults()
        {
            var enumerableApi = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.Private)
                .WithName("Test")
                .WithProperties(new List<PropertyBuilder>
                {
                    Code.CreateProperty("int", "Count"),
                    Code.CreateProperty("string", "Name"),
                })
                .ToSourceCode();

            var paramsApi = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.Private)
                .WithName("Test")
                .WithProperties(
                    Code.CreateProperty("int", "Count"),
                    Code.CreateProperty("string", "Name"))
                .ToSourceCode();

            Assert.AreEqual(enumerableApi, paramsApi);
        }

        [Test]
        public void CreateStruct_WithTypeParameter_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
                .WithTypeParameter(Code.CreateTypeParameter("TKey", "IEquatable<TKey>"))
                .WithTypeParameter(Code.CreateTypeParameter("TValue", TypeParameterConstraint.NotNull))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public struct Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_Params_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
                .WithTypeParameters(
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public struct Dict<K, V>
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_IEnumerable_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
                .WithTypeParameters(new List<TypeParameterBuilder>()
                {
                    Code.CreateTypeParameter("K"),
                    Code.CreateTypeParameter("V"),
                })
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
public struct Dict<K, V>
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_WithField_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
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
public struct Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
    private readonly Dictionary<TKey, TValue> _store;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_Params_WithField_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
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
public struct Dict<K, V>
{
    private readonly Dictionary<K, V> _store;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_IEnumerable_WithField_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
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
public struct Dict<K, V>
{
    private readonly Dictionary<K, V> _store;
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_WithProperty_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
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
public struct Dict<TKey, TValue>
    where TKey : IEquatable<TKey> where TValue : notnull
{
    public Dictionary<TKey, TValue> Store { get; set; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_Params_WithProperty_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
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
public struct Dict<K, V>
{
    public Dictionary<K, V> Store { get; set; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateStruct_WithTypeParameters_IEnumerable_WithProperty_Works()
        {
            var generatedCode = Code.CreateStruct("Dict")
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
public struct Dict<K, V>
{
    public Dictionary<K, V> Store { get; set; }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void StructBuilder_ToSourceCode_ToString_Identical()
        {
            var toSourceCode = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.Internal)
                .WithName("Structure")
                .WithProperty(Code.CreateProperty("double", "Seed"))
                .ToSourceCode();

            var toString = Code.CreateStruct()
                .WithAccessModifier(AccessModifier.Internal)
                .WithName("Structure")
                .WithProperty(Code.CreateProperty("double", "Seed"))
                .ToString();

            Assert.AreEqual(toSourceCode, toString);
        }

        [Test]
        public void CreateStruct_WithInvalidName_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithName(null),
                "Generating the source code for a struct with null as a name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithName(string.Empty),
                "Generating the source code for a struct with an empty name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithName("  "),
                "Generating the source code for a struct with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreateStruct_WithInvalidSummary_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithSummary(null),
                "Generating the source code for a struct with null as summary should throw an exception.");
        }

        [Test]
        public void CreateStruct_WithInvalidImplementedInterface_Throws()
        {
            // WithImplementedInterface
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithImplementedInterface(null),
                "Adding an implemented interface with 'null' name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithImplementedInterface(string.Empty),
                "Adding an implemented interface with an empty name should throw an exception.");

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithImplementedInterface("  "),
                "Adding an implemented interface with a whitespace name should throw an exception.");

            // WithImplementedInterfaces(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithImplementedInterfaces(null as string[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithImplementedInterfaces(new string[] { null }));

            // WithImplementedInterfaces(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithImplementedInterfaces(null as IEnumerable<string>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithImplementedInterfaces(new List<string> { null }));
        }

        [Test]
        public void CreateStruct_WithInvalidBuilders_Throws()
        {
            // WithTypeParameter()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithTypeParameter(null));

            // WithTypeParameters(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithTypeParameters(null as TypeParameterBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithTypeParameters(new TypeParameterBuilder[] { null }));

            // WithTypeParameters(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithTypeParameters(null as IEnumerable<TypeParameterBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithTypeParameters(new List<TypeParameterBuilder> { null }));

            // WithField()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithField(null));

            // WithFields(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithFields(null as FieldBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithFields(new FieldBuilder[] { null }));

            // WithFields(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithFields(null as IEnumerable<FieldBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithFields(new List<FieldBuilder> { null }));

            // WithProperty()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithProperty(null));

            // WithProperties(params)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithProperties(null as PropertyBuilder[]));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithProperties(new PropertyBuilder[] { null }));

            // WithProperties(IEnumerable)
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithProperties(null as IEnumerable<PropertyBuilder>));

            Assert.Throws<ArgumentException>(
                () => Code.CreateStruct().WithProperties(new List<PropertyBuilder> { null }));

            // WithConstructor()
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithConstructor(null));
        }
    }
}
