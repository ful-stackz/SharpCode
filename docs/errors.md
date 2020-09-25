# Errors

## Summary

When building source code from the provided configurations, `SharpCode` performs basic validation. If it determines
that the resulting source code will be invalid it will throw either a `MissingBuilderSettingException` or a
`SyntaxException`. This document describes the causes of the exceptions.

- `MissingBuilderSettingException` - [go to](#missing-setting-exceptions)
- `SyntaxException` - [go to](#syntax-exceptions)

## Missing setting exceptions

The `MissingBuilderSettingException` is thrown when the source code builder has not been provided with a required
setting. Take a look at the following example:

```csharp
Code.CreateClass().ToSourceCode();
// throws MissingBuilderSettingException
```

In the snippet above we are creating a `ClassBuilder` instance with `Code.CreateClass()` and invoking `.ToSourceCode()`
on it, which normally produces the source code representation of a class. In this scenario however a
`MissingBuilderSettingException` is thrown, because we have not provided the `name` of the class. Since every class
must have its own name the builder cannot produce valid source code. Changing the snippet to the following fixes this issue.

```csharp
Code.CreateClass(name: "User").ToSourceCode()
// or
Cdde.CreateClass().WithName("User").ToSourceCode();
```

### Validated scenarios

- Providing the type of the field is required when building a field.
- Providing the name of the field is required when building a field.
- Providing the name of the property is required when building a property.
- Providing the type of the property is required when building a property.
- Providing the name of the class is required when building a class.
- Providing the name of the interface is required when building an interface.
- Providing the name of the namespace is required when building a namespace.

## Syntax exceptions

The `SyntaxException` is thrown when the source code builder detects a configuration that results in invalid source
code. Take a look at the following example:

```csharp
Code.CreateClass()
  .WithName("Config")
  .MakeStatic()
  .WithConstructor(Code.CreateConstructor().WithAccessModifier(AccessModifier.Public))
  .ToSourceCode()
// throws SyntaxException
```

In the snippet above we are:

1. Creating a `ClassBuilder` instance with `Code.CreateClass()`
2. Naming the class `Config` with `.WithName("Config")`
3. Making the class static with `.MakeStatic()`
4. Creating a `ConstructorBuilder` with `Code.CreateConstructor()`
5. Making the constructor `public` with `.WithAccessModifier(AccessModifier.Public)`
6. Adding a the public constructor to the class with `.WithConstructor()`

If we ask the builder to give us the source code without any validation (which is not possible), we would get:

```csharp
public static class Config
{
    public static Config()
    {
    }
}
```

The source code for the `static class Config` is invalid, since `static` classes cannot have constructors with access
modifiers. `SharpCode` detects that and throws the `SyntaxException`.

### Validated scenarios

- Properties with auto implemented setters must also have auto implemented getters. (CS8051)
- Properties with custom getters cannot have auto implemented setters.
- Only auto implemented properties can have a default value. (CS8050)
- Access modifiers are not allowed on static constructors. (CS0515)
- Parameters are not allowed on static constructors. (CS0132)
- Static constructors cannot call base constructors. (CS0514)
- Static classes can have only 1 constructor.
- Interface properties cannot have a default value. (CS8053)
- Interface properties should have at least a getter or a setter.
- Interface properties can only define an auto implemented getter.
- Interface properties can only define an auto implemented setter.
- A class defined under a namespace cannot have the access modifier '{access-modifier}'.
- An interface defined under a namespace cannot have the access modifier '{access-modifier}'.
