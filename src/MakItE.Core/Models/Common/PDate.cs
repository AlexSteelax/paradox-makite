using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    /// 1001.05.28
    /// </summary>
    [Keyable, Valueable, RightMatchable]
    public sealed class PDate : IObject
    {
        public readonly DateOnly Value;
        internal PDate(DateOnly value) => Value = value;

        #region Operators overloading
        public static bool operator ==(PDate lhs, DateTime rhs) => lhs.Value == DateOnly.FromDateTime(rhs);
        public static bool operator !=(PDate lhs, DateTime rhs) => lhs.Value != DateOnly.FromDateTime(rhs);
        public static bool operator ==(PDate lhs, DateOnly rhs) => lhs.Value == rhs;
        public static bool operator !=(PDate lhs, DateOnly rhs) => lhs.Value != rhs;

        public static bool operator ==(PDate lhs, PDate rhs) => lhs.Value == rhs.Value;
        public static bool operator !=(PDate lhs, PDate rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PDate v && v == this;
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
    }
}