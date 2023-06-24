namespace MakItE.Core.Models.Common
{
    public sealed class PDocument
    {
        public readonly Dictionary<string, List<PRoot>> _items = new();

        public IEnumerable<PRoot> FindAll(string url)
        {
            if (_items.TryGetValue(url, out var list))
            {
                return list;
            }
            return Enumerable.Empty<PRoot>();
        }
        public PCollection<IObject>? FindActual(string url)
        {
            if (_items.TryGetValue(url, out var list))
            {
                return list.Last().Items;
            }
            return null;
        }
        /*
        public void Add(PRoot root)
        {
            var key = $"{root.Directory}/{root.Name}";
            
            if (_items.ContainsKey(key))
            {
                _items[key].Add(root);
            }
            else
            {
                _items.Add(key, new() { root });
            }
        }
        */
    }
}
