# SharedDependency.Loader

The goal of this project is to allow plugins to load a shared library into the default context as specified by Microsoft.
This will allow other plugins to then make use of the shared Assembly available in that context,
Effectively removing the need for it to be included in their files.

Do not include the shared library in your plugin archive, otherwise your plugin will not be able to make use of the shared Assembly.

## How to build

1. Define your roots in Directory.Build.props

```xml
<PropertyGroup>
  <!-- Obtain the root of your repo using the way of your choice -->
  <ProjectsRoot>$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), '.git/HEAD'))/Example</ProjectsRoot>
  <DependencyPropsRoot>$(ProjectsRoot)/Props</DependencyPropsRoot>
</PropertyGroup>
```

or define the properties in your projet's reference to the loader

```xml
<ProjectReference Include="../../SharedDependency.Loader/SharedDependency.Loader.csproj">
  <AdditionalProperties>
    DependencyPropsRoot=$(DependencyPropsRoot)
    ProjectsRoot=$(ProjectsRoot)
  </AdditionalProperties>
</ProjectReference>

```

2. Define your dependencies in any of the target files at /Targets

```xml
<Project>
  
  <ItemGroup Label="References" Condition=" '$(TargetFramework)' == 'net6.0' ">
    <ProjectReference Include="$(ProjectsRoot)/Example/MyPlugin.Lib/MyPlugin.Lib.csproj" />
    <ProjectReference Include="$(ProjectsRoot)/Example/MyPlugin.Lib/MyPlugin.Lib.csproj" />
  </ItemGroup>

  <ItemGroup Label="Files" Condition=" '$(TargetFramework)' == 'net6.0' ">
    <Dependency Include="SharedDependency.Lib" />
    <Dependency Include="SharedDependency.OtherLib" />
  </ItemGroup>

</Project>
```

3. Reference the loader in your project

```xml
<ProjectReference Include="../../SharedDependency.Loader/SharedDependency.Loader.csproj" />
```

or perform a standalone build

```bash
dotnet publish SharedDependency.Loader -c Release -f net6.0
```

#### Note

You can also pass `DependencyPropsRoot` & `ProjectsRoot` to msbuild when using the command line.

From the root of your repo in bash :

```bash
project_root=$(pwd)/Example
dotnet publish SharedDependency.Loader -c Release -f net6.0 /p:DependencyPropsRoot="$project_root/Props" /p:ProjectsRoot="$project_root"
```

or from the root of your repo in powershell :

```ps1
$Root = "$((Get-Item .).FullName)/Example"
dotnet publish SharedDependency.Loader -c Release -f net6.0 -p:DependencyPropsRoot="$Root/Props/" /p:ProjectsRoot=$Root
```

## How to use

1. Make sure the loader is loaded before the shared libraries are used by the dependent Assemblies

For a plugin, this is usually done by changing the load order when available or changing the parent directory's name to be first in an iteration.

You may then use the `SharedDependencyLoader` like so :

```cs
static MyClass()
{
    var dependencyLoader = new SharedDependencyLoader("SharedDependency.Lib");

    // Attempt at loading the dependencies if not already loaded
    if (DependenciesLoaded == false)
        DependenciesLoaded = dependencyLoader.Load();

    if (DependenciesLoaded)
        InitializeCore();
}
```

On the dependant side, you may check that the assembly is loaded properly before using it :

##### Iterating through all loaded assemblies

```cs

private static readonly AssemblyLoadContext defaultContext = AssemblyLoadContext.Default;
private static readonly string?[] assemblies = defaultContext.Assemblies.Select(x => x.GetName().Name).ToArray();

public DependantPlugin()
{
    assemblies.Any(assembly => assembly == "SharedDependency.Lib")
}
```

##### Checking if the shared dependency Type is null (not loaded)

```cs
internal readonly Type? SharedDependencyType = Type.GetType(SingleDependencyTypeName);
internal readonly bool DependencyLoaded;

public DependantPlugin()
{
    DependencyLoaded = SharedDependencyType != null;
}
```