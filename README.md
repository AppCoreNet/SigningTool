AppCore .NET Signing Tool
-------------------------

[![Build Status](https://dev.azure.com/AppCoreNet/SigningTool/_apis/build/status/AppCoreNet.SigningTool%20CI?branchName=dev)](https://dev.azure.com/AppCoreNet/SigningTool/_build/latest?definitionId=14&branchName=dev)

The AppCore .NET Signing Tool is a .NET CLI tool which can be used to sign assemblies.

The motivation behind this project is to provide a cross-platform replacement of the 'sn' tool provided
by the classic .NET Framework.

## Installation

The tool is distributed as a [NuGet](https://nuget.org) package and can be easily installed by invoking the following
command:

```
dotnet tool install -g AppCore.SigningTool
```

## Usage

The tool can be invoked using the command `dotnet-signtool`. If you need help about available
commands and options add `--help` to the command line.

### Quickstart

The easiest way to add strong-naming to your assemblies is by signing them directly during the build.

First, create a key pair using the following command:

```
dotnet-signtool create-key my_key.snk
```

Next, edit your project file and add the following tags:

```
<PropertyGroup>
  <SignAssembly>true</SignAssembly>
  <AssemblyOriginatorKeyFile>my_key.snk</AssemblyOriginatorKeyFile>
</PropertyGroup>
```

For more information please read the [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/assembly/strong-named) about
strong-named assemblies.

## Contributing

Contributions, whether you file an issue, fix some bug or implement a new feature, are highly appreciated. The whole user community
will benefit from them.

If you want to try out the latest development version you can specify our [MyGet](https://www.myget.org/gallery/appcorenet)
feed during installation:

```
dotnet tool install -g AppCore.SigningTool --add-source https://www.myget.org/F/appcorenet/api/v3/index.json
```

Please refer to the [Contribution guide](CONTRIBUTING.md).
