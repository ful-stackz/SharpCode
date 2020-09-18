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
    }

    internal struct Constructor
    {
        public string ClassName { get; set; }
        public AccessModifier AccessModifier { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }
        public Option<IEnumerable<Parameter>> BaseCall { get; set; }
    }

    internal class Class
    {
        public string? Namespace { get; set; }
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        public string? Name { get; set; }
        public List<Field> Fields { get; } = new List<Field>();
        public List<Property> Properties { get; } = new List<Property>();
        public List<Constructor> Constructors { get; } = new List<Constructor>();
    }
}