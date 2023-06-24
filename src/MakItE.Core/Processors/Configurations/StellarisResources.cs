namespace MakItE.Core.Processors.Configurations
{
    public class StellarisResources : IResource
    {
        public readonly string _root;
        public string Root => _root;

        readonly (string Dir, string Mask)[] _filter = new[]
        {
                (@"common", "*.txt"),
                (@"events", "*.txt"),
                //(@"interface", "*.gui")
        };

        public IEnumerable<string> GetFiles() => _filter
            .SelectMany(s => Directory.EnumerateFiles(Path.Combine(_root, s.Dir), s.Mask, SearchOption.AllDirectories))
            .Select(s => Path.GetRelativePath(_root, s));

        public StellarisResources(string root) => _root = root;
        public StellarisResources() => _root = @"D:\SteamLibrary\steamapps\common\Stellaris";
    }
}
