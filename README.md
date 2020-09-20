# SharpCode
Small C# code generator. Easily generate code programmatically!

## Simple usage

```csharp
using SharpCode;

var sourceCode = Code.CreateClass("User")
    .WithProperty(Code.CreateProperty("int", "Id"))
    .WithProperty(Code.CreateProperty("string", "Username"))
    .ToSourceCode();

System.IO.File.WriteAllText("User.cs", sourceCode);

// User.cs
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
```

## Extended usage

```csharp
using SharpCode;

var sourceCode = Code.CreateClass("User")
    .WithField(Code.CreateField("int", "_id"))
    .WithField(Code.CreateField("string", "_username"))
    .WithProperty(Code.CreateProperty("int", "Id")
        .WithGetter("_id")
        .WithoutSetter())
    .WithProperty(Code.CreateProperty("string", "Username")
        .WithGetter("_username")
        .WithoutSetter())
    .WithConstructor(Code.CreateConstructor()
        .WithParameter("int", "id", "_id")
        .WithParameter("string", "username", "_username"))
    .ToSourceCode();
    
System.IO.File.WriteAllText("User.cs", sourceCode);

// User.cs
public class User
{
    private int _id;
    private string _username;
    public User(int id, string username)
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
```
