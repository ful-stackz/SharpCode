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

            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct().WithName(null).ToSourceCode(),
                "Generating the source code for a struct with null as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateStruct().WithName(string.Empty).ToSourceCode(),
                "Generating the source code for a struct with an empty string as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateStruct().WithName("   ").ToSourceCode(),
                "Generating the source code for a struct with whitespace as name should throw an exception.");
        }

        [Test]
        public void CreatingStruct_WithParameterlessConstructor_Throws()
        {
            Assert.Throws<SyntaxException>(
                () => Code.CreateStruct(name: "Test").WithConstructor(Code.CreateConstructor()).ToSourceCode(),
                "Generating the source code for a struct with a parameterless constructor should throw an exception.");
        }

        [Test]
        public void CreatingStruct_WithImplementedInterface_WithInvalidName_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => Code.CreateStruct(name: "Test").WithImplementedInterface(null).ToSourceCode(),
                "Generating the source code for a struct with null as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateStruct(name: "Test").WithImplementedInterface(string.Empty).ToSourceCode(),
                "Generating the source code for a struct with an empty string as name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateStruct(name: "Test").WithImplementedInterface("   ").ToSourceCode(),
                "Generating the source code for a struct with whitespace as name should throw an exception.");
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

    public int X
    {
        get => _x;
        set => _x = value;
    }

    public int Y
    {
        get => _y;
        set => _y = value;
    }
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
    public int X
    {
        get => _x;
        set => _x = value;
    }

    public int Y
    {
        get => _y;
        set => _y = value;
    }
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
    }
}
