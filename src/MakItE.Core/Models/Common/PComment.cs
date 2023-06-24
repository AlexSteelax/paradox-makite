using MakItE.Core.Helpers;
using System.Collections;

namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    /// # My test message
    /// </summary>
    public sealed class PComment : IObject, IEnumerable<string>
    {
        readonly string[] _items;
        public int Count => _items.Length;

        internal PComment(IEnumerable<string> values) => _items = values.ToArray();

        #region Operators overloading
        public static bool operator ==(PComment lhs, PComment rhs) => Enumerable.SequenceEqual(lhs, rhs);
        public static bool operator !=(PComment lhs, PComment rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PComment v && v == this;
        public override int GetHashCode() => HashCodeHelper.Combine(_items);
        #endregion

        public IEnumerator<string> GetEnumerator() => _items.Cast<string>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}