# SharpCode
Small C# code generator. Easily generate code programmatically!

[![CI Status](https://img.shields.io/github/workflow/status/ful-stackz/SharpCode/CI?label=CI&logo=github&style=flat)](https://github.com/ful-stackz/SharpCode/actions?query=workflow%3ACI)
[![Nuget](https://img.shields.io/nuget/v/SharpCode?color=success&label=nuget&logo=nuget&style=flat)](https://www.nuget.org/packages/SharpCode/)

## Install

- .NET CLI `dotnet add package SharpCode`
- Package Manager `Install-Package SharpCode`
- Package Reference `<PackageReference Include="SharpCode" Version="0.0.1" />`

## Usage

### Simple usage

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

### Extended usage

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
