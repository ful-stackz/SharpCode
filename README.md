# SharpCode
Small C# code generator. Easily generate code programmatically!

[![CI Status](https://img.shields.io/github/workflow/status/ful-stackz/SharpCode/CI?label=CI&logo=github&style=flat)](https://github.com/ful-stackz/SharpCode/actions?query=workflow%3ACI)
[![Nuget](https://img.shields.io/nuget/v/SharpCode?color=success&label=nuget&logo=nuget&style=flat)](https://www.nuget.org/packages/SharpCode/)
[![codecov](https://codecov.io/gh/ful-stackz/SharpCode/branch/main/graph/badge.svg?token=F2E4FV2DA3)](https://codecov.io/gh/ful-stackz/SharpCode)

## Install

- .NET CLI `dotnet add package SharpCode`
- Package Manager `Install-Package SharpCode`
- Package Reference `<PackageReference Include="SharpCode" Version="0.3.0" />`

## Usage

<details>
    <summary>Simple usage</summary>

```csharp
using SharpCode;

var sourceCode = Code.CreateNamespace("Data")
    .WithClass(Code.CreateClass("User")
        .WithProperty(Code.CreateProperty("int", "Id"))
        .WithProperty(Code.CreateProperty("string", "Username")))
    .ToSourceCode();

System.IO.File.WriteAllText("User.cs", sourceCode);

// User.cs
namespace Data
{
    public class User
    {
        public int Id
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }
    }
}
```
</details>

<details>
    <summary>Extended usage</summary>

```csharp
using SharpCode;

var dataNamespace = Code.CreateNamespace("Data");

var userDetailsClass = Code.CreateClass("UserDetails", AccessModifier.Public)
    .WithField(Code.CreateField("int", "_id", AccessModifier.Private).MakeReadonly())
    .WithField(Code.CreateField("string", "_username", AccessModifier.Private).MakeReadonly())
    .WithConstructor(Code.CreateConstructor()
        .WithAccessModifier(AccessModifier.Public)
        .WithParameter("int", "id", "_id")
        .WithParameter("string", "username", "_username"))
    .WithProperty(Code.CreateProperty("int", "Id", AccessModifier.Public)
        .WithGetter("_id")
        .WithoutSetter())
    .WithProperty(Code.CreateProperty("string", "Username", AccessModifier.Public)
        .WithGetter("_username")
        .WithoutSetter());

var userClass = Code.CreateClass("User", AccessModifier.Public)
    .WithProperty(Code.CreateProperty("UserDetails", "Details", AccessModifier.Public)
        .WithoutSetter())
    .WithConstructor(Code.CreateConstructor()
        .WithAccessModifier(AccessModifier.Public)
        .WithParameter("UserDetails", "details", "Details"));


System.IO.File.WriteAllText(
    "User.cs",
    dataNamespace
        .WithClass(userDetailsClass)
        .WithClass(userClass)
        .ToSourceCode());

// User.cs
namespace Data
{
    public class UserDetails
    {
        private readonly int _id;
        private readonly string _username;
        public UserDetails(int id, string username)
        {
            _id = id;
            _username = username;
        }

        public int Id
        {
            get => _id;
        }

        public string Username
        {
            get => _username;
        }
    }

    public class User
    {
        public User(UserDetails details)
        {
            Details = details;
        }

        public UserDetails Details
        {
            get;
        }
    }
}
```
</details>

## Samples

The [samples](https://github.com/ful-stackz/SharpCode/tree/main/samples) folder contains sample projects which make use of `SharpCode`. The projects are fully functional
and can be played around with. Check the `samples/README.md` for more information.

## Features

With `SharpCode` you can programmatically generate source code for a lot of C# structures. When generating the source
code of any structure `SharpCode` performs basic validation to ensure a level of correctness of the produced source
code.

<details>
<summary>Generating namespace source code</summary>

```csharp
// ✔ Define the name of the namespace
namespace Data
{
    // ✔ Interface members with different access modifiers
    public interface IHasData {}

    // ✔ Class members with different access modifiers
    public class Data {}

    // ✔ Struct members with different access modifiers
    public struct DataPoint {}

    // ✔ Enum members with different access modifiers
    public enum DataType {}
}
```
</details>

<details>
<summary>Generating interface source code</summary>

```csharp
// ✔ Define XML summary docs
// ✔ Define the name of the interface
// ✔ Define a list of implemented interfaces
/// <summary>
/// Docs!
/// </summary>
public interface IHasData : IImplementedInterface
{
    // ✔ Define property members
    int Count { get; set; }
}
```
</details>

<details>
<summary>Generating class source code</summary>

```csharp
// ✔ Define XML summary docs
// ✔ Define the name of the class
// ✔ Define an inherited class
// ✔ Define a list of implemented interfaces
/// <summary>
/// Docs!
/// </summary>
public class Data : DataBase, IHasData
{
    // ✔ Define field members
    private int _count;

    // ✔ Define constructors
    // ✔ Define constructor XML summary docs
    // ✔ Define constructor parameters
    // ✔ Define accepting fields for constructor parameters
    // ✔ Define base calls with parameters
    /// <summary>
    /// Docs!
    /// </summary>
    public Data(int count) : DataBase(count)
    {
        _count = count;
    }

    // ✔ Define property members
    // ✔ Define property XML summary docs
    // ✔ Define custom getter/setter for properties
    /// <summary>
    /// Docs!
    /// </summary>
    public int Count
    {
        get => _count;
        set => _count = value;
    }

    // ✔ Define auto-implemented properties
    public string Name { get; set; }
}
```
</details>

<details>
<summary>Generating struct source code</summary>

```csharp
// ✔ Define XML summary docs
// ✔ Define the name of the struct
// ✔ Define a list of implemented interfaces
/// <summary>
/// Docs!
/// </summary>
public struct DataPoint : IHasData
{
    // ✔ Define field members
    private int _count;

    // ✔ Define constructors
    // ✔ Define constructor XML summary docs
    // ✔ Define constructor parameters
    // ✔ Define accepting fields for constructor parameters
    /// <summary>
    /// Docs!
    /// </summary>
    public Data(int count)
    {
        _count = count;
    }

    // ✔ Define property members
    // ✔ Define property XML summary docs
    // ✔ Define custom getter/setter for properties
    /// <summary>
    /// Docs!
    /// </summary>
    public int Count
    {
        get => _count;
        set => _count = value;
    }
}
```
</details>

<details>
<summary>Generating enum source code</summary>

```csharp
// ✔ Define XML summary docs
// ✔ Define the name of the enum
/// <summary>
/// Docs!
/// </summary>
public enum DataType
{
    // ✔ Define enum members
    // ✔ Define XML summary docs for enum members
    // ✔ Define explicit values for enum members
    /// <summary>
    /// Docs!
    /// </summary>
    Invalid = 0,
    Incomplete,
    Complete,
}
```

```csharp
// ✔ Define enum as flags
// ✔ Auto generated flags-compatible values for enum members
[System.Flags]
public enum ExampleFlags
{
    None = 0,
    A = 1,
    B = 2,
    C = 4,
    D = 8,
}
```
</details>

## Development

You don't need anything special for local development.

- `dotnet build ./src/SharpCode` - will build you your own local `SharpCode`
- `dotnet add <path to your project> reference <path to ./src/SharpCode>` - will add `SharpCode` as a local reference to your project
- `dotnet test ./src/SharpCode.Test` - will run the tests
- `dotnet watch test ./src/SharpCode.Test` - will run the tests everytime you make a change to the source code
of `SharpCode` itself or the tests

## Contributing

This library is still in its early stages and being figured out. Many changes are expected and incoming. For your own
sake please don't work on any big changes as the files might disappear before you're ready.

Nonetheless, insights into the usage of the library, structure and general suggestions are always welcome! Thank you!
