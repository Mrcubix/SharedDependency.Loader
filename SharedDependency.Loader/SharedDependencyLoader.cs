using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SharedDependency.Loader
{
    public class SharedDependencyLoader
    {
        internal static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        internal static readonly string? _assemblyName = _assembly.GetName().Name;

        private static readonly AssemblyLoadContext defaultContext = AssemblyLoadContext.Default;
        private static readonly string?[] assemblies = defaultContext.Assemblies.Select(x => x.GetName().Name).ToArray();
        private readonly string _embeddedPath;

        public SharedDependencyLoader(string name)
        {
            Name = name;
            _embeddedPath = $"{_assemblyName}.libs.{Name}.dll";
        }

        public string Name { get; init; }
        public bool Loaded { get; private set; }

        public bool Load()
        {
            if (Loaded || CheckLoaded())
            {
                Loaded = true;
                return true;
            }

            if (_assemblyName == null)
                return false;

            if (_assembly.GetManifestResourceStream(_embeddedPath) is not Stream stream)
                return false;

            try
            {
                defaultContext.LoadFromStream(stream);
                Loaded = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool CheckLoaded()
        {
            return assemblies.Any(assembly => assembly == "SharedDependency.Lib");
        }
    }
}