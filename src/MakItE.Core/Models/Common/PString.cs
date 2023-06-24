using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    ///     "some text"
    ///     'some text'
    /// </summary>
    [Valueable, RightMatchable]
    public sealed class PString : IObject
    {
        public readonly string Value;

        internal PString(string value) => Value = value;

        #region Operators overloading
        public static bool operator ==(PString lhs, string rhs) => lhs.Value == rhs;
        public static bool operator !=(PString lhs, string rhs) => lhs.Value != rhs;
        public static bool operator ==(PString lhs, PString rhs) => lhs.Value == rhs.Value;
        public static bool operator !=(PString lhs, PString rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PString v && v == this;
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
    }
}