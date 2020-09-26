# Sample projects

This folder contains projects which make use of `SharpCode` in order to present some use cases as working applications.

## JsonToNet

`JsonToNet` is a simple CLI tool accepts the path to a number of JSON files and an (optional) output directory and
extracts C# classes from the shape of the JSON data.

`JsonToNet` defines the logic for extracting data from a JSON file, for example what the name and type of a certain
property should be in a C# class; then it turns to `SharpCode` and provides all this extracted metadata. From there on
`SharpCode` builds the requirements and returns the source code. `JsonToNet` takes that source code and writes it into
a file in the specified output directory.

`JsonToNet` is limited in features, as it is not meant to be a full-scale JSON data shape extraction tool.

### Sample commands

For quickly trying out how `JsonToNet` works you can use the following commands, which will fire up `JsonToNet` with
some built-in example JSON source files.

You can also modify the files in the `JsonToNet/input` folder or add your own and try it out.

```
cd JsonToNet
dotnet run -- input:./input
// will generate C# classes from all files in the ./input folder
// and output them in ./output
```

```
cd JsonToNet
dotnet run -- input:./input/user.json output:./custom-output
// will generate a C# class from ./input/user.json
// and output it to ./custom-output
```
