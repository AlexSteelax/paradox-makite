using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    public enum PdxNumberKind
    {
        None,
        Percent
    }
    /// <summary>
    /// Example:
    ///     10.5
    ///     -10
    ///     5%
    /// </summary>
    [Keyable, Valueable, RightMatchable]
    public sealed class PNumber : IObject
    {
        public readonly decimal Value;
        public readonly PdxNumberKind Kind;

        internal PNumber(decimal value, PdxNumberKind kind) => (Value, Kind) = (value, kind);

        #region Operators overloading
        public static bool operator ==(PNumber lhs, decimal rhs) => lhs.Value == rhs;
        public static bool operator !=(PNumber lhs, decimal rhs) => lhs.Value != rhs;
        public static bool operator >(PNumber lhs, decimal rhs) => lhs.Value > rhs;
        public static bool operator <(PNumber lhs, decimal rhs) => lhs.Value < rhs;
        public static bool operator >=(PNumber lhs, decimal rhs) => lhs.Value >= rhs;
        public static bool operator <=(PNumber lhs, decimal rhs) => lhs.Value <= rhs;

        public static bool operator ==(PNumber lhs, PdxNumberKind rhs) => lhs.Kind == rhs;
        public static bool operator !=(PNumber lhs, PdxNumberKind rhs) => lhs.Kind != rhs;

        public static bool operator ==(PNumber lhs, PNumber rhs) => lhs.Value == rhs.Value && lhs.Kind == rhs.Kind;
        public static bool operator !=(PNumber lhs, PNumber rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PNumber v && v == this;
        public override int GetHashCode() => (Value, Kind).GetHashCode();
        #endregion
    }
}