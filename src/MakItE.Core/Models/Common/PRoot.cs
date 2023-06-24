using static MakItE.Core.Models.Common.IObject;

namespace MakItE.Core.Models.Common
{
    public sealed class PRoot
    {
        public readonly string Url;
        public readonly PList Items;

        internal int Index { get; }

        public PRoot(string url): this(Enumerable.Empty<IObject>(), url) { }

        public PRoot(IEnumerable<IObject> items, string url)
        {
            ValidateUrl(url);

            Url = url;
            Items = NewList(items);
        }

        void ValidateUrl(string url)
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
        }
    }
}
