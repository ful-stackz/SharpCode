using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using SharpCode;

namespace JsonToNet
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const ConsoleColor LogoColor = ConsoleColor.Green;
            Write(@"       _            _____    _  _     _   ", LogoColor);
            Write(@"    _ | |___ ___ _ |_   _|__| \| |___| |_ ", LogoColor);
            Write(@"   | || (_-</ _ \ ' \| |/ _ \ .` / -_)  _|", LogoColor);
            Write(@"    \__//__/\___/_||_|_|\___/_|\_\___|\__|", LogoColor);
            Write(@"                                          ", LogoColor);

            Write($"Working directory: {Directory.GetCurrentDirectory()}", endWithEmptyLine: true);

            var inputFiles = Args.GetInputFiles(args)
                .SelectMany(path =>
                {
                    if (File.Exists(path))
                    {
                        return new string[] { path };
                    }
                    else if (Directory.Exists(path))
                    {
                        return Directory.GetFiles(path);
                    }
                    else
                    {
                        Write($"Input argument '{path}' is not a file nor a valid directory and will not be used.");
                        return new string[] { string.Empty };
                    }
                })
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Where(path => path.EndsWith(".json", ignoreCase: true, culture: null));

            if (!inputFiles.Any())
            {
                Write("Please provide input file(s).", ConsoleColor.DarkRed);
                Write("\tdotnet run -- input:<file-path | dir-path>[,<file-path | dir-path>]", ConsoleColor.DarkRed);
                return;
            }

            var outputDirectory = Args.GetOutputDirectory(args) ?? "./output";

            Write($"Output directory: {Path.GetFullPath(outputDirectory)}", endWithEmptyLine: true);
            Write($"Input files: {inputFiles.Select(Path.GetFullPath).Join("\n\t")}", endWithEmptyLine: true);

            foreach (var filePath in inputFiles)
            {
                var fileFullName = Path.GetFileName(filePath);
                var fileName = fileFullName.Replace(".json", string.Empty);

                Write($"Processing '{fileFullName}'...");
                var generatedCode = SourceCode.FromJson(fileName, File.ReadAllText(filePath));
                var outputFilePath = Path.Join(outputDirectory, $"{fileName.ToPascalCase()}.cs");
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                File.WriteAllText(outputFilePath, generatedCode);

                Write($"Generated '{Path.GetFullPath(outputFilePath)}'", ConsoleColor.Green, endWithEmptyLine: true);
            }
        }

        private static void Write(
            string message,
            ConsoleColor color = ConsoleColor.White,
            bool endWithEmptyLine = false)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();

            if (endWithEmptyLine)
            {
                Console.WriteLine();
            }
        }
    }

    internal static class SourceCode
    {
        /// <summary>
        /// Generates source code matching the shape of the data from the provided JSON contents.
        /// Supports the following types of JSON files:
        /// <code>
        /// // fileName: user.json
        /// {
        ///     "accountHolder": {
        ///         "name": "string",
        ///         "login": {
        ///             "email": "string",
        ///             "password": "string"
        ///         }
        ///     }
        /// }
        /// // in this case the single top-level property will be used to generate the main class
        /// // therefore the main class will have the name AccountHolder, rather than User
        /// // and all properties of the "accountHolder" object will be extracted as properties of that class
        /// </code>
        /// <code>
        /// // fileName: user.json
        /// {
        ///     "name": "string",
        ///     "login": {
        ///         "email": "string",
        ///         "password": "string"
        ///     }
        /// }
        /// // in this case the name of the file will be the name of the main generated class (User)
        /// // and all top level properties will be extracted as properties of that class
        /// </code>
        /// </summary>
        public static string FromJson(string name, string contents)
        {
            // Parse the contents of the file into a JSON object
            var json = JObject.Parse(contents);

            // Create the namespace builder, which contains all classes extracted from the JSON file
            var namespaceBuilder = Code.CreateNamespace(name: "JsonToNet.Data");

            var topLevelProperties = json.Children().OfType<JProperty>();
            if (topLevelProperties.Take(2).Count() == 1 &&
                topLevelProperties.First().Value.Type == JTokenType.Object)
            {
                // This branch handles the first supported JSON structure (see the summary of the method).
                // In that case we don't need to do anything special here, as the main class is simply a "sub-class" of
                // the top-level JSON object and will be extracted below by @ExtractClasses()
            }
            else
            {
               // This branch handles the second supported JSON structure (see the summary of the method).
                _ = namespaceBuilder.WithClass(Code.CreateClass()
                    .WithName(name.ToPascalCase())
                    .WithProperties(ExtractProperties(json)));
            }

            // Extract any sub-object values as classes
            var subClasses = ExtractClasses(json);

            return namespaceBuilder
                .WithUsing("System")
                .WithClasses(subClasses)
                .ToSourceCode();
        }

        private static IEnumerable<ClassBuilder> ExtractClasses(JObject json) =>
            json.Children()
            .OfType<JProperty>()
            .Where(child => child.Value.Type == JTokenType.Object)
            .SelectMany(child =>
            {
                var subClasses = ExtractClasses(child.Value as JObject);

                var mainClass = Code.CreateClass()
                    .WithName(child.Name.ToPascalCase())
                    .WithProperties(ExtractProperties(child.Value as JObject));

                return subClasses.Append(mainClass);
            });

        private static IEnumerable<PropertyBuilder> ExtractProperties(JObject json)
        {
            return ExtractSimpleProperties(json)
                .Concat(ExtractObjectProperties(json))
                .Concat(ExtractArrayProperties(json));

            static IEnumerable<PropertyBuilder> ExtractSimpleProperties(JObject jobject) =>
                jobject.Children()
                .OfType<JProperty>()
                .Where(child => IsSimpleType(child.Value.Type))
                .Select(child =>
                {
                    return Code.CreateProperty()
                        .WithName(child.Name.ToPascalCase())
                        .WithType(JsonTypeToNet(child.Value.Type));
                });

            static IEnumerable<PropertyBuilder> ExtractObjectProperties(JObject jObject) =>
                jObject.Children()
                .OfType<JProperty>()
                .Where(child => child.Value.Type == JTokenType.Object)
                .Select(child =>
                {
                    return Code.CreateProperty()
                        .WithName(child.Name.ToPascalCase())
                        .WithType(child.Name.ToPascalCase());
                });

            static IEnumerable<PropertyBuilder> ExtractArrayProperties(JObject jObject) =>
                jObject.Children()
                .OfType<JProperty>()
                .Where(child => child.Value.Type == JTokenType.Array)
                .Where(child =>
                {
                    return (child.Value as JArray)
                        .Children()
                        .Select(x => x.Type)
                        .Distinct()
                        .Take(2)
                        .Where(x => x != JTokenType.Object)
                        .Count() == 1;
                })
                .Select(child =>
                {
                    var arrayType = (child.Value as JArray).First.Type;

                    return Code.CreateProperty()
                        .WithName(child.Name.ToPascalCase())
                        .WithType(JsonTypeToNet(arrayType) + "[]");
                });
        }

        private static string JsonTypeToNet(JTokenType type) =>
            type switch
            {
                JTokenType.String => "string",
                JTokenType.Integer => "int",
                JTokenType.Float => "double",
                JTokenType.Boolean => "bool",
                JTokenType.Date => "DateTime",
                JTokenType.TimeSpan => "TimeSpan",
                _ => string.Empty
            };

        private static bool IsSimpleType(JTokenType type) =>
            type switch
            {
                JTokenType.String => true,
                JTokenType.Integer => true,
                JTokenType.Float => true,
                JTokenType.Boolean => true,
                JTokenType.Date => true,
                JTokenType.TimeSpan => true,
                _ => false
            };
    }

    internal static class Args
    {
        public static IEnumerable<string> GetInputFiles(string[] args) =>
            args.Where(arg => arg.StartsWith("input:", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault()?
            .Split(":", 2)
            .Last()
            .Split(",") ?? Enumerable.Empty<string>();

        public static string GetOutputDirectory(string[] args) =>
            args.Where(arg => arg.StartsWith("out:", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault()?
            .Split(":")
            .Last() ?? null;
    }

    internal static class Extensions
    {
        public static string Join(this IEnumerable<string> source, string separator) =>
            string.Join(separator, source);

        public static string ToPascalCase(this string source) =>
            string.IsNullOrWhiteSpace(source)
                ? string.Empty
                : source.Substring(0, 1).ToUpperInvariant() +
                Regex.Replace(source[1..], @"(_|-)(\w)", (match) => match.Groups[2].Value.ToUpperInvariant());
    }
}
