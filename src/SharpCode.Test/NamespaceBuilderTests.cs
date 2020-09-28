using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class NamespaceBuilderTests
    {
        [Test]
        public void CreateNamespace_WithNoMembers_Works()
        {
            var expectedCode = @"
namespace Generated
{
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, Code.CreateNamespace().WithName("Generated").ToSourceCode().WithUnixEOL());
            Assert.AreEqual(expectedCode, Code.CreateNamespace("Generated").ToSourceCode().WithUnixEOL());
        }

        [Test]
        public void CreateNamespace_WithUsings_Works()
        {
            var expectedCode = @"
using System;
using System.Collections.Generic;

namespace Generated
{
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateNamespace("Generated")
                .WithUsing("System")
                .WithUsing("System.Collections.Generic;")
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateNamespace_WithClasses_Works()
        {
            var expectedCode = @"
namespace Generated
{
    public class Car
    {
    }

    internal class Motorbike
    {
    }
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateNamespace("Generated")
                .WithClass(Code.CreateClass("Car"))
                .WithClass(Code.CreateClass("Motorbike", AccessModifier.Internal))
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateNamespace_WithUsingsAndClasses_Works()
        {
            var expectedCode = @"
using System;
using System.Speed;

namespace Vehicles
{
    public class Vehicle
    {
        private readonly int _wheels;
        public Vehicle(int wheels)
        {
            _wheels = wheels;
        }

        public int Wheels
        {
            get
            {
                return _wheels;
            }
        }
    }

    public class Car : Vehicle
    {
        private readonly int _maxSpeed;
        public Car(int maxSpeed): base(4)
        {
            _maxSpeed = maxSpeed;
        }

        public int MaxSpeed
        {
            get => _maxSpeed;
        }
    }

    public class Motorbike : Vehicle
    {
        private readonly int _maxSpeed;
        public Motorbike(int maxSpeed): base(2)
        {
            _maxSpeed = maxSpeed;
        }

        public int MaxSpeed
        {
            get => _maxSpeed;
        }
    }
}
            ".Trim().WithUnixEOL();

            var generatedCode = Code.CreateNamespace()
                .WithName("Vehicles")
                .WithUsing("System")
                .WithUsing("System.Speed;")
                .WithClass(Code.CreateClass("Vehicle")
                    .WithField(Code.CreateField("int", "_wheels")
                        .MakeReadonly())
                    .WithConstructor(Code.CreateConstructor()
                        .WithParameter("int", "wheels", "_wheels"))
                    .WithProperty(Code.CreateProperty("int", "Wheels")
                        .WithGetter("{ return _wheels; }")
                        .WithoutSetter()))
                .WithClass(Code.CreateClass("Car")
                    .WithInheritedClass("Vehicle")
                    .WithField(Code.CreateField("int", "_maxSpeed")
                        .MakeReadonly())
                    .WithConstructor(Code.CreateConstructor()
                        .WithParameter("int", "maxSpeed", "_maxSpeed")
                        .WithBaseCall("4"))
                    .WithProperty(Code.CreateProperty("int", "MaxSpeed")
                        .WithGetter("_maxSpeed")
                        .WithoutSetter()))
                .WithClass(Code.CreateClass("Motorbike")
                    .WithInheritedClass("Vehicle")
                    .WithField(Code.CreateField("int", "_maxSpeed")
                        .MakeReadonly())
                    .WithConstructor(Code.CreateConstructor()
                        .WithParameter("int", "maxSpeed", "_maxSpeed")
                        .WithBaseCall("2"))
                    .WithProperty(Code.CreateProperty("int", "MaxSpeed")
                        .WithGetter("_maxSpeed")
                        .WithoutSetter()))
                .ToSourceCode()
                .WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingNamespace_WithEnum_Works()
        {
            var generatedCode = Code.CreateNamespace(name: "GeneratedCode")
                .WithEnum(Code.CreateEnum(name: "Level")
                    .WithMembers(
                        Code.CreateEnumMember("Low"),
                        Code.CreateEnumMember("Medium"),
                        Code.CreateEnumMember("High")))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
namespace GeneratedCode
{
    public enum Level
    {
        Low,
        Medium,
        High,
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreatingNamespace_WithFlagsEnum_Works()
        {
            var generatedCode = Code.CreateNamespace(name: "GeneratedCode")
                .WithEnum(Code.CreateEnum(name: "Level")
                    .WithMembers(
                        Code.CreateEnumMember("Off"),
                        Code.CreateEnumMember("VeryLow"),
                        Code.CreateEnumMember("Low"),
                        Code.CreateEnumMember("Medium"),
                        Code.CreateEnumMember("High"),
                        Code.CreateEnumMember("ExtraHigh"),
                        Code.CreateEnumMember("Max"))
                    .MakeFlagsEnum())
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
namespace GeneratedCode
{
    [System.Flags]
    public enum Level
    {
        Off = 0,
        VeryLow = 1,
        Low = 2,
        Medium = 4,
        High = 8,
        ExtraHigh = 16,
        Max = 32,
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }

        [Test]
        public void CreateClass_Throws_WhenRequiredSettingsNotProvided()
        {
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace().ToSourceCode(),
                "Generating the source code for a namespace without a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace(name: null).ToSourceCode(),
                "Generating the source code for a namespace with null as a name should throw an exception.");
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace().WithName(null).ToSourceCode(),
                "Generating the source code for a namespace with null as a name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace(name: string.Empty).ToSourceCode(),
                "Generating the source code for a namespace with an empty name should throw an exception.");
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace().WithName(string.Empty).ToSourceCode(),
                    "Generating the source code for a namespace with an empty name should throw an exception.");

            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace(name: "  ").ToSourceCode(),
                "Generating the source code for a namespace with a whitespace name should throw an exception.");
            Assert.Throws<MissingBuilderSettingException>(
                () => Code.CreateNamespace().WithName("  ").ToSourceCode(),
                "Generating the source code for a namespace with a whitespace name should throw an exception.");
        }

        [Test]
        public void CreatingNamespace_ValidatesMembersAccessModifiers()
        {
            // -- Classes
            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithClass(Code.CreateClass("Test", AccessModifier.Private))
                    .ToSourceCode(),
                "Generating the source code for a private class inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithClass(Code.CreateClass("Test", AccessModifier.PrivateInternal))
                    .ToSourceCode(),
                "Generating the source code for a private internal class inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithClass(Code.CreateClass("Test", AccessModifier.Protected))
                    .ToSourceCode(),
                "Generating the source code for a protected class inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithClass(Code.CreateClass("Test", AccessModifier.ProtectedInternal))
                    .ToSourceCode(),
                "Generating the source code for a protected internal class inside a namespace should throw an exception.");

            // -- Interfaces
            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithInterface(Code.CreateInterface("ITest", AccessModifier.Private))
                    .ToSourceCode(),
                "Generating the source code for a private interface inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithInterface(Code.CreateInterface("ITest", AccessModifier.PrivateInternal))
                    .ToSourceCode(),
                "Generating the source code for a private internal interface inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithInterface(Code.CreateInterface("ITest", AccessModifier.Protected))
                    .ToSourceCode(),
                "Generating the source code for a protected interface inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithInterface(Code.CreateInterface("ITest", AccessModifier.ProtectedInternal))
                    .ToSourceCode(),
                "Generating the source code for a protected internal interface inside a namespace should throw an exception.");

            // -- Enums
            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithEnum(Code.CreateEnum("Test", AccessModifier.Private))
                    .ToSourceCode(),
                "Generating the source code for a private enum inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithEnum(Code.CreateEnum("Test", AccessModifier.PrivateInternal))
                    .ToSourceCode(),
                "Generating the source code for a private internal enum inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithEnum(Code.CreateEnum("Test", AccessModifier.Protected))
                    .ToSourceCode(),
                "Generating the source code for a protected enum inside a namespace should throw an exception.");

            Assert.Throws<SyntaxException>(
                () => Code.CreateNamespace("Test")
                    .WithEnum(Code.CreateEnum("Test", AccessModifier.ProtectedInternal))
                    .ToSourceCode(),
                "Generating the source code for a protected internal enum inside a namespace should throw an exception.");
        }

        [Test]
        public void CreatingNamespace_WithInterfaces_Works()
        {
            var generatedCode = Code.CreateNamespace("GeneratedGoodies")
                .WithInterface(Code.CreateInterface("IHaveGoodies")
                    .WithProperty(Code.CreateProperty("int", "Count")))
                .WithInterface(Code.CreateInterface("IHaveHiddenGoodies", AccessModifier.Internal)
                    .WithImplementedInterface("IHaveGoodies")
                    .WithProperty(Code.CreateProperty("int", "HiddenCount")))
                .ToSourceCode()
                .WithUnixEOL();

            var expectedCode = @"
namespace GeneratedGoodies
{
    public interface IHaveGoodies
    {
        int Count
        {
            get;
            set;
        }
    }

    internal interface IHaveHiddenGoodies : IHaveGoodies
    {
        int HiddenCount
        {
            get;
            set;
        }
    }
}
            ".Trim().WithUnixEOL();

            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}
