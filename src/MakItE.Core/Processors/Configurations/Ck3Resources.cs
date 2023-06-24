namespace MakItE.Core.Processors.Configurations
{
    public class Ck3Resources : IResource
    {
        public readonly string _root;
        public string Root => _root;

        readonly (string Dir, string Mask)[] _filter = new[]
        {
                (@"common", "*.txt"),
                (@"events", "*.txt"),
                //(@"gui", "*.gui")
        };

        public IEnumerable<string> GetFiles() => _filter
            .SelectMany(s => Directory.EnumerateFiles(Path.Combine(_root, s.Dir), s.Mask, SearchOption.AllDirectories))
            .Select(s => Path.GetRelativePath(_root, s));

        public Ck3Resources(string root) => _root = root;
        public Ck3Resources() => _root = @"D:\SteamLibrary\steamapps\common\Crusader Kings III\game";
    }
}
