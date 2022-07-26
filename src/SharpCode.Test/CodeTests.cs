using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpCode.Test;

[TestFixture]
public class CodeTests
{
    [Test]
    public void CreateNamespace_Works()
    {
        var expected = @"
namespace TestNamespace
{
}
        ".Trim().WithUnixEOL();

        // CreateNamespace()
        Assert.AreEqual(expected, Code.CreateNamespace().WithName("TestNamespace").ToSourceCode().WithUnixEOL());

        // CreateNamespace(string)
        Assert.AreEqual(expected, Code.CreateNamespace("TestNamespace").ToSourceCode().WithUnixEOL());
    }

    [Test]
    public void CreateNamespace_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateNamespace(name: null),
            "Generating the source code for a namespace with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateNamespace(name: string.Empty),
            "Generating the source code for a namespace with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateNamespace(name: "  "),
            "Generating the source code for a namespace with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateEnum_Works()
    {
        var expected = @"
public enum TestEnum
{
}
        ".Trim().WithUnixEOL();

        // CreateEnum()
        Assert.AreEqual(expected, Code.CreateEnum().WithName("TestEnum").ToSourceCode().WithUnixEOL());

        // CreateEnum(string, AccessModifier)
        Assert.AreEqual(expected, Code.CreateEnum("TestEnum").ToSourceCode().WithUnixEOL());
    }

    [Test]
    public void CreateEnum_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateEnum(name: null),
            "Generating the source for an enum with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnum(name: string.Empty),
            "Generating the source for an enum with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnum(name: "  "),
            "Generating the source for an enum with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateEnumMember_Works()
    {
        var expected = @"
public enum TestEnum
{
    OptionOne = 0,
    OptionTwo = 1
}
        ".Trim().WithUnixEOL();

        Assert.AreEqual(
            expected,
            Code.CreateEnum("TestEnum")
                .WithMembers(
                    Code.CreateEnumMember().WithName("OptionOne").WithValue(0),
                    Code.CreateEnumMember("OptionTwo", 1))
                .ToSourceCode()
                .WithUnixEOL());
    }

    [Test]
    public void CreateEnumMember_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateEnumMember(null),
            "Generating the source code for an enum member with null as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnumMember(string.Empty),
            "Generating the source code for an enum member with an empty string as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnumMember("  "),
            "Generating the source code for an enum member with whitespace name should throw an exception.");
    }

    [Test]
    public void CreateInterface_Works()
    {
        var expected = @"
public interface ITest
{
}
        ".Trim().WithUnixEOL();

        // CreateInterface()
        Assert.AreEqual(expected, Code.CreateInterface().WithName("ITest").ToSourceCode().WithUnixEOL());

        // CreateInterface(string, AccessModifier)
        Assert.AreEqual(expected, Code.CreateInterface("ITest").ToSourceCode().WithUnixEOL());
    }

    [Test]
    public void CreateInterface_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateInterface(name: null),
            "Generating the source for an interface with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateInterface(name: string.Empty),
            "Generating the source for an interface with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateInterface(name: "  "),
            "Generating the source for an interface with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateClass_Works()
    {
        var expected = @"
public class Test
{
}
        ".Trim().WithUnixEOL();

        // CreateClass()
        Assert.AreEqual(expected, Code.CreateClass().WithName("Test").ToSourceCode().WithUnixEOL());

        // CreateClass(string, AccessModifier)
        Assert.AreEqual(expected, Code.CreateClass("Test").ToSourceCode().WithUnixEOL());
    }

    [Test]
    public void CreateClass_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateClass(name: null),
            "Generating the source for a class with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateClass(name: string.Empty),
            "Generating the source for a class with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateClass(name: "  "),
            "Generating the source for a class with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateStruct_Works()
    {
        var expected = @"
public struct Test
{
}
        ".Trim().WithUnixEOL();

        // CreateStruct()
        Assert.AreEqual(expected, Code.CreateStruct().WithName("Test").ToSourceCode().WithUnixEOL());

        // CreateStruct(string, AccessModifier)
        Assert.AreEqual(expected, Code.CreateStruct("Test").ToSourceCode().WithUnixEOL());
    }

    [Test]
    public void CreateStruct_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateStruct(name: null),
            "Generating the source for a struct with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateStruct(name: string.Empty),
            "Generating the source for a struct with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateStruct(name: "  "),
            "Generating the source for a struct with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateField_Works()
    {
        var expected = "private Action _field;";

        // CreateField()
        Assert.AreEqual(
            expected,
            Code.CreateField()
                .WithType(typeof(Action))
                .WithName("_field")
                .ToSourceCode()
                .WithUnixEOL());

        // CreateField(string, string, AccessModifier)
        Assert.AreEqual(
            expected,
            Code.CreateField("Action", "_field")
                .ToSourceCode()
                .WithUnixEOL());

        // CreateField(Type, string, AccessModifier)
        Assert.AreEqual(
            expected,
            Code.CreateField(typeof(Action), "_field")
                .ToSourceCode()
                .WithUnixEOL());
    }

    [Test]
    public void CreateField_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateField(name: null, type: "string"),
            "Generating the source code for a field with 'null' as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateField(name: string.Empty, type: "string"),
            "Generating the source code for a field with an empty string as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateField(name: "  ", type: "string"),
            "Generating the source code for a field with a whitespace string as name should throw an exception.");
    }

    [Test]
    public void CreateField_WithInvalidType_Throws()
    {
        // CreateField(string, string)
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateField(name: "test", type: null as string),
            "Generating the source code for a field with 'null' as type should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateField(name: "test", type: string.Empty),
            "Generating the source code for a field with an empty string as type should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateField(name: "test", type: "  "),
            "Generating the source code for a field with a whitespace string as type should throw an exception.");

        // CreateField(string, Type)
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateField(name: "test", type: null as Type),
            "Generating the source code for a field with 'null' as type should throw an exception.");
    }

    [Test]
    public void CreateProperty_Works()
    {
        var expected = "public Action DoWork { get; set; }";

        // CreateProperty()
        Assert.AreEqual(
            expected,
            Code.CreateProperty()
                .WithType(typeof(Action))
                .WithName("DoWork")
                .ToSourceCode()
                .WithUnixEOL());

        // CreateProperty(string, string, AccessModifier)
        Assert.AreEqual(
            expected,
            Code.CreateProperty("Action", "DoWork")
                .ToSourceCode()
                .WithUnixEOL());

        // CreateProperty(string, string, AccessModifier, AccessModifier)
        Assert.AreEqual(
            expected,
            Code.CreateProperty("Action", "DoWork", AccessModifier.Public, AccessModifier.Public)
                .ToSourceCode()
                .WithUnixEOL());

        // CreateProperty(Type, string, AccessModifier)
        Assert.AreEqual(
            expected,
            Code.CreateProperty(typeof(Action), "DoWork")
                .ToSourceCode()
                .WithUnixEOL());

        // CreateProperty(Type, string, AccessModifier, AccessModifier)
        Assert.AreEqual(
            expected,
            Code.CreateProperty(typeof(Action), "DoWork", AccessModifier.Public, AccessModifier.Public)
                .ToSourceCode()
                .WithUnixEOL());
    }

    [Test]
    public void CreateProperty_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateProperty(name: null, type: "string"),
            "Generating the source code for a property with 'null' as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateProperty(name: string.Empty, type: "string"),
            "Generating the source code for a property with an empty string as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateProperty(name: "   ", type: "string"),
            "Generating the source code for a property with a whitespace string as name should throw an exception.");
    }

    [Test]
    public void CreateProperty_WithInvalidType_Throws()
    {
        // CreateProperty(string, string)
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateProperty(name: "test", type: null as string),
            "Generating the source code for a property with 'null' as type should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateProperty(name: "test", type: string.Empty),
            "Generating the source code for a property with an empty string as type should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateProperty(name: "test", type: "  "),
            "Generating the source code for a property with a whitespace string as type should throw an exception.");

        // CreateProperty(string, Type)
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateProperty(name: "test", type: null as Type),
            "Generating the source code for a property with 'null' as type should throw an exception.");
    }

    [Test]
    public void CreateTypeParameter_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter(name: null),
            "Creating type parameter with name 'null' should throw.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter(name: string.Empty),
            "Creating a type parameter with an empty string as name should throw.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter(name: "  "),
            "Creating a type parameter with a whitespace string as name should throw.");
    }

    [Test]
    public void CreateTypeParameter_WithInvalidConstraints_Throws()
    {
        // CreateTypeParameter(string, params)
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter("T", null as string[]));

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter("T", new string[] { null }));

        // CreateTypeParameter(string, IEnumerable)
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter("T", null as IEnumerable<string>));

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter("T", new List<string> { null }));
    }
}
