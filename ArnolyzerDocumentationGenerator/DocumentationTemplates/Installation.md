---
title: Arnolyzer - Installation
layout: default
---
### Installing the Arnolyzer Analyzers ###
The Arnolyzer Analyzers (hereafter, just Arnolyzer) is available as a [nuget package](https://www.nuget.org/packages/Arnolyzer/). It isn't available as a VSIX package. The consequence of this is that it must be installed for each solution individually. The upside to the nuget approach is that the rule-exception attributes are made available via a standard nuget library reference. (see what is installed for more details).

The following instructions are for installing Arnolyzer into a solution for Visual Studio 2015 only as previous versions do not support Roslyn and thus cannot run Roslyn-based analyzers.

To install it, either run `Install-Package Arnolyzer` within the [Package Manager Console](https://docs.nuget.org/consume/package-manager-console) in Visual Studio, or select "`Manage NuGet Packages for solution...`" from the solution's context menu and add it to the desired projects.

### What is installed ###
The nuget package adds four dll's to the `References -> Analyzers` section of each project selected when the nuget package is installed. They are:
* `ArnolyzerAnalyzers.dll` - This is the analyzers assembly itself and the list of analyzers supported by the version installed show up below this.
* `JetBrains.Annotations.dll`, `SuccincT.dll` and `YamlDotNet.dll` - These three assemblies are used by `ArnolyzerAnalyzers` but do not contain analyzers themselves. Due to the way Visual Studio exposes other assemblies to an analyzer assembly, they must be installed alongside the analyzer and show up accordingly. They can effectively be ignored, but are documented here for anyone curious as to why they are there.

The `ArnolyzerAnalyzers.dll` assembly is also exposed to each project as a normal reference. This allows the rule-exception attributes (in the `Arnolyzer.RuleExceptionAttributes` namespace) to be used by those projects to suppress Arnolyzer diagnostics when necessary.

### Changing the severity/disabling analyzers you don't like ###
You can modify the severity level of individual analyzers for each project that it is added to via the `Solution Explorer` window.

Navigate to `References` -> `Analyzers` -> `ArnolyzerAnalyzers` and expand the list below that. Then right-click on the analyzer of interest, select the oddly named menu item `Set Rule Set Severity` and select the desired reporting level. Selecting `None` turns off the rule completely.

### Disabling the checking of auto-generated code and other files ###
Currently, any file containing the `// <auto-generated>` comment before the start of the code in that file, will be ignored by the Arnolyzer analyzers.

To disable the checking of other files, such as `Resources.Designer.cs`, the `arnolyzer.yml` configuration file can be used to specify ignored paths and files.

#### Where to locate `arnolyzer.yml` ####
The Arnolyzer analyzers look for a `arnolyzer.yml` file in the following locations:
1. In the project directory (ie, the first directory, containing a .csproj file, found by starting at the first analysed source file's directory and moving up the directory hierarchy to the root).
2. In the solution directory (ie, the first directory, containing a .sln file, found by starting at the first analysed source file's directory and moving up the directory hierarchy to the root).
3. In the directory pointed to by the `ARNOLYZER_HOME` environment variable.

#### Format of the file ####
The format of the settings file is fairly simple, containing just three components:
````
DoNotTraverse : false
IgnoreArnolyzerHome : false
IgnorePaths: 
  - "SomeClass.cs"
  - "GeneratedClasses"
  - "Exluded*.cs"
````
##### `DoNotTraverse` #####
By default, the Arnolyzer analyzers look in the project directory, then the solution directory, then in the directory pointed to by the `ARNOLYZER_HOME` environment variable, in that order. If `DoNotTraverse` is `true` in either the project or solution settings file, then this process is halted and the subsequence settings files are not processed.
##### `IgnoreArnolyzerHome` #####
If `true` in the settings file in either the project or solution directory, then the settings file in the directory pointed to by the `ARNOLYZER_HOME` environment variable is not processed.
##### `IgnorePaths` #####
This defines a set of paths that will be ignored by the analyzers. The matching process is "aggressive", ie `GeneratedClasses` will match any occurrence of that string in the path, eg `C:\Development\Project\NonGeneratedClasses` will be matched and all files below that point ignored.

The wildcard character, `*`, can be used to create sets of similar named classes. For example, `Excluded*.cs` will match `Excluded.cs` as well as `C:\Development\Project\ExcludedClasses\SomeFile.cs`. 