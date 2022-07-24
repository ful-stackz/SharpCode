using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpCode.Test;

[TestFixture]
public class TypeParameterBuilderTests
{
    [Test]
    public void CreateTypeParameter_WithoutConstraints_Works()
    {
        var typeParameter = Code.CreateTypeParameter().WithName("TValue").Build();
        Assert.AreEqual("TValue", typeParameter.Name.ValueOr(string.Empty));
        Assert.AreEqual(0, typeParameter.Constraints.Count);
    }

    [Test]
    public void CreateTypeParameter_WithConstraints_Works()
    {
        var typeParameter = Code.CreateTypeParameter()
            .WithName("TValue")
            .WithConstraint(TypeParameterConstraint.Class)
            .WithConstraints(TypeParameterConstraint.Default)
            .WithConstraints(new List<string> { TypeParameterConstraint.New })
            .Build();
        Assert.AreEqual("TValue", typeParameter.Name.ValueOr(string.Empty));
        Assert.AreEqual(3, typeParameter.Constraints.Count);
        Assert.Contains(TypeParameterConstraint.Class, typeParameter.Constraints);
        Assert.Contains(TypeParameterConstraint.Default, typeParameter.Constraints);
        Assert.Contains(TypeParameterConstraint.New, typeParameter.Constraints);
    }

    [Test]
    public void CreateTypeParameter_WithoutRequiredSettings_Throws()
    {
        Assert.Throws<MissingBuilderSettingException>(
            () => Code.CreateTypeParameter().Build(),
            "Generating the source code for a type parameter without setting the name should throw an exception.");
    }

    [Test]
    public void CreateTypeParameter_WithInvalidName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter().WithName(null),
            "Setting type parameter name to 'null' should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithName(string.Empty),
            "Setting type parameter name to an empty string should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithName("  "),
            "Setting type parameter name to a whitespace string should throw an exception.");
    }

    [Test]
    public void CreateTypeParameter_WithInvalidConstraint_Throws()
    {
        // WithConstraint(string constraint) API
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter().WithConstraint(null),
            "Setting type parameter constraint to 'null' should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithConstraint(string.Empty),
            "Setting type parameter constraint to an empty string should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithConstraint("  "),
            "Setting type parameter constraint to a whitespace string should throw an exception.");

        // WithConstraints(params string[] constraints) API
        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithConstraints("IEnumerable", string.Empty),
            "Setting type parameter constraint to an empty string should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithConstraints("IEnumerable", null),
            "Setting type parameter constraint to a 'null' string should throw an exception.");

        // WithConstraints(IEnumerable<string> constraints) API
        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter().WithConstraints(null as IEnumerable<string>),
            "Setting type parameter constraints to a null IEnumerable should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithConstraints(new List<string> { "IEnumerable", string.Empty }),
            "Setting type parameter constraint to an empty string should throw an exception.");

        Assert.Throws<ArgumentException>(
            () => Code.CreateTypeParameter().WithConstraints(new List<string> { "IEnumerable", null }),
            "Setting type parameter constraint to a 'null' string should throw an exception.");
    }
}
