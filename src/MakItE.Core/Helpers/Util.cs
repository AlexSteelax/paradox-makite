using MakItE.Core.Models.Collection;
using MakItE.Core.Models.Common;

namespace MakItE.Core.Helpers
{
    public static class Util
    {
        public static T As<T>(this PCollectionNode<IObject> node) where T: IObject => node.Value.As<T>();
        public static string UrlNormalize(this string url) => url.Replace('\\', '/').TrimStart('/');
    }
}
