using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    ///     @my_variable
    /// </summary>
    [Keyable, Valueable, RightMatchable]
    public sealed class PVariable : IObject
    {
        public readonly string Value;

        internal PVariable(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            Value = value;
        }

        #region Operators overloading
        public static bool operator ==(PVariable lhs, string rhs) => lhs.Value == rhs;
        public static bool operator !=(PVariable lhs, string rhs) => lhs.Value != rhs;
        public static bool operator ==(PVariable lhs, PVariable rhs) => lhs.Value == rhs.Value;
        public static bool operator !=(PVariable lhs, PVariable rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PVariable v && v == this;
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
    }
}