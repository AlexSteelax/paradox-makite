using MakItE.Core.Models;

namespace MakItE.Core.Helpers
{
    internal static class HashCodeHelper
    {
        public static int Combine<T>(IEnumerable<T> values)
        {
            var hc = new HashCode();
            
            foreach (var code in values)
                hc.Add(code);

            return hc.ToHashCode();
        }
    }
}
