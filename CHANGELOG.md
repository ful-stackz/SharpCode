# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Start using a changelog
- Static classes
- Static properties
- Properties with default values
- Generating interface source code
- Bulk-friendly APIs
- Errors documentation
- Generate XML documentation

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
