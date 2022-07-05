using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;

namespace SharpCode;

public class TypeParameterBuilder
{
    public TypeParameterBuilder()
    {
    }

    public TypeParameterBuilder(string name, Option<IEnumerable<string>> constraints = default)
    {
        TypeParameter = new TypeParameter(name: Option.Some(name), constraints.Map(x => x.ToList()));
    }

    internal TypeParameter TypeParameter { get; private set; } = new TypeParameter(name: Option.None<string>());

    public TypeParameterBuilder WithName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        TypeParameter = TypeParameter.With(name: Option.Some(name));
        return this;
    }

    public TypeParameterBuilder WithConstraint(string constraint)
    {
        if (string.IsNullOrWhiteSpace(constraint))
        {
            throw new ArgumentNullException(nameof(constraint));
        }

        TypeParameter.Constraints.Add(constraint);
        return this;
    }

    public TypeParameterBuilder WithConstraints(params string[] constraints)
    {
        if (constraints.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("One of the constraints value is null or empty.");
        }

        TypeParameter.Constraints.AddRange(constraints);
        return this;
    }

    public TypeParameterBuilder WithConstraints(IEnumerable<string> constraints)
    {
        if (constraints is null)
        {
            throw new ArgumentNullException(nameof(constraints));
        }

        if (constraints.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("One of the constraints value is null or empty.");
        }

        TypeParameter.Constraints.AddRange(constraints);
        return this;
    }

    internal TypeParameter Build()
    {
        if (string.IsNullOrWhiteSpace(TypeParameter.Name.ValueOrDefault()))
        {
            throw new MissingBuilderSettingException(
                "Providing the name of the type parameter is required when building a type parameter.");
        }

        return TypeParameter;
    }
}

/// <summary>
/// For more information about type paramater constraints, see the official documentation
/// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters .
/// </summary>
public static class TypeParameterConstraint
{
    /// <summary>
    /// The type argument must be a non-nullable value type.
    /// For information about nullable value types, see Nullable value types.
    /// Because all value types have an accessible parameterless constructor,
    /// the struct constraint implies the new() constraint and can't be combined
    /// with the new() constraint. You can't combine the struct constraint with
    /// the unmanaged constraint.
    /// </summary>
    public const string Struct = "struct";

    /// <summary>
    /// The type argument must be a reference type. This constraint applies also
    /// to any class, interface, delegate, or array type. In a nullable context
    /// in C# 8.0 or later, T must be a non-nullable reference type.
    /// </summary>
    public const string Class = "class";

    /// <summary>
    /// The type argument must be a reference type, either nullable or
    /// non-nullable. This constraint applies also to any class, interface,
    /// delegate, or array type.
    /// </summary>
    public const string NullableClass = "class?";

    /// <summary>
    /// The type argument must be a non-nullable type. The argument can be a
    /// non-nullable reference type in C# 8.0 or later, or a non-nullable value
    /// type.
    /// </summary>
    public const string NotNull = "notnull";

    /// <summary>
    /// This constraint resolves the ambiguity when you need to specify an
    /// unconstrained type parameter when you override a method or provide an
    /// explicit interface implementation. The default constraint implies the
    /// base method without either the class or struct constraint.
    /// For more information, see the default constraint spec proposal.
    /// </summary>
    public const string Default = "default";

    /// <summary>
    /// The type argument must be a non-nullable unmanaged type. The unmanaged
    /// constraint implies the struct constraint and can't be combined with
    /// either the struct or new() constraints.
    /// </summary>
    public const string Unmanaged = "unmanaged";

    /// <summary>
    /// The type argument must have a public parameterless constructor. When
    /// used together with other constraints, the new() constraint must be
    /// specified last. The new() constraint can't be combined with the struct
    /// and unmanaged constraints.
    /// </summary>
    public const string New = "new()";
}
