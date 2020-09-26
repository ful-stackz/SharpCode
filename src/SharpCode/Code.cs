using System;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for creating various source code builders.
    /// </summary>
    public static class Code
    {
        /// <summary>
        /// Creates a new <see cref="NamespaceBuilder"/> instance for building namespaces.
        /// </summary>
        public static NamespaceBuilder CreateNamespace() =>
            new NamespaceBuilder();

        /// <summary>
        /// Creates a new pre-configured <see cref="NamespaceBuilder"/> instance for building namespaces. Configures
        /// the <see cref="NamespaceBuilder"/> instance with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the namespace. Used as-is.
        /// </param>
        public static NamespaceBuilder CreateNamespace(string name) =>
            new NamespaceBuilder(name);

        /// <summary>
        /// Creates a new <see cref="InterfaceBuilder"/> instance for building interface structures.
        /// </summary>
        public static InterfaceBuilder CreateInterface() =>
            new InterfaceBuilder();

        /// <summary>
        /// Creates a new pre-configured <see cref="InterfaceBuilder"/> instance for building interface structures.
        /// Configures the <see cref="InterfaceBuilder"/> with the specified <paramref name="name"/> and
        /// <paramref name="accessModifier"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the interface. Used as-is.
        /// </param>
        /// <param name="accessModifier">
        /// The access modifier of the interface.
        /// </param>
        public static InterfaceBuilder CreateInterface(
            string name,
            AccessModifier accessModifier = AccessModifier.Public) =>
            new InterfaceBuilder(name, accessModifier);

        /// <summary>
        /// Creates a new <see cref="ClassBuilder"/> instance for building class structures.
        /// </summary>
        public static ClassBuilder CreateClass() =>
            new ClassBuilder();

        /// <summary>
        /// Creates a new pre-configured <see cref="ClassBuilder"/> instance for building class structures. Configures
        /// the <see cref="ClassBuilder"/> with the specified <paramref name="name"/> and
        /// <paramref name="accessModifier"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the class. Used as-is.
        /// </param>
        /// <param name="accessModifier">
        /// The access modifier of the class.
        /// </param>
        public static ClassBuilder CreateClass(string name, AccessModifier accessModifier = AccessModifier.Public) =>
            new ClassBuilder(accessModifier, name);

        /// <summary>
        /// Creates a new <see cref="FieldBuilder"/> instance for building fields.
        /// </summary>
        public static FieldBuilder CreateField() =>
            new FieldBuilder();

        /// <summary>
        /// Creates a new pre-configured <see cref="FieldBuilder"/> instance for building fields. Configures the
        /// <see cref="FieldBuilder"/> with the specified <paramref name="type"/>, <paramref name="name"/> and
        /// <paramref name="accessModifier"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the field, eg. <c>int</c>, <c>string</c>, <c>User</c>. Used as-is.
        /// </param>
        /// <param name="name">
        /// The name of the field. Used as-is.
        /// </param>
        /// <param name="accessModifier">
        /// The access of modifier of the field.
        /// </param>
        public static FieldBuilder CreateField(
            string type,
            string name,
            AccessModifier accessModifier = AccessModifier.Private) =>
            new FieldBuilder(accessModifier, type, name);

        /// <summary>
        /// Creates a new pre-configured <see cref="FieldBuilder"/> instance for building fields. Configures the
        /// <see cref="FieldBuilder"/> with the specified <paramref name="type"/>, <paramref name="name"/> and
        /// <paramref name="accessModifier"/>.
        /// </summary>
        /// <param name="type">
        /// Used to derive the type of the field. The derived type of the field will use the framework keyword instead
        /// of the language keyword, eg. <c>Int32</c> instead of <c>int</c> and <c>String</c> instead of <c>string</c>.
        /// </param>
        /// <param name="name">
        /// The name of the field. Used as-is.
        /// </param>
        /// <param name="accessModifier">
        /// The access of modifier of the field.
        /// </param>
        public static FieldBuilder CreateField(
            Type type,
            string name,
            AccessModifier accessModifier = AccessModifier.Private) =>
            new FieldBuilder(accessModifier, type, name);

        /// <summary>
        /// Creates a new <see cref="ConstructorBuilder"/> instance for building constructors.
        /// </summary>
        public static ConstructorBuilder CreateConstructor(AccessModifier accessModifier = AccessModifier.Public) =>
            new ConstructorBuilder(accessModifier);

        /// <summary>
        /// Creates a new <see cref="PropertyBuilder"/> instance for building properties.
        /// </summary>
        public static PropertyBuilder CreateProperty() =>
            new PropertyBuilder();

        /// <summary>
        /// Creates a new pre-configured <see cref="PropertyBuilder"/> instance for building properties. Configures the
        /// <see cref="PropertyBuilder"/> with the specified <paramref name="type"/>, <paramref name="name"/> and
        /// <paramref name="accessModifier"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the property, eg. <c>int</c>, <c>string</c>, <c>User</c>. Used as-is.
        /// </param>
        /// <param name="name">
        /// The name of the property. Used as-is.
        /// </param>
        /// <param name="accessModifier">
        /// The access of modifier of the property.
        /// </param>
        public static PropertyBuilder CreateProperty(
            string type,
            string name,
            AccessModifier accessModifier = AccessModifier.Public) =>
            new PropertyBuilder(accessModifier, type, name);

        /// <summary>
        /// Creates a new pre-configured <see cref="PropertyBuilder"/> instance for building properties. Configures the
        /// <see cref="PropertyBuilder"/> with the specified <paramref name="type"/>, <paramref name="name"/> and
        /// <paramref name="accessModifier"/>.
        /// </summary>
        /// <param name="type">
        /// Used to derive the type of the property. The derived type of the field will use the framework keyword
        /// instead of the language keyword, eg. <c>Int32</c> instead of <c>int</c> and <c>String</c> instead of
        /// <c>string</c>.
        /// </param>
        /// <param name="name">
        /// The name of the property. Used as-is.
        /// </param>
        /// <param name="accessModifier">
        /// The access of modifier of the property.
        /// </param>
        public static PropertyBuilder CreateProperty(
            Type type,
            string name,
            AccessModifier accessModifier = AccessModifier.Public) =>
            new PropertyBuilder(accessModifier, type, name);
    }
}
