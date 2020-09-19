using System.Collections.Generic;
using Optional;

namespace SharpCode
{
    public enum AccessModifier
    {
        Private,
        Internal,
        Protected,
        Public,
        PrivateInternal,
        ProtectedInternal,
    }

    internal struct Field
    {
        public AccessModifier AccessModifier { get; set; }
        public bool IsReadonly { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

    internal struct Property
    {
        public AccessModifier AccessModifier { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Option<string> Getter { get; set; }
        public Option<string> Setter { get; set; }
    }

    internal struct Parameter
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string? ReceivingMember { get; set; }
    }

    internal class Constructor
    {
        public string ClassName { get; set; } = string.Empty;
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public Option<IEnumerable<string>> BaseCallParameters { get; set; } = Option.None<IEnumerable<string>>();
    }

    internal class Class
    {
        public string? Namespace { get; set; }
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        public string? Name { get; set; }
        public Option<string> InheritedClass { get; set; } = Option.None<string>();
        public List<string> ImplementedInterfaces { get; } = new List<string>();
        public List<Field> Fields { get; } = new List<Field>();
        public List<Property> Properties { get; } = new List<Property>();
        public List<Constructor> Constructors { get; } = new List<Constructor>();
    }
}