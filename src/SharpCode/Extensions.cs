using System.Collections.Generic;
using System.Linq;

namespace SharpCode
{
    internal static class Extensions
    {
        public static string Join(this IEnumerable<string> source, string separator) =>
            string.Join(separator, source);

        public static string FormatSourceCode(this string source) =>
            Utils.FormatSourceCode(source);

        public static bool AtLeast<T>(this IEnumerable<T> source, int count) =>
            source.Take(count).Count() == count;

        public static string ToSourceCode(this AccessModifier accessModifier) =>
            accessModifier switch
            {
                AccessModifier.None => string.Empty,
                AccessModifier.Internal => "internal",
                AccessModifier.PrivateInternal => "private internal",
                AccessModifier.Protected => "protected",
                AccessModifier.ProtectedInternal => "protected internal",
                AccessModifier.Public => "public",
                _ => "private"
            };

        public static string ToSourceCode(this Field field, bool formatted)
        {
            const string Template = "{access-modifier} {readonly} {type} {name};";
            var raw = Template
                .Replace("{access-modifier}", field.AccessModifier.ToSourceCode())
                .Replace("{readonly}", field.IsReadonly ? "readonly" : string.Empty)
                .Replace("{type}", field.Type)
                .Replace("{name}", field.Name);

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Parameter parameter)
        {
            const string Template = "{type} {name}";
            return Template
                .Replace("{type}", parameter.Type)
                .Replace("{name}", parameter.Name);
        }

        public static string ToSourceCode(this Constructor constructor, bool formatted)
        {
            const string Template = @"
{access-modifier} {static-modifier} {name}({parameters}){base-call}
{
    {assignments}
}";
            var raw = Template
                .Replace("{access-modifier}", constructor.AccessModifier.ToSourceCode())
                .Replace("{static-modifier}", constructor.IsStatic ? "static" : string.Empty)
                .Replace("{name}", constructor.ClassName)
                .Replace("{parameters}", constructor.Parameters.Select(param => param.ToSourceCode()).Join(", "))
                .Replace("{base-call}", constructor.BaseCallParameters.Match(
                    some: (parameters) => $": base({parameters.Join(", ")})",
                    none: () => string.Empty))
                .Replace(
                    "{assignments}",
                    constructor.Parameters
                        .Where(param => !string.IsNullOrWhiteSpace(param.ReceivingMember))
                        .Select(param => $"{param.ReceivingMember} = {param.Name};")
                        .Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Property property, bool formatted)
        {
            const string Template = @"
{access-modifier} {static-modifier} {type} {name}
{getter-setter-open-bracket}
    {getter}
    {setter}
{getter-setter-close-bracket} {default-value}
            ";

            var raw = Template
                .Replace("{access-modifier}", property.AccessModifier.ToSourceCode())
                .Replace("{static-modifier}", property.IsStatic ? "static" : string.Empty)
                .Replace("{type}", property.Type)
                .Replace("{name}", property.Name)
                .Replace("{getter-setter-open-bracket}", property.Getter.Else(property.Setter).HasValue
                    ? "{"
                    : ";") // Use ; instead of { if there is no getter or setter
                .Replace("{getter-setter-close-bracket}", property.Getter.Else(property.Setter).HasValue
                    ? "}"
                    : string.Empty)
                .Replace("{getter}", property.Getter.Match(
                    some: (getter) => string.IsNullOrWhiteSpace(getter)
                        ? "get;"
                        : getter!.StartsWith("{") ? $"get{getter}" : $"get => {getter};",
                    none: () => string.Empty))
                .Replace("{setter}", property.Setter.Match(
                    some: (setter) => string.IsNullOrWhiteSpace(setter)
                        ? "set;"
                        : setter!.StartsWith("{") ? $"set{setter}" : $"set => {setter};",
                    none: () => string.Empty))
                .Replace("{default-value}", property.DefaultValue.Match(
                    some: (def) =>
                    {
                        var defValue = def.StartsWith("=") ? string.Empty : "= ";
                        defValue += def;
                        defValue += def.EndsWith(";") ? string.Empty : ";";
                        return defValue;
                    },
                    none: () => string.Empty));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Class classData, bool formatted)
        {
            const string ClassTemplate = @"
{access-modifier} {static-modifier} class {name}{inheritance}
{
    {fields}
    {constructors}
    {properties}
}
            ";

            var inheritance = classData.InheritedClass.Match(
                some: (className) => classData.ImplementedInterfaces.Prepend(className),
                none: () => classData.ImplementedInterfaces);

            // Do not format members separately, rather format the entire class, if requested
            var raw = ClassTemplate
                .Replace("{access-modifier}", classData.AccessModifier.ToSourceCode())
                .Replace("{static-modifier}", classData.IsStatic ? "static" : string.Empty)
                .Replace("{name}", classData.Name)
                .Replace("{inheritance}", inheritance.Any() ? $": {inheritance.Join(", ")}" : string.Empty)
                .Replace("{fields}", classData.Fields.Select(field => field.ToSourceCode(false)).Join("\n"))
                .Replace("{constructors}", classData.Constructors.Select(ctor => ctor.ToSourceCode(false)).Join("\n"))
                .Replace("{properties}", classData.Properties.Select(property => property.ToSourceCode(false)).Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Interface data, bool formatted)
        {
            const string Template = @"
{access-modifier} interface {name}{inheritance}
{
    {properties}
}
            ";

            var raw = Template
                .Replace("{access-modifier}", data.AccessModifier.ToSourceCode())
                .Replace("{name}", data.Name)
                .Replace("{inheritance}", data.ImplementedInterfaces.Any()
                    ? $": {data.ImplementedInterfaces.Join(", ")}"
                    : string.Empty)
                .Replace("{properties}", data.Properties.Select(item => item.ToSourceCode(false)).Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this EnumerationMember data)
        {
            return "{name}{value},"
                .Replace("{name}", data.Name)
                .Replace("{value}", data.Value.Match(
                    some: (value) => $" = {value}",
                    none: () => string.Empty));
        }

        public static string ToSourceCode(this Enumeration data, bool formatted)
        {
            const string Template = @"
{flags-attribute}
{access-modifier} enum {name}
{
    {members}
}
            ";

            var raw = Template
                .Replace("{flags-attribute}", data.IsFlag ? "[System.Flags]" : string.Empty)
                .Replace("{access-modifier}", data.AccessModifier.ToSourceCode())
                .Replace("{name}", data.Name!)
                .Replace("{members}", data.Members.Select(ToSourceCode).Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Namespace data, bool formatted)
        {
            const string Template = @"
{usings}

namespace {name}
{
    {enums}
    {interfaces}
    {classes}
}
            ";

            var raw = Template
                .Replace("{name}", data.Name)
                .Replace("{usings}", data.Usings.Select(item => $"using {item};").Join("\n"))
                .Replace("{enums}", data.Enums.Select(item => item.ToSourceCode(false)).Join("\n"))
                .Replace("{interfaces}", data.Interfaces.Select(item => item.ToSourceCode(false)).Join("\n"))
                .Replace("{classes}", data.Classes.Select(item => item.ToSourceCode(false)).Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }
    }
}
