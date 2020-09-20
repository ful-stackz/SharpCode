# SharpCode
Small C# code generator. Easily generate code programmatically!

![CI](https://github.com/ful-stackz/SharpCode/workflows/CI/badge.svg?branch=main)

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
