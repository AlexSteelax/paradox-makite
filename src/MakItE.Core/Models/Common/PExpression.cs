using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    ///     @[ ... ]
    /// </summary>
    [Valueable, RightMatchable]
    public sealed class PExpression : IObject
    {
        public readonly string Value;

        internal PExpression(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            Value = value;
        }

        #region Operators overloading
        public static bool operator ==(PExpression lhs, string rhs) => lhs.Value == rhs;
        public static bool operator !=(PExpression lhs, string rhs) => lhs.Value != rhs;
        public static bool operator ==(PExpression lhs, PExpression rhs) => lhs.Value == rhs.Value;
        public static bool operator !=(PExpression lhs, PExpression rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PExpression v && v == this;
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
    }
}
