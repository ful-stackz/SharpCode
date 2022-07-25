using System;
using NUnit.Framework;

namespace SharpCode.Test;

[TestFixture]
public class CodeTests
{
    [Test]
    public void CreateNamespace_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateNamespace(name: null).ToSourceCode(),
            "Generating the source code for a namespace with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateNamespace(name: string.Empty).ToSourceCode(),
            "Generating the source code for a namespace with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateNamespace(name: "  ").ToSourceCode(),
            "Generating the source code for a namespace with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateClass_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateClass(name: null).ToSourceCode(),
            "Generating the source for a class with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateClass(name: string.Empty).ToSourceCode(),
            "Generating the source for a class with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateClass(name: "  ").ToSourceCode(),
            "Generating the source for a class with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateEnum_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateEnum(name: null).ToSourceCode(),
            "Generating the source for an enum with null as a name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnum(name: string.Empty).ToSourceCode(),
            "Generating the source for an enum with an empty name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnum(name: "  ").ToSourceCode(),
            "Generating the source for an enum with a whitespace name should throw an exception.");
    }

    [Test]
    public void CreateEnumMember_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateEnumMember(null).Build(),
            "Generating the source code for an enum member with null as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnumMember(string.Empty).Build(),
            "Generating the source code for an enum member with an empty string as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateEnumMember("  ").Build(),
            "Generating the source code for an enum member with whitespace name should throw an exception.");
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
    public void CreateProperty_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateProperty(name: null, type: "string").ToSourceCode(),
            "Generating the source code for a property with 'null' as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateProperty(name: string.Empty, type: "string").ToSourceCode(),
            "Generating the source code for a property with an empty string as name should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateProperty(name: "   ", type: "string").ToSourceCode(),
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
}
