using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    public enum PdxColorKind
    {
        RGB,
        HSV
    }
    [Valueable]
    public sealed class PColor: IObject
    {
        public readonly PdxColorKind Kind;
        public readonly decimal Value1;
        public readonly decimal Value2;
        public readonly decimal Value3;

        internal PColor(decimal v1, decimal v2, decimal v3, PdxColorKind kind) => (Value1, Value2, Value3, Kind) = (v1, v2, v3, kind);

        #region Operators overloading
        public static bool operator ==(PColor lhs, (decimal, decimal, decimal) rhs) => lhs.Value1 == rhs.Item1 && lhs.Value2 == rhs.Item2 && lhs.Value3 == rhs.Item3;
        public static bool operator !=(PColor lhs, (decimal, decimal, decimal) rhs) => !(lhs == rhs);
        public static bool operator ==(PColor lhs, PdxColorKind rhs) => lhs.Kind == rhs;
        public static bool operator !=(PColor lhs, PdxColorKind rhs) => !(lhs == rhs);
        public static bool operator ==(PColor lhs, PColor rhs) => lhs.Value1 == rhs.Value1 && lhs.Value2 == rhs.Value2 && lhs.Value3 == rhs.Value3 && lhs.Kind == rhs.Kind;
        public static bool operator !=(PColor lhs, PColor rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PColor v && v == this;
        public override int GetHashCode() => (Value1, Value2, Value3, Kind).GetHashCode();
        #endregion
    }
}
