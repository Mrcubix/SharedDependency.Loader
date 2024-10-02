using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using MyPlugin.Lib;

namespace SharedDependency.DependentPlugin;

[PluginName(PLUGIN_NAME)]
public class DependentTool : ITool
{
    public const string PLUGIN_NAME = "MyPlugin.Lib Dependent Tool";

    internal const string SingleDependencyTypeName = "MyPlugin.Lib.MySharedObject, MyPlugin.Lib";
    internal readonly Type? SharedDependencyType = Type.GetType(SingleDependencyTypeName);
    internal readonly bool DependencyLoaded;

    public DependentTool()
    {
        DependencyLoaded = SharedDependencyType != null;
    }

    public bool Initialize()
    {
        if (DependencyLoaded)
            InitializeCore();
        else
            Log.Write(PLUGIN_NAME, "Shared dependency not loaded");

        return true;
    }

    private void InitializeCore()
    {
        if (MySharedObject.Instance != null)
        {
            Log.Write(PLUGIN_NAME, "Shared object is not null");
            Log.Write(PLUGIN_NAME, $"Name of shared object: {MySharedObject.Instance.Name}");
        }
    }

    public void Dispose()
    {
        
    }
}
