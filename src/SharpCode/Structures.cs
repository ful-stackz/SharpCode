using System.Collections.Generic;
using Optional;

namespace SharpCode
{
    /// <summary>
    /// Represents the access modifier of a C# structure.
    /// </summary>
    public enum AccessModifier
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        None,
        Private,
        Internal,
        Protected,
        Public,
        PrivateProtected,
        ProtectedInternal,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Represents the various types of C# structure members.
    /// </summary>
    public enum MemberType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Interface,
        Class,
        Struct,
        Enum,
        EnumMember,
        Field,
        Property,
        UsingStatement,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    internal readonly struct TypeParameter
    {
        public TypeParameter(Option<string> name = default, Option<List<string>> constraints = default)
        {
            Name = name;
            Constraints = constraints.ValueOr(new List<string>());
        }

        public readonly Option<string> Name { get; }

        public readonly List<string> Constraints { get; }

        public TypeParameter With(Option<string> name) =>
            new TypeParameter(
                name: name.Else(Name),
                constraints: Option.Some(Constraints));
    }

    internal readonly struct Field
    {
        public Field(
            AccessModifier accessModifier,
            bool isReadonly = false,
            Option<string> type = default,
            Option<string> name = default,
            Option<string> summary = default,
            Option<List<TypeParameter>> typeParameters = default)
        {
            AccessModifier = accessModifier;
            IsReadonly = isReadonly;
            Type = type;
            Name = name;
            Summary = summary;
            TypeParameters = typeParameters.ValueOr(new List<TypeParameter>());
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly bool IsReadonly { get; }

        public readonly Option<string> Type { get; }

        public readonly Option<string> Name { get; }

        public readonly Option<string> Summary { get; }

        public readonly List<TypeParameter> TypeParameters { get; }

        public readonly Field With(
            Option<AccessModifier> accessModifier = default,
            Option<bool> isReadonly = default,
            Option<string> type = default,
            Option<string> name = default,
            Option<string> summary = default) =>
            new Field(
                accessModifier.ValueOr(AccessModifier),
                isReadonly.ValueOr(IsReadonly),
                type.Else(Type),
                name.Else(Name),
                summary.Else(Summary));
    }

    internal readonly struct Property
    {
        public const string AutoGetterSetter = "@auto";

        public Property(
            AccessModifier accessModifier,
            AccessModifier setterAccessModifier,
            bool isStatic = false,
            Option<string> type = default,
            Option<string> name = default,
            Option<string> summary = default,
            Option<string> defaultValue = default,
            Option<string> getter = default,
            Option<string> setter = default,
            Option<List<TypeParameter>> typeParameters = default)
        {
            AccessModifier = accessModifier;
            SetterAccessModifier = setterAccessModifier;
            IsStatic = isStatic;
            Type = type;
            Name = name;
            Summary = summary;
            DefaultValue = defaultValue;
            Getter = getter;
            Setter = setter;
            TypeParameters = typeParameters.ValueOr(new List<TypeParameter>());
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly AccessModifier SetterAccessModifier { get; }

        public readonly bool IsStatic { get; }

        public readonly Option<string> Type { get; }

        public readonly Option<string> Name { get; }

        public readonly Option<string> Summary { get; }

        public readonly Option<string> DefaultValue { get; }

        public readonly Option<string> Getter { get; }

        public readonly Option<string> Setter { get; }

        public readonly List<TypeParameter> TypeParameters { get; }

        public readonly Property With(
            Option<AccessModifier> accessModifier = default,
            Option<AccessModifier> setterAccessModifier = default,
            Option<bool> isStatic = default,
            Option<string> type = default,
            Option<string> name = default,
            Option<string> summary = default,
            Option<string> defaultValue = default,
            Option<Option<string>> getter = default,
            Option<Option<string>> setter = default) =>
            new Property(
                accessModifier: accessModifier.ValueOr(AccessModifier),
                setterAccessModifier: setterAccessModifier.ValueOr(SetterAccessModifier),
                isStatic: isStatic.ValueOr(IsStatic),
                type: type.Else(Type),
                name: name.Else(Name),
                summary: summary.Else(Summary),
                defaultValue: defaultValue.Else(DefaultValue),
                getter: getter.ValueOr(Getter),
                setter: setter.ValueOr(Setter));
    }

    internal readonly struct EnumerationMember
    {
        public EnumerationMember(Option<string> name, Option<int> value = default, Option<string> summary = default)
        {
            Name = name;
            Value = value;
            Summary = summary;
        }

        public readonly Option<string> Name { get; }

        public readonly Option<int> Value { get; }

        public readonly Option<string> Summary { get; }

        public readonly EnumerationMember With(
            Option<string> name = default,
            Option<int> value = default,
            Option<string> summary = default) =>
            new EnumerationMember(
                name: name.Else(Name),
                value: value.Else(value),
                summary: summary.Else(summary));
    }

    internal readonly struct Parameter
    {
        public Parameter(string type, string name, Option<string> receivingMember = default)
        {
            Type = type;
            Name = name;
            ReceivingMember = receivingMember;
        }

        public readonly string Type { get; }

        public readonly string Name { get; }

        public readonly Option<string> ReceivingMember { get; }
    }

    internal readonly struct Constructor
    {
        public Constructor(
            AccessModifier accessModifier,
            bool isStatic = false,
            Option<string> className = default,
            Option<string> summary = default,
            Option<List<Parameter>> parameters = default,
            Option<IEnumerable<string>> baseCallParameters = default)
        {
            AccessModifier = accessModifier;
            IsStatic = isStatic;
            ClassName = className;
            Summary = summary;
            Parameters = parameters.ValueOr(new List<Parameter>());
            BaseCallParameters = baseCallParameters;
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly bool IsStatic { get; }

        public readonly Option<string> ClassName { get; }

        public readonly Option<string> Summary { get; }

        public readonly List<Parameter> Parameters { get; }

        public readonly Option<IEnumerable<string>> BaseCallParameters { get; }

        public readonly Constructor With(
            Option<AccessModifier> accessModifier = default,
            Option<bool> isStatic = default,
            Option<string> className = default,
            Option<string> summary = default,
            Option<IEnumerable<string>> baseCallParameters = default) =>
            new Constructor(
                accessModifier: accessModifier.ValueOr(AccessModifier),
                isStatic: isStatic.ValueOr(IsStatic),
                className: className.Else(ClassName),
                summary: summary.Else(Summary),
                parameters: Option.Some(Parameters),
                baseCallParameters: baseCallParameters.Else(BaseCallParameters));
    }

    internal readonly struct Class
    {
        public Class(
            AccessModifier accessModifier,
            bool isStatic = false,
            Option<string> name = default,
            Option<string> summary = default,
            Option<string> inheritedClass = default,
            Option<List<string>> implementedInterfaces = default,
            Option<List<Field>> fields = default,
            Option<List<Property>> properties = default,
            Option<List<Constructor>> constructors = default,
            Option<List<TypeParameter>> typeParameters = default)
        {
            AccessModifier = accessModifier;
            IsStatic = isStatic;
            Name = name;
            Summary = summary;
            InheritedClass = inheritedClass;
            ImplementedInterfaces = implementedInterfaces.ValueOr(new List<string>());
            Fields = fields.ValueOr(new List<Field>());
            Properties = properties.ValueOr(new List<Property>());
            Constructors = constructors.ValueOr(new List<Constructor>());
            TypeParameters = typeParameters.ValueOr(new List<TypeParameter>());
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly bool IsStatic { get; }

        public readonly Option<string> Name { get; }

        public readonly Option<string> Summary { get; }

        public readonly Option<string> InheritedClass { get; }

        public readonly List<string> ImplementedInterfaces { get; }

        public readonly List<Field> Fields { get; }

        public readonly List<Property> Properties { get; }

        public readonly List<Constructor> Constructors { get; }

        public readonly List<TypeParameter> TypeParameters { get; }

        public readonly Class With(
            Option<AccessModifier> accessModifier = default,
            Option<bool> isStatic = default,
            Option<string> name = default,
            Option<string> summary = default,
            Option<string> inheritedClass = default) =>
            new Class(
                accessModifier: accessModifier.ValueOr(AccessModifier),
                isStatic: isStatic.ValueOr(IsStatic),
                name: name.Else(Name),
                summary: summary.Else(Summary),
                inheritedClass: inheritedClass.Else(inheritedClass),
                implementedInterfaces: Option.Some(ImplementedInterfaces),
                fields: Option.Some(Fields),
                properties: Option.Some(Properties),
                constructors: Option.Some(Constructors));
    }

    internal readonly struct Struct
    {
        public Struct(
            AccessModifier accessModifier,
            Option<string> name = default,
            Option<string> summary = default,
            Option<List<string>> implementedInterfaces = default,
            Option<List<Constructor>> constructors = default,
            Option<List<Field>> fields = default,
            Option<List<Property>> properties = default,
            Option<List<TypeParameter>> typeParameters = default)
        {
            AccessModifier = accessModifier;
            Name = name;
            Summary = summary;
            ImplementedInterfaces = implementedInterfaces.ValueOr(new List<string>());
            Constructors = constructors.ValueOr(new List<Constructor>());
            Fields = fields.ValueOr(new List<Field>());
            Properties = properties.ValueOr(new List<Property>());
            TypeParameters = typeParameters.ValueOr(new List<TypeParameter>());
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly Option<string> Name { get; }

        public readonly Option<string> Summary { get; }

        public readonly List<string> ImplementedInterfaces { get; }

        public readonly List<Constructor> Constructors { get; }

        public readonly List<Field> Fields { get; }

        public readonly List<Property> Properties { get; }

        public readonly List<TypeParameter> TypeParameters { get; }

        public readonly Struct With(
            Option<AccessModifier> accessModifier = default,
            Option<string> name = default,
            Option<string> summary = default) =>
            new Struct(
                accessModifier: accessModifier.ValueOr(AccessModifier),
                name: name.Else(Name),
                summary: summary.Else(Summary),
                implementedInterfaces: Option.Some(ImplementedInterfaces),
                constructors: Option.Some(Constructors),
                fields: Option.Some(Fields),
                properties: Option.Some(Properties));
    }

    internal readonly struct Interface
    {
        public Interface(
            AccessModifier accessModifier,
            Option<string> name = default,
            Option<string> summary = default,
            Option<List<string>> implementedInterfaces = default,
            Option<List<Property>> properties = default,
            Option<List<TypeParameter>> typeParameters = default)
        {
            AccessModifier = accessModifier;
            Name = name;
            Summary = summary;
            ImplementedInterfaces = implementedInterfaces.ValueOr(new List<string>());
            Properties = properties.ValueOr(new List<Property>());
            TypeParameters = typeParameters.ValueOr(new List<TypeParameter>());
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly Option<string> Name { get; }

        public readonly Option<string> Summary { get; }

        public readonly List<string> ImplementedInterfaces { get; }

        public readonly List<Property> Properties { get; }

        public readonly List<TypeParameter> TypeParameters { get; }

        public Interface With(
            Option<AccessModifier> accessModifier = default,
            Option<string> name = default,
            Option<string> summary = default) =>
            new Interface(
                accessModifier: accessModifier.ValueOr(AccessModifier),
                name: name.Else(Name),
                summary: summary.Else(Summary),
                implementedInterfaces: Option.Some(ImplementedInterfaces),
                properties: Option.Some(Properties));
    }

    internal readonly struct Enumeration
    {
        public Enumeration(
            AccessModifier accessModifier,
            Option<string> name = default,
            Option<string> summary = default,
            Option<bool> isFlag = default,
            Option<List<EnumerationMember>> members = default)
        {
            AccessModifier = accessModifier;
            Name = name;
            Summary = summary;
            IsFlag = isFlag.ValueOr(false);
            Members = members.ValueOr(new List<EnumerationMember>());
        }

        public readonly AccessModifier AccessModifier { get; }

        public readonly Option<string> Name { get; }

        public readonly Option<string> Summary { get; }

        public readonly bool IsFlag { get; }

        public readonly List<EnumerationMember> Members { get; }

        public readonly Enumeration With(
            Option<AccessModifier> accessModifier = default,
            Option<string> name = default,
            Option<string> summary = default,
            Option<bool> isFlag = default) =>
            new Enumeration(
                accessModifier: accessModifier.ValueOr(AccessModifier),
                name: name.Else(Name),
                summary: summary.Else(Summary),
                isFlag: isFlag.Else(Option.Some(IsFlag)),
                members: Option.Some(Members));
    }

    internal readonly struct Namespace
    {
        public Namespace(
            Option<string> name = default,
            Option<List<string>> usings = default,
            Option<List<Class>> classes = default,
            Option<List<Struct>> structs = default,
            Option<List<Interface>> interfaces = default,
            Option<List<Enumeration>> enums = default)
        {
            Name = name;
            Usings = usings.ValueOr(new List<string>());
            Classes = classes.ValueOr(new List<Class>());
            Structs = structs.ValueOr(new List<Struct>());
            Interfaces = interfaces.ValueOr(new List<Interface>());
            Enums = enums.ValueOr(new List<Enumeration>());
        }

        public readonly Option<string> Name { get; }

        public readonly List<string> Usings { get; }

        public readonly List<Class> Classes { get; }

        public readonly List<Struct> Structs { get; }

        public readonly List<Interface> Interfaces { get; }

        public readonly List<Enumeration> Enums { get; }

        public readonly Namespace With(Option<string> name = default) =>
            new Namespace(
                name: name.Else(Name),
                usings: Option.Some(Usings),
                classes: Option.Some(Classes),
                structs: Option.Some(Structs),
                interfaces: Option.Some(Interfaces),
                enums: Option.Some(Enums));
    }
}