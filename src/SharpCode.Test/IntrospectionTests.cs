using System;
using NUnit.Framework;

namespace SharpCode.Test
{
    [TestFixture]
    public class IntrospectionTests
    {
        [Test]
        public void ClassBuilder_HasMember_WorksWithFields()
        {
            var builder = Code.CreateClass(name: "Test")
                .WithFields(
                    Code.CreateField("string", "_name"),
                    Code.CreateField("bool", "_hasValue"));

            Assert.IsTrue(builder.HasMember("_name"));
            Assert.IsTrue(builder.HasMember("_name", MemberType.Field));

            Assert.IsTrue(builder.HasMember("_HaSValUE", MemberType.Field));
            Assert.IsFalse(builder.HasMember("_HaSValUE", MemberType.Field, comparison: StringComparison.Ordinal));

            Assert.IsFalse(builder.HasMember("_hasValue", MemberType.Property));
        }

        [Test]
        public void ClassBuilder_HasMember_WorksWithProperties()
        {
            var builder = Code.CreateClass(name: "Test")
                .WithProperties(
                    Code.CreateProperty("string", "Name"),
                    Code.CreateProperty("bool", "HasValue"));

            Assert.IsTrue(builder.HasMember("Name"));
            Assert.IsTrue(builder.HasMember("Name", MemberType.Property));

            Assert.IsTrue(builder.HasMember("HaSValUE", MemberType.Property));
            Assert.IsFalse(builder.HasMember("HaSValUE", MemberType.Property, comparison: StringComparison.Ordinal));

            Assert.IsFalse(builder.HasMember("Name", MemberType.Field));
        }

        [Test]
        public void ClassBuilder_HasMember_WorksWithAnyMember()
        {
            var builder = Code.CreateClass(name: "Test")
                .WithFields(
                    Code.CreateField("string", "_name"),
                    Code.CreateField("int", "_count"))
                .WithProperties(
                    Code.CreateProperty("string", "Name"),
                    Code.CreateProperty("bool", "HasValue"));

            Assert.IsTrue(builder.HasMember("_name"));
            Assert.IsFalse(builder.HasMember("_name", MemberType.Property));

            Assert.IsTrue(builder.HasMember("_count", MemberType.Field));
            Assert.IsTrue(builder.HasMember("_COUNT", MemberType.Field));

            Assert.IsTrue(builder.HasMember("Name"));
            Assert.IsTrue(builder.HasMember("Name", MemberType.Property));
            Assert.IsFalse(builder.HasMember("Name", MemberType.Field));

            Assert.IsTrue(builder.HasMember("HaSValUE", MemberType.Property));
            Assert.IsFalse(builder.HasMember("HaSValUE", MemberType.Field));
        }

        [Test]
        public void EnumBuilder_HasMember_Works()
        {
            var builder = Code.CreateEnum(name: "Test").WithMembers(
                Code.CreateEnumMember("None"),
                Code.CreateEnumMember("Some"));

            Assert.IsTrue(builder.HasMember("none"));
            Assert.IsTrue(builder.HasMember("some"));

            Assert.IsFalse(builder.HasMember("any"));
            Assert.IsFalse(builder.HasMember("NONE", StringComparison.Ordinal));
        }

        [Test]
        public void StructBuilder_HasMember_Works()
        {
            var builder = Code.CreateStruct(name: "Test")
                .WithFields(
                    Code.CreateField("int", "_x"),
                    Code.CreateField("int", "_y"),
                    Code.CreateField("string", "_title"))
                .WithProperties(
                    Code.CreateProperty("int", "X").WithGetter("_x").WithSetter("_x"),
                    Code.CreateProperty("int", "Y").WithGetter("_y").WithSetter("_y"),
                    Code.CreateProperty("string", "Title").WithGetter("_title").WithSetter("_title"));

            Assert.IsTrue(builder.HasMember("_x"));
            Assert.IsTrue(builder.HasMember("_y"));
            Assert.IsTrue(builder.HasMember("_title"));
            Assert.IsTrue(builder.HasMember("_x", MemberType.Field));
            Assert.IsTrue(builder.HasMember("_y", MemberType.Field));
            Assert.IsTrue(builder.HasMember("_title", MemberType.Field));

            Assert.IsTrue(builder.HasMember("X"));
            Assert.IsTrue(builder.HasMember("Y"));
            Assert.IsTrue(builder.HasMember("Title"));
            Assert.IsTrue(builder.HasMember("X", MemberType.Property));
            Assert.IsTrue(builder.HasMember("Y", MemberType.Property));
            Assert.IsTrue(builder.HasMember("Title", MemberType.Property));

            Assert.IsFalse(builder.HasMember("_x", MemberType.Property));
            Assert.IsFalse(builder.HasMember("_y", MemberType.Property));
            Assert.IsFalse(builder.HasMember("_title", MemberType.Property));

            Assert.IsFalse(builder.HasMember("X", MemberType.Field));
            Assert.IsFalse(builder.HasMember("Y", MemberType.Field));
            Assert.IsFalse(builder.HasMember("Title", MemberType.Field));
        }

        [Test]
        public void InterfaceBuilder_HasMember_Works()
        {
            var builder = Code.CreateInterface("ITest").WithProperties(
                Code.CreateProperty(typeof(string), "Prefix"),
                Code.CreateProperty(typeof(string), "Suffix"),
                Code.CreateProperty(typeof(string), "Name"));

            Assert.IsTrue(builder.HasMember("Prefix"));
            Assert.IsTrue(builder.HasMember("Suffix", MemberType.Property));
            Assert.IsFalse(builder.HasMember("Name", MemberType.Field));
        }

        [Test]
        public void NamespaceBuilder_HasMember_Works()
        {
            var builder = Code.CreateNamespace("Container")
                .WithClass(Code.CreateClass("TestClass"))
                .WithEnum(Code.CreateEnum("TestEnum"))
                .WithInterface(Code.CreateInterface("ITest"))
                .WithStruct(Code.CreateStruct("TestStruct"));

            Assert.IsTrue(builder.HasMember("TestClass", MemberType.Class));
            Assert.IsTrue(builder.HasMember("TestEnum", MemberType.Enum));
            Assert.IsTrue(builder.HasMember("ITest", MemberType.Interface));
            Assert.IsTrue(builder.HasMember("TestStruct", MemberType.Struct));
        }
    }
}
