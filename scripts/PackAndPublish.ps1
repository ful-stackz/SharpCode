param (
  [string] $Version,
  [string] $Configuration="Release",
  [string] $SourceDirectory="../src/SharpCode",
  [string] $OutDirectory="../publish",
  [string] $NuGetApiKey,
  [switch] $Publish=$false,
  [switch] $Beta=$false
)

function ReplaceInFile {
  param (
    [string] $filePath,
    [string] $matchExpression,
    [string] $replacedValue
  )
  ((Get-Content $filePath) -replace $matchExpression, $replacedValue) | Set-Content -Path $filePath
}

function AsSingleLine {
  param ([string] $text)
  # Remove line breaks, normalize excess spacing and trim
  (($text -replace "\r?\n", "") -replace "\s+", " ").Trim()
}

function RunCmd {
  param ([string] $command)
  AsSingleLine $command | Invoke-Expression
}

if ($Version.Length -eq 0) {
  $Version = Get-Content ../.version
  if ($Version.Length -eq 0) {
    Write-Error "Please specify the target version that the package will have when published"
    exit 1
  }
}

if ($Publish -and $NuGetApiKey.Length -eq 0) {
  $NuGetApiKey = $Env:NUGET_API_KEY
  if ($NuGetApiKey.Length -eq 0) {
    Write-Error "Please provide a NuGet API key for publishing the package"
    exit 1
  }
}

if ($Beta) {
  $Version += "-beta"
  ReplaceInFile `
    -filePath:$SourceDirectory/SharpCode.csproj `
    -matchExpression "<PackageId>SharpCode</PackageId>" `
    -replacedValue "<PackageId>Beta.SharpCode</PackageId>"
}

Write-Host "Packing with configuration '$Configuration', version '$Version'..."
RunCmd @"
  dotnet pack
  --configuration $Configuration
  --output $OutDirectory
  -p:version=$Version
  $SourceDirectory
"@

if ($Beta) {
  # Restore any ID changes after publishing to the beta feed
  ReplaceInFile `
    -filePath:$SourceDirectory/SharpCode.csproj `
    -matchExpression "<PackageId>Beta.SharpCode</PackageId>" `
    -replacedValue "<PackageId>SharpCode</PackageId>"
}

if ($LASTEXITCODE -ne 0) {
  Write-Error -Message "An error occurred while creating package. dotnet pack exited with status $LASTEXITCODE"
  exit $LASTEXITCODE
} else {
  Write-Host "Packing completed successfully" -ForegroundColor Green
}

if ($Publish) {
  $FileName = "SharpCode.$Version.nupkg"
  if ($Beta) { $FileName = "Beta.$FileName" }

  Write-Host "Pushing package '$FileName' to NuGet..."
  RunCmd @"
    dotnet nuget push $OutDirectory/$FileName
    --source https://api.nuget.org/v3/index.json
    --api-key $NuGetApiKey
    --no-symbols
"@
  
  if ($LASTEXITCODE -ne 0) {
    Write-Error "An error occurred while pushing package to NuGet. dotnet nuget push exited with status $LASTEXITCODE"
    exit $LASTEXITCODE
  } else {
    Write-Host "Package published successfully" -ForegroundColor Green
  }
}
