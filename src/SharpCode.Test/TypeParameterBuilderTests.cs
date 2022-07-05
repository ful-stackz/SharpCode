using System;
using NUnit.Framework;

namespace SharpCode.Test;

[TestFixture]
public class TypeParameterBuilderTests
{
    [Test]
    public void CreateTypeParameter_WithoutRequiredSettings_Throws()
    {
        Assert.Throws<MissingBuilderSettingException>(
            () => Code.CreateTypeParameter().Build(),
            "Generating the source code for a type parameter without setting the name should throw an exception.");

        Assert.Throws<MissingBuilderSettingException>(
            () => Code.CreateTypeParameter().WithName(string.Empty).Build(),
            "Generating the source code for a type parameter with an empty string as name should throw an exception.");

        Assert.Throws<MissingBuilderSettingException>(
            () => Code.CreateTypeParameter().WithName("  ").Build(),
            "Generating the source code for a type parameter with whitespace name should throw an exception.");

        Assert.Throws<ArgumentNullException>(
            () => Code.CreateTypeParameter().WithName(null).Build(),
            "Generating the source code for a type parameter with null as name should throw an exception.");
    }
}
