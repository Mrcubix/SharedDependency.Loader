namespace MyPlugin.Lib
{
    public class MySharedObject
    {
        public static MySharedObject? Instance { get; set; }

        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name ?? "Unknown";
    }
}