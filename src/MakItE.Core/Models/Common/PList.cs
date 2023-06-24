using MakItE.Core.Helpers;
using MakItE.Core.Models.Markers;
using MakItE.Core.Services;

namespace MakItE.Core.Models.Common
{
    [Valueable, RightMatchable]
    public sealed class PList : PCollection<IObject>, IObject
    {
        internal PList(IEnumerable<IObject> nodes) : base(MementoService.Instance, nodes) { }

        #region Operators overloading
        public static bool operator ==(PList lhs, PList rhs) => Enumerable.SequenceEqual(lhs, rhs);
        public static bool operator !=(PList lhs, PList rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PList v && v == this;
        public override int GetHashCode() => HashCodeHelper.Combine(this.Select(o => o.Value));
        #endregion
    }
}