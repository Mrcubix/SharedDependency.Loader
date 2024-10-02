using System.Runtime.CompilerServices;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using MyPlugin.Lib;
using SharedDependency.Loader;

#pragma warning disable CA2255

namespace MyPlugin;

[PluginName("My Tool")]
public class MyTool : ITool
{
    public static bool DependenciesLoaded { get; private set; }

    [ModuleInitializer]
    public static void InitializeModule()
    {
        var dependencyLoader = new SharedDependencyLoader("SharedDependency.Lib");

        // Attempt at loading the dependencies if not already loaded
        if (DependenciesLoaded == false)
            DependenciesLoaded = dependencyLoader.Load();

        if (DependenciesLoaded)
            InitializeCore();
    }

    private static void InitializeCore()
    {
        MySharedObject.Instance = new MySharedObject()
        {
            Name = "SharedDependency"
        };
    }

    public bool Initialize() => true;

    public void Dispose()
    {

    }
}
