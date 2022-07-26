# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- New access modifier - `AccessModifier.PrivateProtected` - `private protected`
- Support for setting the access modifier of a property' getter and setter individually via the `PropertyBuilder.WithAccessModifier(AccessModifier getterAccessModifier, AccessModifier setterAccessModifier)` overloaded method
- Support for defining type parameters when building classes, structs, interfaces, fields and parameters
- More validation added to fluent `With...()` and `Code.Create...()` methods that accept `string` parameters; empty or whitespace strings will now result in an `ArgumentException`

### Changed

- `Code.cs`, the entry point for creating a structure, has been modified to use the fluent APIs of each structure instead of using overridden constructors; this reduces duplicated validation code
- The Roslyn packages were updated which results in a slightly different formatting in some places
  - `constructor() : base()` instead of `constructor(): base()`
  - `Property { get; set; }` and `Property { get => _value; set => _value = value; }` instead of the old multi-line version

### Fixed

- Removed the invalid access modifier `AccessModifier.PrivateInternal` - `private internal`

## [0.3.0] - 2021-08-05

### Added

- Introspection API via the new .HasMember() method, available on applicable structure builders
- More null arguments sanitizing in public methods and consecutively more `ArgumentNullException` and `ArgumentException`, which are documented in the respective methods
- Using Roslyn APIs to generate the source code via an actual AST, rather than string templates hydration

### Changed

- **[breaking]** `.ToSourceCode(bool)` has been changed to `.ToSourceCode()`; all generated code is now formatted by default
- Refactored the internal structures from `class` to `readonly struct`
- Made better use of optionals and replaced some nullables
- Enums no longer have a trailing comma

### Fixed

- Fixes the issue described in [PR#13](https://github.com/ful-stackz/SharpCode/pull/13) - source code template strings were breaking when hydration keywords were used by consumer code

## [0.2.0] - 2020-10-05

### Added

- Automated GitHub releases from the release action
- Support for enums generation
- Support for various enum members - with implicit and/or explicit values, flags
- Support for structs generation
- Support for generating XML summary documentation ([Zaid Ajaj](https://github.com/Zaid-Ajaj))
   - for interfaces, classes, structs, enums, enum members, fields, properties and constructors
- Code coverage, via CodeCov
- [StyleCop.Analyzer](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) and [SonarAnalyzer.CSharp](https://github.com/SonarSource/sonar-dotnet)

## [0.1.0] - 2020-09-26

### Added

- Start using a changelog
- Static classes
- Static properties
- Properties with default values
- Generating interface source code
- Bulk-friendly APIs
- Errors documentation
- XML documentation for exposed API

### Changed

- Improved release action
   - Use GitHub secrets and env vars instead of providing the token manually
   - Changed the publish script to look for an env var if nuget api key not provided as an argument
- Structure build logic - keep builders locally, build when explicitly requested
- Improved validation during the build of a structure
- Validate the access members of namespace members (no private/protected [internal] classes/interfaces!)

## [0.0.1] - 2020-09-20

### Added

- Basic code generation for namespaces, classes, constructors, fields and properties
- Tests project
- Automated GitHub Actions for CI and releases
- Documentation for releasing
