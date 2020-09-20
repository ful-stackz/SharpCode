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

        public static string ToSourceCode(this AccessModifier accessModifier) =>
            accessModifier switch
            {
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
{access-modifier} {name}({parameters}){base-call}
{
    {assignments}
}";
            var raw = Template
                .Replace("{access-modifier}", constructor.AccessModifier.ToSourceCode())
                .Replace("{name}", constructor.ClassName)
                .Replace("{parameters}", constructor.Parameters.Select(param => param.ToSourceCode()).Join(", "))
                .Replace("{base-call}", constructor.BaseCallParameters.Match(
                    some: (parameters) => $": base({parameters.Join(", ")})",
                    none: () => string.Empty
                ))
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
{access-modifier} {type} {name}
{
    {getter}
    {setter}
}
            ";

            var raw = Template
                .Replace("{access-modifier}", property.AccessModifier.ToSourceCode())
                .Replace("{type}", property.Type)
                .Replace("{name}", property.Name)
                .Replace("{getter}", property.Getter.Match(
                    some: (getter) => getter.StartsWith("{") ? $"get{getter}" : $"get => {getter};",
                    none: () => property.Setter.HasValue ? string.Empty : "get;"
                ))
                .Replace("{setter}", property.Setter.Match(
                    some: (setter) => setter.StartsWith("{") ? $"set{setter}" : $"set => {setter};",
                    none: () => property.Getter.HasValue ? string.Empty : "set;"
                ));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Class classData, bool formatted)
        {
            const string ClassTemplate = @"
{access-modifier} class {name}{inheritance}
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
                .Replace("{name}", classData.Name)
                .Replace("{inheritance}", inheritance.Any() ? $": {inheritance.Join(", ")}" : string.Empty)
                .Replace("{fields}", classData.Fields.Select(field => field.ToSourceCode(false)).Join("\n"))
                .Replace("{constructors}", classData.Constructors.Select(ctor => ctor.ToSourceCode(false)).Join("\n"))
                .Replace("{properties}", classData.Properties.Select(property => property.ToSourceCode(false)).Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }
    }
}