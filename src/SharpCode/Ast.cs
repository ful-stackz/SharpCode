using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Optional.Unsafe;

namespace SharpCode
{
    internal static class Ast
    {
        public static SyntaxToken[] FromDefinition(AccessModifier accessModifier) =>
            accessModifier switch
            {
                AccessModifier.Internal => Utils.AsArray(SyntaxFactory.Token(SyntaxKind.InternalKeyword)),

                AccessModifier.Private => Utils.AsArray(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)),

                AccessModifier.PrivateProtected => Utils.AsArray(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)),

                AccessModifier.Protected => Utils.AsArray(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)),

                AccessModifier.ProtectedInternal => Utils.AsArray(
                    SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                    SyntaxFactory.Token(SyntaxKind.InternalKeyword)),

                AccessModifier.Public => Utils.AsArray(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),

                _ => Utils.AsArray<SyntaxToken>(),
            };

        public static FieldDeclarationSyntax FromDefinition(Field definition)
        {
            var variableDeclaration = definition.TypeParameters.Any()
                /* create variable declaration with type parameters, ie. "Dictionary<TKey, TValue> _store" */
                ? SyntaxFactory
                    .VariableDeclaration(
                        SyntaxFactory.GenericName(definition.Type.ValueOrFailure())
                            .WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.SeparatedList(
                                        definition.TypeParameters.Select<TypeParameter, TypeSyntax>(
                                            typeParam => SyntaxFactory.IdentifierName(typeParam.Name.ValueOrFailure()))))))
                    .AddVariables(SyntaxFactory.VariableDeclarator(definition.Name.ValueOrFailure()))
                /* create regular variable declaration, ie. "string _name" */
                : SyntaxFactory
                    .VariableDeclaration(SyntaxFactory.ParseTypeName(definition.Type.ValueOrFailure()))
                    .AddVariables(SyntaxFactory.VariableDeclarator(definition.Name.ValueOrFailure()));

            var fieldDeclaration = SyntaxFactory
                .FieldDeclaration(variableDeclaration)
                .AddModifiers(FromDefinition(definition.AccessModifier));

            if (definition.IsReadonly)
            {
                fieldDeclaration = fieldDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
            }

            definition.Summary.MatchSome(summary =>
            {
                fieldDeclaration = fieldDeclaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            return fieldDeclaration;
        }

        public static PropertyDeclarationSyntax FromDefinition(Property definition)
        {
            var declaration = SyntaxFactory
                .PropertyDeclaration(
                    type: SyntaxFactory.ParseTypeName(definition.Type.ValueOrFailure()),
                    identifier: definition.Name.ValueOrFailure())
                .AddModifiers(FromDefinition(definition.AccessModifier));

            if (definition.IsStatic)
            {
                declaration = declaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            definition.Getter.Map(x => x.Trim()).MatchSome(body =>
            {
                declaration = declaration.AddAccessorListAccessors(
                    AddBodyToAccessor(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration), body));
            });

            definition.Setter.Map(x => x.Trim()).MatchSome(body =>
            {
                AccessorDeclarationSyntax accessor =
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);

                if (IsHigher(definition.AccessModifier, definition.SetterAccessModifier))
                {
                    accessor = accessor.WithModifiers(
                        new SyntaxTokenList(FromDefinition(definition.SetterAccessModifier)));
                }
                else if (definition.AccessModifier != definition.SetterAccessModifier)
                {
                    throw new SyntaxException(
                        $"The accessibility modifier of the accessor must be more restrictive than the property '{definition.Name.ValueOrFailure()}'");
                }

                declaration = declaration.AddAccessorListAccessors(
                    AddBodyToAccessor(accessor, body));
            });

            definition.DefaultValue.MatchSome(value =>
            {
                declaration = declaration
                    .WithInitializer(
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.ParseExpression(value)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            });

            if (!definition.Getter.HasValue &&
                !definition.Setter.HasValue &&
                !definition.DefaultValue.HasValue)
            {
                // if the property has no getter, setter and default value
                // then we need to end the line with a semicolon
                // in which case the property would looks like
                // "public int Identifier;"
                declaration = declaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }

            return declaration;

            static AccessorDeclarationSyntax AddBodyToAccessor(AccessorDeclarationSyntax accessor, string body)
            {
                if (body.Equals(Property.AutoGetterSetter))
                {
                    return accessor.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                }

                if (body.StartsWith("{"))
                {
                    var statementsContainer = CSharpSyntaxTree
                        .ParseText(body)
                        .GetRoot()
                        .ChildNodes()
                        .First();

                    if (statementsContainer is GlobalStatementSyntax c && c.Statement is BlockSyntax block)
                    {
                        return accessor.WithBody(block);
                    }
                }

                // in the final case the body is an arrow expression syntax
                return accessor
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression(body)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }

            static bool IsHigher(AccessModifier left, AccessModifier right)
            {
                switch (left)
                {
                    case AccessModifier.None:
                    case AccessModifier.Private:
                        return false;
                    case AccessModifier.Internal:
                        switch (right)
                        {
                            case AccessModifier.Private:
                            case AccessModifier.PrivateProtected:
                                return true;
                            default:
                                return false;
                        }

                    case AccessModifier.Protected:
                        switch (right)
                        {
                            case AccessModifier.Private:
                            case AccessModifier.PrivateProtected:
                                return true;
                            default:
                                return false;
                        }

                    case AccessModifier.Public:
                        switch (right)
                        {
                            case AccessModifier.Private:
                            case AccessModifier.PrivateProtected:
                            case AccessModifier.Internal:
                            case AccessModifier.Protected:
                            case AccessModifier.ProtectedInternal:
                                return true;
                            default:
                                return false;
                        }

                    case AccessModifier.PrivateProtected:
                        switch (right)
                        {
                            case AccessModifier.Private:
                                return true;
                            default:
                                return false;
                        }

                    case AccessModifier.ProtectedInternal:
                        switch (right)
                        {
                            case AccessModifier.Private:
                            case AccessModifier.Internal:
                            case AccessModifier.Protected:
                            case AccessModifier.PrivateProtected:
                            case AccessModifier.ProtectedInternal:
                                return true;
                            default:
                                return false;
                        }

                    default:
                        return false;
                }
            }
        }

        public static ParameterSyntax FromDefinition(Parameter definition)
        {
            return SyntaxFactory
                .Parameter(SyntaxFactory.Identifier(definition.Name))
                .WithType(SyntaxFactory.ParseTypeName(definition.Type));
        }

        public static ConstructorDeclarationSyntax FromDefinition(Constructor definition)
        {
            var declaration = SyntaxFactory
                .ConstructorDeclaration(SyntaxFactory.Identifier(definition.ClassName.ValueOrFailure()))
                .AddModifiers(FromDefinition(definition.AccessModifier));

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            if (definition.IsStatic)
            {
                declaration = declaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            if (definition.Parameters.Any())
            {
                declaration = declaration.WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(
                            definition.Parameters.Select(FromDefinition))));
            }

            definition.BaseCallParameters.MatchSome(parameters =>
            {
                declaration = declaration.WithInitializer(
                    SyntaxFactory
                        .ConstructorInitializer(
                            SyntaxKind.BaseConstructorInitializer,
                            SyntaxFactory.ArgumentList())
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    parameters.Select(param =>
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(param)))))));
            });

            var parametersWithAssignment = definition.Parameters.Where(x => x.ReceivingMember.HasValue);
            declaration = parametersWithAssignment.Any()
                ? declaration.WithBody(
                    SyntaxFactory.Block(
                        parametersWithAssignment.Select(param =>
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.IdentifierName(param.ReceivingMember.ValueOrFailure()),
                                    SyntaxFactory.IdentifierName(param.Name))))))
                : declaration.WithBody(SyntaxFactory.Block());

            return declaration;
        }

        public static ClassDeclarationSyntax FromDefinition(Class definition)
        {
            var declaration = SyntaxFactory
                .ClassDeclaration(definition.Name.ValueOrFailure())
                .AddModifiers(FromDefinition(definition.AccessModifier));

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            if (definition.IsStatic)
            {
                declaration = declaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            definition.InheritedClass.MatchSome(name =>
            {
                declaration = declaration.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(name)));
            });

            if (definition.TypeParameters.Any())
            {
                declaration = declaration
                    .WithTypeParameterList(
                        SyntaxFactory.TypeParameterList(
                            SyntaxFactory.SeparatedList(
                                definition.TypeParameters.Select(FromDefinition))))
                    .WithConstraintClauses(
                        SyntaxFactory.List(
                            definition.TypeParameters
                                .Where(x => x.Constraints.Any())
                                .Select(GetTypeParameterConstraints)));
            }

            if (definition.ImplementedInterfaces.Any())
            {
                declaration = declaration.AddBaseListTypes(
                    definition.ImplementedInterfaces.Select(
                        name => SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(name))).ToArray());
            }

            if (definition.Fields.Any())
            {
                declaration = declaration.AddMembers(
                    definition.Fields.Select(FromDefinition).ToArray());
            }

            if (definition.Constructors.Any())
            {
                declaration = declaration.AddMembers(
                    definition.Constructors.Select(FromDefinition).ToArray());
            }

            if (definition.Properties.Any())
            {
                declaration = declaration.AddMembers(
                    definition.Properties.Select(FromDefinition).ToArray());
            }

            return declaration;
        }

        public static StructDeclarationSyntax FromDefinition(Struct definition)
        {
            var declaration = SyntaxFactory
                .StructDeclaration(definition.Name.ValueOrFailure())
                .AddModifiers(FromDefinition(definition.AccessModifier));

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            if (definition.ImplementedInterfaces.Any())
            {
                declaration = declaration.AddBaseListTypes(
                    definition.ImplementedInterfaces
                        .Select(name => SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(name)))
                        .ToArray());
            }

            if (definition.Fields.Any())
            {
                declaration = declaration.AddMembers(definition.Fields.Select(FromDefinition).ToArray());
            }

            if (definition.Constructors.Any())
            {
                declaration = declaration.AddMembers(definition.Constructors.Select(FromDefinition).ToArray());
            }

            if (definition.Properties.Any())
            {
                declaration = declaration.AddMembers(definition.Properties.Select(FromDefinition).ToArray());
            }

            return declaration;
        }

        public static EnumMemberDeclarationSyntax FromDefinition(EnumerationMember definition)
        {
            var declaration = SyntaxFactory.EnumMemberDeclaration(definition.Name.ValueOrDefault());

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            definition.Value.MatchSome(value =>
            {
                declaration = declaration.WithEqualsValue(
                    SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(value))));
            });

            return declaration;
        }

        public static EnumDeclarationSyntax FromDefinition(Enumeration definition)
        {
            var declaration = SyntaxFactory
                .EnumDeclaration(definition.Name.ValueOrFailure())
                .AddModifiers(FromDefinition(definition.AccessModifier));

            if (definition.IsFlag)
            {
                declaration = declaration.WithAttributeLists(
                    SyntaxFactory.SingletonList(
                        SyntaxFactory.AttributeList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Attribute(
                                    SyntaxFactory.ParseName("System.Flags"))))));
            }

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            if (definition.Members.Any())
            {
                declaration = declaration.AddMembers(definition.Members.Select(FromDefinition).ToArray());
            }

            return declaration;
        }

        public static InterfaceDeclarationSyntax FromDefinition(Interface definition)
        {
            var declaration = SyntaxFactory
                .InterfaceDeclaration(definition.Name.ValueOrFailure())
                .AddModifiers(FromDefinition(definition.AccessModifier));

            definition.Summary.MatchSome(summary =>
            {
                declaration = declaration.WithLeadingTrivia(CreateXmlDoc(summary));
            });

            if (definition.ImplementedInterfaces.Any())
            {
                declaration = declaration.AddBaseListTypes(
                    definition.ImplementedInterfaces
                        .Select(name => SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(name)))
                        .ToArray());
            }

            if (definition.Properties.Any())
            {
                declaration = declaration.AddMembers(definition.Properties.Select(FromDefinition).ToArray());
            }

            return declaration;
        }

        public static NamespaceDeclarationSyntax FromDefinition(Namespace definition)
        {
            var declaration =
                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(definition.Name.ValueOrFailure()));

            if (definition.Interfaces.Any())
            {
                declaration = declaration.AddMembers(definition.Interfaces.Select(FromDefinition).ToArray());
            }

            if (definition.Enums.Any())
            {
                declaration = declaration.AddMembers(definition.Enums.Select(FromDefinition).ToArray());
            }

            if (definition.Structs.Any())
            {
                declaration = declaration.AddMembers(definition.Structs.Select(FromDefinition).ToArray());
            }

            if (definition.Classes.Any())
            {
                declaration = declaration.AddMembers(definition.Classes.Select(FromDefinition).ToArray());
            }

            return declaration;
        }

        public static TypeParameterSyntax FromDefinition(TypeParameter definition) =>
            SyntaxFactory.TypeParameter(SyntaxFactory.Identifier(definition.Name.ValueOrFailure()));

        public static TypeParameterConstraintClauseSyntax GetTypeParameterConstraints(TypeParameter definition) =>
            SyntaxFactory
                .TypeParameterConstraintClause(SyntaxFactory.IdentifierName(definition.Name.ValueOrFailure()))
                .WithConstraints(SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                    definition.Constraints.Select(constraint => SyntaxFactory.TypeConstraint(SyntaxFactory.IdentifierName(constraint)))));

        public static CompilationUnitSyntax NamespaceContainer(List<string> usings) =>
            SyntaxFactory.CompilationUnit().WithUsings(
                SyntaxFactory.List(
                    usings.Select(
                        @using => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@using)))));

        public static string Stringify(MemberDeclarationSyntax ast) =>
            SyntaxFactory
                .CompilationUnit()
                .AddMembers(ast)
                .NormalizeWhitespace()
                .ToFullString();

        public static string Stringify(CompilationUnitSyntax ast) =>
            ast.NormalizeWhitespace().ToFullString();

        internal static SyntaxTriviaList CreateXmlDoc(string summary)
        {
            return SyntaxFactory.TriviaList(
                SyntaxFactory.Trivia(
                    SyntaxFactory.DocumentationCommentTrivia(
                        kind: SyntaxKind.SingleLineDocumentationCommentTrivia,
                        content: SyntaxFactory.List(
                            new XmlNodeSyntax[]
                            {
                                SyntaxFactory
                                    .XmlText()
                                    .WithTextTokens(
                                        SyntaxFactory.TokenList(
                                            SyntaxFactory.XmlTextLiteral(
                                                leading: SyntaxFactory.TriviaList(
                                                    SyntaxFactory.DocumentationCommentExterior("///")),
                                                text: " ",
                                                value: " ",
                                                trailing: SyntaxFactory.TriviaList()))),
                                CreateXmlDocBlock("summary", summary), SyntaxFactory
                                    .XmlText()
                                    .WithTextTokens(
                                        SyntaxFactory.TokenList(
                                            SyntaxFactory.XmlTextNewLine(
                                                leading: SyntaxFactory.TriviaList(),
                                                text: "\n",
                                                value: "\n",
                                                trailing: SyntaxFactory.TriviaList()))),
                            }))));

            static XmlElementSyntax CreateXmlDocBlock(string name, string content)
            {
                var tokens = content
                    .Split('\n')
                    .Select(line => SyntaxFactory.XmlTextLiteral(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.DocumentationCommentExterior("///")),
                        text: $" {line}",
                        value: $" {line}",
                        trailing: SyntaxFactory.TriviaList()))
                    .ToList();

                // not really sure what this does exactly, but RoslynQuoter says it's needed,
                // and if not present the closing tag doesn't get the leading /// 😐
                tokens.Add(SyntaxFactory.XmlTextLiteral(
                    leading: SyntaxFactory.TriviaList(SyntaxFactory.DocumentationCommentExterior("///")),
                    text: " ",
                    value: " ",
                    trailing: SyntaxFactory.TriviaList()));

                // add a new line xml token between each text line,
                // as well as a leading new line as seen on RoslynQuoter
                for (var i = 0; i < tokens.Count; i += 2)
                {
                    tokens.Insert(i, SyntaxFactory.XmlTextNewLine(
                        leading: SyntaxFactory.TriviaList(),
                        text: "\n",
                        value: "\n",
                        trailing: SyntaxFactory.TriviaList()));
                }

                return SyntaxFactory
                    .XmlExampleElement(
                        SyntaxFactory.SingletonList<XmlNodeSyntax>(
                            SyntaxFactory.XmlText().WithTextTokens(
                                SyntaxFactory.TokenList(tokens))))
                    .WithStartTag(
                        SyntaxFactory.XmlElementStartTag(
                            SyntaxFactory.XmlName(
                                SyntaxFactory.Identifier(name))))
                    .WithEndTag(
                        SyntaxFactory.XmlElementEndTag(
                            SyntaxFactory.XmlName(
                                SyntaxFactory.Identifier(name))));
            }
        }
    }
}