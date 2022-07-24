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
}