# How to: Releases

## Official releases

Official releases are created through the designated GitHub action.

1. Modify the [.version](../.version) file with the target version of the release
2. Run the _"Pack and publish"_ GitHub action
   - Set the _"Publish as beta"_ flag accordingly. Possible values are `true` and `false`
3. Tag the released commit with the version from the .version file
   - Use the `v{major.minor.patch[-beta]}` format

## Local releases

To make a local release/package you only need to run the `scripts/PackAndPublish.ps1` powershell script. It will then
create a new folder `publish/` in the main directory with the nuget package inside.

### Script arguments

| Name | Type |Default | Description |
| ---- | ---- | -------- | ----------- |
| `Version` | `string` | `""` | The version that the package will have after being packaged. If not provided the conotents of the `.version` file will be used. |
| `Beta` | `switch` | `false` | Appends `-beta` to the package version and changes the ID to `Beta.SharpCode` |

### Examples
- `PackAndPublish.ps1 -Version:1.0.0` will produce a package with version `1.0.0`
- `PackAndPublish.ps1 -Version:1.0.0 -Beta` will produce a package with ID `Beta.SharpCode` and version `1.0.0-beta`