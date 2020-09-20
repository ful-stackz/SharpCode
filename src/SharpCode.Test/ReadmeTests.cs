using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class ReadmeTests
    {
        [Test]
        public void SimpleExample()
        {
            var generatedCode = Code.CreateNamespace("Data")
                .WithClass(Code.CreateClass("User")
                    .WithProperty(Code.CreateProperty("int", "Id"))
                    .WithProperty(Code.CreateProperty("string", "Username")))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
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
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void ExtentedUsageExample()
        {
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

            var generatedCode = dataNamespace
                .WithClass(userDetailsClass)
                .WithClass(userClass)
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
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
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}